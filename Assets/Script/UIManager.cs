using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject dieMenu;
    public GameObject clearMenu;
    public GameObject pauseButton;
    private bool swPause;
    public TextMesh text;

    // Use this for initialization
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
        Player.instance.gameObject.SetActive(true);
        Player.instance.transform.position = Vector2.zero;
        pauseButton.SetActive(true);
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
}