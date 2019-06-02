using System.Collections.Generic;
using UnityEngine;

public class ZigZagSpectrum : MonoBehaviour
{
    private Vector3 world_size;

    private List<GameObject> targetGrids = new List<GameObject>();
    private List<Vector3> spectrumPos = new List<Vector3>();
    public int spectrumCnt;

    private LineRenderer linePrefab;

    [SerializeField]
    private LineRenderer line;

    [Range(0, 1)]
    public float minLen;

    [Range(1, 25)]
    public float sensitive;

    public float intarval;

    public GameObject emptyObject;

    // Use this for initialization
    private void Start()
    {
        line.positionCount = spectrumCnt;
        int index = -spectrumCnt / 2;
        line = Instantiate(line, transform);
        for (int i = 0; i < spectrumCnt; i++)
        {
            targetGrids.Add(Instantiate(emptyObject, transform));
            targetGrids[i].transform.Translate(transform.position - Vector3.right * intarval * index);
            if (index % 2 == 0) targetGrids[i].transform.Rotate(0, 0, 180);
            spectrumPos.Add(targetGrids[i].transform.position);
            line.SetPosition(i, targetGrids[i].transform.position);
            index++;
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
            line.SetPosition(i, targetGrids[i].transform.position);
        }
    }
}