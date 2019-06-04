using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>();
    public List<float> hightTimes = new List<float>();
    public int idx;
    public Button left;
    public Button right;

    public int index
    {
        get { return idx; }
        set
        {
            idx = value;
            InGameManager.instance.curSongName = panels[idx].name;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].SetActive(false);
            }
            if (index > 0)
            {
                panels[idx - 1].GetComponent<PanelSort>().index = -1;
                panels[idx - 1].SetActive(true);
                left.interactable = true;
                if (PlayerPrefs.HasKey(panels[idx - 1].name + "Score"))
                {
                    panels[idx - 1].GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(panels[idx - 1].name + "Score") + "%";
                }
            }
            else
            {
                left.interactable = false;
            }

            panels[idx].GetComponent<PanelSort>().index = 0;
            panels[idx].SetActive(true);
            if (PlayerPrefs.HasKey(panels[idx].name + "Score"))
            {
                panels[idx].GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(InGameManager.instance.curSongName + "Score") + "%";
            }

            if (index < panels.Count - 1)
            {
                panels[idx + 1].GetComponent<PanelSort>().index = 1;
                panels[idx + 1].SetActive(true);
                right.interactable = true;
                if (PlayerPrefs.HasKey(panels[idx + 1].name + "Score"))
                {
                    panels[idx + 1].GetComponentInChildren<Text>().text = PlayerPrefs.GetInt(panels[idx + 1] + "Score") + "%";
                }
            }
            else
            {
                right.interactable = false;
            }
            SoundManager.instance.MenuHighlightPlay(hightTimes[idx]);
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
        InGameManager.instance.state = GameState.InGame;
    }
}