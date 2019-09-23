using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider progressBar;
    [SerializeField] private UnityEngine.UI.Text line1Text;
    [SerializeField] private UnityEngine.UI.Text line2Text;
    private const string originText1 = "Loading...";
    private const string originText2 = "AvoidSpectrum";
    private string applyStr;
    private string applyStrWithTag;

    private bool isFirstLineEnd;

    private bool twinkFlag;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SceneLoad());
        StartCoroutine(TextTwink());
        StartCoroutine(TextApply());
    }

    // Update is called once per frame
    private IEnumerator SceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        var operation = SceneManager.LoadSceneAsync("Menu");
        operation.allowSceneActivation = false;
        while (true)
        {
            progressBar.value = Mathf.MoveTowards(progressBar.value, operation.progress, 0.2f * Time.deltaTime);
            if (operation.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }
        while (progressBar.value < 1)
        {
            progressBar.value += Random.Range(0, 0.1f) * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        operation.allowSceneActivation = true;
    }

    private IEnumerator TextApply()
    {
        yield return null;
        for (int i = 0; i < originText1.Length; i++)
        {
            applyStr += originText1[i];
            applyStrWithTag = applyStr + "<color=#06D890><b>_</b></color>";
            if (twinkFlag)
                line1Text.text = applyStrWithTag;
            else
                line1Text.text = applyStr;
            yield return new WaitForSeconds(Random.Range(0.07f, 0.085f));
        }
        line1Text.text = applyStr;
        applyStr = "";
        applyStrWithTag = "";
        isFirstLineEnd = true;

        for (int i = 0; i < originText2.Length; i++)
        {
            applyStr += originText2[i];
            applyStrWithTag = applyStr + "<color=#06D890><b>_</b></color>";
            if (twinkFlag)
                line2Text.text = applyStrWithTag;
            else
                line2Text.text = applyStr;
            yield return new WaitForSeconds(Random.Range(0.07f, 0.085f));
        }
    }

    private IEnumerator TextTwink()
    {
        while (!isFirstLineEnd)
        {
            twinkFlag = !twinkFlag;
            if (twinkFlag)
                line1Text.text = applyStrWithTag;
            else
                line1Text.text = applyStr;
            yield return new WaitForSeconds(0.5f);
        }
        while (true)
        {
            twinkFlag = !twinkFlag;
            if (twinkFlag)
                line2Text.text = applyStrWithTag;
            else
                line2Text.text = applyStr;
            yield return new WaitForSeconds(0.5f);
        }
    }
}