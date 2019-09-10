using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Normal, Hard, Expert
}

[Serializable]
public enum ControlType
{
    Touch, Joystick
}

public enum GameState
{
    Main, Menu, InGame, Option, Shop
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
    public UnityEngine.UI.Text timeScaleSetText;

    public UnityEngine.UI.Image panel;
    public UnityEngine.UI.Text coinText;
    private float[] difficultys = new float[3] { 1.25f, 1f, 0.85f };
    public float applyTimeScale;

    public bool isPause;
    private bool stopFlag;
    public GameObject pauseButton;
    private Color curPlayerColor;

    public bool isMusicStarted;
    public TestPlayerJoyStic testPlayerJoyStick;
    public string curSongName;
    private Difficulty dif;

    public Difficulty difficulty
    {
        get { return dif; }
        set
        {
            dif = value;
            for (int i = 0; i < 3; i++)
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
        Invoke("AddAdGold", 0.3f);
    }

    [SerializeField]
    public UnityEngine.UI.Slider moveSensitiveSlider;

    public UnityEngine.UI.Slider timeScaleSlider;

    public float GetApplyTimeScale(float t)
    {
        return t * applyTimeScale;
    }

    public float GetApplyTimeScale()
    {
        return applyTimeScale;
    }

    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            PlayerPrefs.SetInt("Gold", gold);
            coinText.text = gold.ToString();
        }
    }

    public GameState state
    {
        get { return gs; }
        set
        {
            if (gs == GameState.Main || gs == GameState.Option || gs == GameState.Shop)
            {
                stopFlag = true;
            }
            else
            {
                if (gs == GameState.Menu)
                {
                    if (value != GameState.InGame)
                    {
                        stopFlag = true;
                    }
                    else
                    {
                        stopFlag = false;
                    }
                }
                else
                    stopFlag = false;
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
            if (gs == GameState.Option || gs == GameState.InGame)
            {
                if (controlType == ControlType.Joystick)
                    Joystick.instance.gameObject.SetActive(true);
            }
            else
            {
                Joystick.instance.gameObject.SetActive(false);
            }
            Joystick.instance.JoyStickInit();
            UIManager.instance.Init();
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
        controlType = (ControlType)PlayerPrefs.GetInt("ControlType", (int)ControlType.Touch);
    }

    // Use this for initialization
    private void Start()
    {
        switch (controlType)
        {
            case ControlType.Joystick:
                MenuManager.instance.ChangeControlTypeToJoyStick();
                break;

            case ControlType.Touch:
                MenuManager.instance.ChangeControlTypeToTouch();
                break;
        }
        Gold = PlayerPrefs.GetInt("Gold", 0);
        isPause = false;
        moveSensitiveSlider.onValueChanged.AddListener(delegate { MoveSensitiveAdapt(); });
        moveSensitiveSlider.value = (PlayerPrefs.GetFloat("MoveSensitive", 1f) - 0.2f) * 1.25f;
        timeScaleSlider.onValueChanged.AddListener(delegate { TextApply(); });
        float timescale = PlayerPrefs.GetFloat("TimeScale", 1);
        timeScaleSlider.value = (timescale - 0.75f) / 2;
        string str = string.Format("TimeSlcale x {0:f2}", timescale);
        timeScaleSetText.text = str;
        Time.timeScale = timescale;
        Player.instance.AdaptTimeScale();
        testPlayerJoyStick.AdaptTimeScale();
        applyTimeScale = Time.timeScale;
        difficulty = Difficulty.Normal;
        InGameManager.instance.Gold = PlayerPrefs.GetInt("Gold", 0);
        float r = PlayerPrefs.GetFloat("PlayerColorR", 0.184f);
        float g = PlayerPrefs.GetFloat("PlayerColorG", 1);
        float b = PlayerPrefs.GetFloat("PlayerColorB", 1);
        Player.instance.SetPlayerColor(new Color(r, g, b));
        state = GameState.Main;
    }

    public void ChangeControlType(ControlType control)
    {
        controlType = control;
        Player.instance.ApplyChangedControlType(control);
        PlayerPrefs.SetInt("ControlType", (int)controlType);
    }

    public void AddAdGold()
    {
        int addgold = 1000;
        Gold += addgold;
    }

    public void AddPlayGold(int t)
    {
        Gold += t;
    }

    public void SetControlType(ControlType type)
    {
        controlType = type;
    }

    private void MoveSensitiveAdapt()
    {
        Player.instance.SetMoveSensitive(0.2f + moveSensitiveSlider.value * 0.8f);
    }

    public void SetPerfactSliderValue()
    {
        int temp = (int)(timeScaleSlider.value / 0.125);
        float _temp = timeScaleSlider.value - (temp * 0.125f);
        if (_temp >= 0.125f / 2)
        {
            if (temp != 8)
                temp += 1;
        }

        timeScaleSlider.value = temp * 0.125f;
        applyTimeScale = 0.75f + timeScaleSlider.value * 2;
        string str = string.Format("TimeSlcale x {0:f2}", applyTimeScale);
        timeScaleSetText.text = str;
        Time.timeScale = applyTimeScale;
        Player.instance.AdaptTimeScale();
        testPlayerJoyStick.AdaptTimeScale();
        PlayerPrefs.SetFloat("TimeScale", applyTimeScale);
        PlayerPrefs.Save();
    }

    private void TextApply()
    {
        int temp = (int)(timeScaleSlider.value / 0.125);
        float _temp = timeScaleSlider.value - (temp * 0.125f);
        if (_temp >= 0.125f / 2)
        {
            if (temp != 8)
                temp += 1;
        }
;
        applyTimeScale = 0.75f + temp * 0.125f * 2;

        string str = string.Format("TimeSlcale x {0:f2}", applyTimeScale);
        if (timeScaleSetText.text != str)
        {
            Vibration.Vibrate(1);
        }
        timeScaleSetText.text = str;
    }

    public float GetDifficulty()
    {
        return difficultys[(int)difficulty];
    }

    public void RevivalReady()
    {
        Invoke("Revival", 0.3f);
    }

    private void Revival()
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
        UIManager.instance.coinText.text = (int)(SoundManager.instance.curSongScore * 0.2f * ((int)difficulty + 1) * applyTimeScale) + "";
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

    private IEnumerator ReadyPlay()
    {
        while (Input.touchCount <= 0)
        {
            yield return null;
        }
        UIManager.instance.Pause();
    }
}