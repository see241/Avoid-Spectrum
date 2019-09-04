using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject pauseMenu;
    public GameObject dieMenu;
    public GameObject clearMenu;
    public GameObject pauseButton;
    private bool swPause;
    public TextMesh text;
    public Button revivalButton;
    public Image revivalButtonImage;
    

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (SoundManager.instance.isClear)
        {
            Clear();
        }
    }

    public void Pause()
    {
        if (swPause)
        {
            Resume();
        }
        if (InGameManager.instance.isPause == false)
        {
            Time.timeScale = 0;
            SoundManager.instance.SoundPause();
            InGameManager.instance.isPause = true;
            pauseMenu.SetActive(true);
            SoundManager.instance.SetUIInfo();
            swPause = true;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        InGameManager.instance.isPause = false;
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        clearMenu.SetActive(false);
        PoolManager.instance.CleanPool();
        SoundManager.instance.CloseScene();
        SoundManager.instance.InitScene();
        Player.instance.Init();
        pauseButton.SetActive(true);
        Player.instance.isRevival = false;
    }
    public void Revival()
    {
        revivalButton.interactable = false;
        AdMobManager.instance.ShowRewardBasedVideo();
        //InGameManager.instance.Revival();
    }
    public void Clear()
    {
        clearMenu.SetActive(true);
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        SoundManager.instance.SetUIInfo();
    }
    
    public void BackHome()
    {
        Time.timeScale = 1;
        PoolManager.instance.CleanPool();
        SoundManager.instance.AllSoundStop();
        SoundManager.instance.CloseScene();
        PoolManager.instance.CleanPool();
        InGameManager.instance.isPause = false;
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        clearMenu.SetActive(false);
        pauseButton.SetActive(true);
        Player.instance.isRevival = false;

        InGameManager.instance.state = GameState.Menu;
        GameObject.Find("MenuManager").GetComponent<MenuManager>().index = GameObject.Find("MenuManager").GetComponent<MenuManager>().index;
    }

    public void Resume()
    {
        StartCoroutine(_Resume());
    }

    private IEnumerator _Resume()
    {
        swPause = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            TextMesh _text = Instantiate(text,transform);
            _text.text = (3 - i).ToString();
            yield return new WaitForSeconds(1f);
            Destroy(_text.gameObject);
        }

        InGameManager.instance.isPause = false;
        SoundManager.instance.SoundResume();
    }
    public void StartRevivalTimer()
    {
        StartCoroutine(RevivalTimer());
    }
    IEnumerator RevivalTimer()
    {
        revivalButton.interactable = true;
        revivalButtonImage.fillAmount = 1;
        while (revivalButtonImage.fillAmount>0)
        {
            revivalButtonImage.fillAmount -= (1 / 5.0f) * Time.deltaTime;
            yield return null;
        }
        revivalButton.interactable = false;
    }
}