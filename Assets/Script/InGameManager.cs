using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Normal, Hard, Expert
}

public enum ControlType
{
    Touch, Joystick
}

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    public ParticleSystem dieEffect;

    public GameObject dieMenu;

    public bool isPause;

    public string curSongName;

    public Difficulty difficulty;
    public ControlType controlType;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        if (PlayerPrefs.HasKey("ControlType"))
        {
            controlType = (ControlType)PlayerPrefs.GetInt("ControlType");
        }
        else
        {
            controlType = ControlType.Touch;
        }
        isPause = false;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void PlayerDie(GameObject player)
    {
        ParticleSystem ptc = Instantiate(dieEffect);
        ptc.transform.position = player.transform.position;
        Destroy(ptc, ptc.duration + ptc.startLifetime);
        SoundManager.instance.SetUIInfo();
        dieMenu.SetActive(true);

        SoundManager.instance.curSong.volume /= 2;
    }

    private void ChangeControlType(ControlType type)
    {
        controlType = type;
        PlayerPrefs.SetInt("ControlType", (int)controlType);
    }
}