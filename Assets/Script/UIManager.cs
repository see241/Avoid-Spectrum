using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;

    public TextMesh text;

    // Use this for initialization
    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Pause()
    {
        if (InGameManager.instance.isPause == false)
        {
            Time.timeScale = 0;
            SoundManager.instance.SoundPause();
            InGameManager.instance.isPause = true;
            pauseMenu.SetActive(true);
            SoundManager.instance.SetUIInfo();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        InGameManager.instance.isPause = false;
        pauseMenu.SetActive(false);
        PoolManager.instance.CleanPool();
        SoundManager.instance.CloseScene();
        SoundManager.instance.InitScene();
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
        InGameManager.instance.state = GameState.Menu;
    }

    public void Resume()
    {
        StartCoroutine(_Resume());
    }

    private IEnumerator _Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            TextMesh _text = Instantiate(text);
            _text.text = (3 - i).ToString();
            yield return new WaitForSeconds(1f);
            Destroy(_text.gameObject);
        }

        InGameManager.instance.isPause = false;
        SoundManager.instance.SoundResume();
    }
}