using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private Vector3 world_size;

    private List<GameObject> targetGrids = new List<GameObject>();
    private List<Vector3> spectrumPos = new List<Vector3>();
    private int spectrumCnt;

    [SerializeField]
    private LineRenderer linePrefab;

    [Range(0, 1)]
    public float minLen;

    [Range(1, 25)]
    public float sensitive;

    private List<LineRenderer> lines = new List<LineRenderer>();

    // Use this for initialization
    private void Start()
    {
        spectrumCnt = transform.GetChildCount();
        linePrefab.positionCount = 2;
        Vector2 sprite_size = GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        world_size = local_sprite_size;
        world_size.x *= transform.lossyScale.x;
        world_size.y *= transform.lossyScale.y;
        for (int i = 0; i < spectrumCnt; i++)
        {
            targetGrids.Add(transform.GetChild(i).gameObject);
            targetGrids[i].transform.Rotate(0, 0, 360 / (float)spectrumCnt * i);
            targetGrids[i].transform.Translate(Vector3.up * 3.92f / 2);
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