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

public enum GameState
{
    Main, Menu, InGame
}

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    public ParticleSystem dieEffect;

    public List<GameObject> uiList;
    public List<GameObject> objectList;
    public GameObject dieMenu;

    public bool isPause;

    public bool isMusicStarted;

    public string curSongName;

    public Difficulty difficulty;
    public ControlType controlType;
    private GameState gs;

    public GameState state
    {
        get { return gs; }
        set
        {
            gs = value;
            for (int i = 0; i < uiList.Count; i++)
            {
                uiList[i].SetActive(false);
                objectList[i].SetActive(false);
            }
            uiList[(int)gs].SetActive(true);
            objectList[(int)gs].SetActive(true);
            if (gs == GameState.InGame)
            {
                SoundManager.instance.InitScene();
            }
            if (SoundManager.instance.curSong != null)
            {
                SoundManager.instance.curSong.Stop();
                SoundManager.instance.curSong.time = 0;
            }
        }
    }

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
        state = GameState.Menu;
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