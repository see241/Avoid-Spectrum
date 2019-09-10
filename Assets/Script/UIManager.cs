using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject pauseMenu;
    public GameObject dieMenu;
    public GameObject clearMenu;
    public GameObject pauseButton;
    public bool swPause;
    public TextMesh text;
    public Button revivalButton;
    public Image revivalButtonImage;
    public Image goldCoinImage;
    private RectTransform coinTransform;
    public Canvas canvas;
    public Text popupText;
    public Text coinText;
    private bool duringReceiveResult;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        coinTransform = GameObject.Find("Coin").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (SoundManager.instance.isClear)
        {
            Clear();
        }
    }

    public void Init()
    {
        duringReceiveResult = false;
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
        Time.timeScale = InGameManager.instance.applyTimeScale;
        InGameManager.instance.isPause = false;
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        clearMenu.SetActive(false);
        PoolManager.instance.CleanPool();
        SoundManager.instance.CloseScene();
        SoundManager.instance.InitScene();
        Player.instance.Init();
        pauseButton.SetActive(true);
    }

    public void GameResultApply()
    {
        if (!duringReceiveResult)
        {
            if (SoundManager.instance.isClear)
            {
                int result = 20 * ((int)InGameManager.instance.difficulty + 1) * (int)InGameManager.instance.applyTimeScale;
                StartCoroutine(AddGoldAnimation_BackHome(result));
            }
            else
            {
                int result = (int)(SoundManager.instance.curSongScore * 0.2f * ((int)InGameManager.instance.difficulty + 1) * InGameManager.instance.applyTimeScale);
                if (result > 0)
                    StartCoroutine(AddGoldAnimation_BackHome(result));
                else
                {
                    BackHome();
                }
            }
            duringReceiveResult = true;
        }
        //StartCoroutine(AddGoldAnimation(100));
    }

    public void GameResultApplyRestart()
    {
        if (SoundManager.instance.isClear)
        {
            int result = 20 * ((int)InGameManager.instance.difficulty + 1) * (int)InGameManager.instance.applyTimeScale;
            StartCoroutine(AddGoldAnimation_Restart(result));
        }
        else
        {
            int result = (int)(SoundManager.instance.curSongScore * 0.2f * ((int)InGameManager.instance.difficulty + 1) * InGameManager.instance.applyTimeScale);
            if (result > 0)
                StartCoroutine(AddGoldAnimation_Restart(result));
            else
            {
                BackHome();
            }
        }
    }

    public void Revival()
    {
        revivalButton.interactable = false;
        AdMobManager.instance.ShowRewardBasedVideo(RewardType.Revival);
        //InGameManager.instance.Revival();
    }

    public void Clear()
    {
        clearMenu.SetActive(true);
        pauseButton.SetActive(false);
        SoundManager.instance.SetUIInfo();
    }

    public void BackHome()
    {
        Time.timeScale = InGameManager.instance.applyTimeScale;
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
        Time.timeScale = InGameManager.instance.applyTimeScale;
        pauseMenu.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            TextMesh _text = Instantiate(text, transform);
            _text.text = (3 - i).ToString();
            Destroy(_text.gameObject, 1);
            yield return new WaitForSeconds(1f);
        }

        InGameManager.instance.isPause = false;
        SoundManager.instance.SoundResume();
    }

    public void StartRevivalTimer()
    {
        StartCoroutine(RevivalTimer());
    }

    private IEnumerator RevivalTimer()
    {
        revivalButton.interactable = true;
        revivalButtonImage.fillAmount = 1;
        while (revivalButtonImage.fillAmount > 0)
        {
            revivalButtonImage.fillAmount -= (1 / 5.0f) * Time.deltaTime;
            yield return null;
        }
        revivalButton.interactable = false;
    }

    private IEnumerator AddGoldAnimation_BackHome(int addGold)
    {
        Debug.Log(addGold);
        int textAdaptCoinCount = addGold;
        coinText.text = textAdaptCoinCount + "";
        int count = addGold / 3;

        for (int i = 0; i < count; i++)
        {
            Image img = Instantiate(goldCoinImage, canvas.transform);
            StartCoroutine(AddSingleGold(img, coinTransform, 3));
            textAdaptCoinCount -= 3;
            coinText.text = textAdaptCoinCount + "";
            yield return new WaitForSeconds(InGameManager.instance.GetApplyTimeScale(0.25f));
            Debug.Log("For");
        }
        if (textAdaptCoinCount > 0)
        {
            Image image = Instantiate(goldCoinImage, canvas.transform);
            textAdaptCoinCount -= addGold % 3;
            Debug.Log(addGold % 3);
            coinText.text = textAdaptCoinCount + "";
            yield return StartCoroutine(AddSingleGold(image, coinTransform, addGold % 3));
            Debug.Log("AddMod");
        }

        yield return StartCoroutine(PopUpText(addGold));
        Time.timeScale = InGameManager.instance.applyTimeScale;
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

    private IEnumerator AddGoldAnimation_Restart(int addGold)
    {
        Debug.Log(addGold);
        int textAdaptCoinCount = addGold;
        coinText.text = textAdaptCoinCount + "";
        int count = addGold / 3;

        for (int i = 0; i < count; i++)
        {
            Image img = Instantiate(goldCoinImage, canvas.transform);
            StartCoroutine(AddSingleGold(img, coinTransform, 3));
            textAdaptCoinCount -= 3;
            coinText.text = textAdaptCoinCount + "";
            yield return new WaitForSeconds(InGameManager.instance.GetApplyTimeScale(0.25f));
            Debug.Log("For");
        }
        if (textAdaptCoinCount > 0)
        {
            Image image = Instantiate(goldCoinImage, canvas.transform);
            textAdaptCoinCount -= addGold % 3;
            Debug.Log(addGold % 3);
            coinText.text = textAdaptCoinCount + "";
            yield return StartCoroutine(AddSingleGold(image, coinTransform, addGold % 3));
            Debug.Log("AddMod");
        }

        yield return StartCoroutine(PopUpText(addGold));
        Time.timeScale = InGameManager.instance.applyTimeScale;
        InGameManager.instance.isPause = false;
        pauseMenu.SetActive(false);
        dieMenu.SetActive(false);
        clearMenu.SetActive(false);
        PoolManager.instance.CleanPool();
        SoundManager.instance.CloseScene();
        SoundManager.instance.InitScene();
        Player.instance.Init();
        pauseButton.SetActive(true);
    }

    private IEnumerator AddSingleGold(Image target, RectTransform goal, int c)
    {
        Vector2 dir = goal.anchoredPosition - target.rectTransform.anchoredPosition;
        for (int i = 0; i < 30; i++)
        {
            target.rectTransform.anchoredPosition = Vector2.Lerp(target.rectTransform.anchoredPosition, coinTransform.anchoredPosition, 0.33f);
            yield return new WaitForSeconds(InGameManager.instance.GetApplyTimeScale(0.0015f));
        }
        Destroy(target.gameObject);
        InGameManager.instance.AddPlayGold(c);
    }

    private IEnumerator PopUpText(int v)
    {
        Text text = Instantiate(popupText, coinTransform);
        text.text = "+" + v;
        float t = Time.time;
        float fontsize = 12;
        text.fontSize = 12;
        while (t + InGameManager.instance.GetApplyTimeScale() > Time.time)
        {
            fontsize += Time.deltaTime * 15 / InGameManager.instance.GetApplyTimeScale();
            text.fontSize = (int)fontsize;
            text.rectTransform.anchoredPosition += Vector2.up * Time.deltaTime * 55 / InGameManager.instance.GetApplyTimeScale();
            yield return null;
        }
        Destroy(text.gameObject);
    }
}