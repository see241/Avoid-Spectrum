using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Slider progressBar;
    public Slider _progressBar;
    public Text score;
    public Text _title;
    public Text title;

    public TextMesh text;

    public AudioSource curSong;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(StartScene());
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetUIInfo()
    {
        progressBar.value = curSong.time / curSong.clip.length;
        _progressBar.value = progressBar.value;
        score.text = ((int)(_progressBar.value * 100)) + "%";
    }

    public void SoundChage(string a)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<AudioSource>().Stop();
        }
        curSong = transform.Find(a).GetComponent<AudioSource>();
        curSong.Play();
        _title.text = a;
        title.text = a;
    }

    public void AllSoundStop()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<AudioSource>().Stop();
        }
    }

    public void SoundPause()
    {
        if (curSong != null)
            curSong.Pause();
    }

    public void SoundResume()
    {
        curSong.UnPause();
    }

    private IEnumerator StartScene()
    {
        for (int i = 0; i < 3; i++)
        {
            TextMesh _text = Instantiate(text);
            _text.text = (3 - i).ToString();
            yield return new WaitForSeconds(1f);
            Destroy(_text.gameObject);
        }
        SoundChage(InGameManager.instance.curSongName);
    }
}