using System;
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
    Main, Menu, InGame,Option,Shop
}

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    public ParticleSystem dieEffect;
    public GameObject number;
    public List<GameObject> uiList;
    public List<GameObject> objectList;
    public GameObject dieMenu;
    public List<UnityEngine.UI.Button> diffSetButtons;
    public List<UnityEngine.UI.Text> diffSetTexts;
    public UnityEngine.UI.Image panel;
    public UnityEngine.UI.Text coinText;
    float[] difficultys = new float[3] { 1.25f, 1f, 0.85f};

    public bool isPause;
    private bool stopFlag;
    public GameObject pauseButton;
    Color curPlayerColor;

    public bool isMusicStarted;

    public string curSongName;
    Difficulty dif;
    public Difficulty difficulty
    {
        get { return dif; }
        set
        {
            dif = value;
            for(int i = 0; i < 3; i++)
            {
                diffSetButtons[i].interactable = true;
                diffSetTexts[i].color = new Color(1, 1, 1, 0.5f);
            }

            diffSetButtons[(int)dif].interactable = false;
            diffSetTexts[(int)dif].color = new Color(1, 1, 1, 1);
            MenuManager.instance.SetDifficultyScore();
        }
    }
    public ControlType controlType;
    private GameState gs;
    private int gold;

    public void AdGold()
    {
        Invoke("AddAdGold",0.3f);
    }

    [SerializeField]
    public UnityEngine.UI.Slider slider;
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            PlayerPrefs.SetInt("Gold", gold);
            coinText.text = gold.ToString(  );
        }
    }

    public GameState state
    {
        get { return gs; }
        set
        {
            if (gs == GameState.Main)
            {
                stopFlag = true;
            }
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
                AdMobManager.instance.DestroyBannerAd();
                SoundManager.instance.InitScene();
                Player.instance.transform.position = Vector2.zero;
            }
            if (gs != GameState.InGame)
            {
                if (!AdMobManager.instance.IsBannerOnScreen())
                {
                    AdMobManager.instance.RequestBannerAd();
                }
            }
            if (gs == GameState.Shop)
            {
                Camera.main.transform.position = new Vector3(0, 10, -10);
            }
            else
            {
                Camera.main.transform.position = new Vector3(0, 0, -10);
            }
            if (SoundManager.instance.curSong != null)
            {
                if (!stopFlag)
                {
                    SoundManager.instance.curSong.Stop();
                    SoundManager.instance.curSong.time = 0;
                }
            }
            stopFlag = false;
            SoundManager.instance.isClear = false;
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
            controlType = (ControlType)PlayerPrefs.GetInt("ControlType",(int)ControlType.Touch);
        
        Gold = PlayerPrefs.GetInt("Gold", 0);
        state = GameState.Main;
        isPause = false;
        slider.onValueChanged.AddListener(delegate { MoveSensitiveAdapt(); });
        slider.value = (PlayerPrefs.GetFloat("MoveSensitive", 1f) - 0.2f) * 1.25f;
        difficulty = Difficulty.Normal;
        InGameManager.instance.Gold = PlayerPrefs.GetInt("Gold", 0);
        float r = PlayerPrefs.GetFloat("PlayerColorR", 0.184f);
        float g = PlayerPrefs.GetFloat("PlayerColorG", 1);
        float b = PlayerPrefs.GetFloat("PlayerColorB", 1);
        Player.instance.SetPlayerColor(new Color(r, g, b));
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            state = GameState.Shop;
    }
    public void AddAdGold()
    {
        int addgold = 1000;
        Gold += addgold;
        Debug.Log("Get "+addgold+" Gold");
    }
    public void AddPlayGold(int t)
    {
        Gold += t;
    }
    public void SetControlType(ControlType type)
    {
        controlType = type;
    }
    void MoveSensitiveAdapt()
    {
        Player.instance.SetMoveSensitive(0.2f + slider.value * 0.8f);
    }
    public float GetDifficulty()
    {
        return difficultys[(int)difficulty];
    }
    public void RevivalReady()
    {
        Invoke("Revival", 0.3f);
    }
    void Revival()
    {
        Player.instance.gameObject.SetActive(true);
        dieMenu.SetActive(false);
        pauseButton.SetActive(true);
        Player.instance.RevivalInit();
        StartCoroutine(PlayerInvisible());
    }

    public void PlayerDie(GameObject player)
    {
        ParticleSystem ptc = Instantiate(dieEffect);
        ptc.transform.position = player.transform.position;                               
        Destroy(ptc, ptc.duration + ptc.startLifetime);
        SoundManager.instance.SetUIInfo();
        dieMenu.SetActive(true);
        pauseButton.SetActive(false);
        SoundManager.instance.curSong.volume = 0.5f;
        if (!Player.instance.isRevival)
        {
            UIManager.instance.StartRevivalTimer();
            Player.instance.isRevival = true;
        }
    }

    private void ChangeControlType(ControlType type)
    {
        controlType = type;
        PlayerPrefs.SetInt("ControlType", (int)controlType);
    }

    private IEnumerator PlayerInvisible()
    {
        Player.instance.GetComponent<CircleCollider2D>().enabled = false;
        panel.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            GameObject count = Instantiate(number);
            count.GetComponent<TextMesh>().text = (3 - i).ToString();
            Destroy(count, 1);
            yield return new WaitForSeconds(1);
        }
        panel.gameObject.SetActive(false);
        SoundManager.instance.curSong.volume = 1;
        Player.instance.GetComponent<CircleCollider2D>().enabled = true;
    }
    IEnumerator ReadyPlay()
    {
        while (Input.touchCount <= 0)
        {
            yield return null;
        }
        UIManager.instance.Pause();
    }
}