using System.Collections.Generic;
using UnityEngine;

public class CircleSpectrum_V2 : MonoBehaviour
{
    private Vector3 world_size;

    private List<GameObject> targetGrids = new List<GameObject>();
    private List<Vector3> spectrumPos = new List<Vector3>();
    public int spectrumCnt;

    [SerializeField]
    private LineRenderer linePrefab;

    [Range(0, 1)]
    public float minLen;

    [Range(1, 25)]
    public float sensitive;

    public int radius;

    public GameObject emptyObject;

    private List<LineRenderer> lines = new List<LineRenderer>();

    // Use this for initialization
    private void Start()
    {
        linePrefab.positionCount = 2;
        for (int i = 0; i < spectrumCnt; i++)
        {
            targetGrids.Add(Instantiate(emptyObject, transform));
            targetGrids[i].transform.Rotate(0, 0, 360 / (float)spectrumCnt * i);
            targetGrids[i].transform.Translate(Vector3.down * radius / 2);
            spectrumPos.Add(targetGrids[i].transform.position);
            lines.Add(Instantiate(linePrefab, transform.GetChild(i)));
            lines[i].SetPosition(0, targetGrids[i].transform.position);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        float[] SpectrumData = AudioListener.GetSpectrumData(2048, 0, FFTWindow.Hamming);
        for (int i = 0; i < spectrumCnt; i++)
        {
            float targetPos = SpectrumData[i] * sensitive;

            Vector3 targetVec = spectrumPos[i] + targetGrids[i].transform.up * minLen + targetGrids[i].transform.up * targetPos;
            targetGrids[i].transform.position = Vector3.MoveTowards(targetGrids[i].transform.position, targetVec, 0.1f);
            lines[i].SetPosition(1, targetGrids[i].transform.position);
        }
    }
}