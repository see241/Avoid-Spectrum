using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>();
    public int idx;

    public int index
    {
        get { return idx; }
        set
        {
            idx = value;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].SetActive(false);
            }
            if (index > 0)
            {
                panels[idx - 1].GetComponent<PanelSort>().index = -1;
                panels[idx - 1].SetActive(true);
            }
            panels[idx].GetComponent<PanelSort>().index = 0;
            panels[idx].SetActive(true);
            if (index < panels.Count - 1)
            {
                panels[idx + 1].GetComponent<PanelSort>().index = 1;
                panels[idx + 1].SetActive(true);
            }
            InGameManager.instance.curSongName = panels[idx].name;
        }
    }

    // Use this for initialization
    private void Start()
    {
        index = 0;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void RightButton()
    {
        if (index < panels.Count - 1)
            index++;
    }

    public void LeftButton()
    {
        if (index > 0)
            index--;
    }

    public void ChangeScene()
    {
        StartCoroutine(_ChangeScene());
    }

    private IEnumerator _ChangeScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("GameScene");
        while (!async.isDone)
        {
            Debug.Log(async.progress);
            yield return null;
        }
    }
}