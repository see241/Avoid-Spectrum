using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumReact : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        float t = 0;

        float[] SpectrumData = AudioListener.GetSpectrumData(64, 0, FFTWindow.Rectangular);
        for (int i = 0; i < SpectrumData.Length; i++)
        {
            t += SpectrumData[i];
        }
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * t, 0.02f);
    }
}