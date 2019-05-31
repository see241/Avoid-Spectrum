using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Vector3 world_size;

    private List<GameObject> spectrumLine = new List<GameObject>();
    private List<Vector2> spectrumPos = new List<Vector2>();
    private int _mSpectrumCnt;

    [SerializeField]
    private LineRenderer _mSpectrumLine;

    // Use this for initialization
    private void Start()
    {
        _mSpectrumCnt = transform.GetChildCount();
        _mSpectrumLine.positionCount = _mSpectrumCnt + 1;
        Vector2 sprite_size = GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        world_size = local_sprite_size;
        world_size.x *= transform.lossyScale.x;
        world_size.y *= transform.lossyScale.y;
        for (int i = 0; i < _mSpectrumCnt; i++)
        {
            spectrumLine.Add(transform.GetChild(i).gameObject);
            spectrumLine[i].transform.Rotate(0, 0, 360 / _mSpectrumCnt * i);
            spectrumLine[i].transform.Translate(Vector3.up * world_size.x / 2);
            spectrumPos.Add(spectrumLine[i].transform.position);
            Debug.Log(spectrumLine[i].transform.position);
        }
        //_mSpectrumLine.SetColors(Color.red, new Color(1, 0.5f, 0), Color.yellow, Color.green, Color.blue, new Color(0, 0, 0.5f), new Color(0.545f, 0, 1));
        _mSpectrumLine.SetColors(Color.red, Color.blue);
    }

    // Update is called once per frame
    private void Update()
    {
        float[] SpectrumData = AudioListener.GetSpectrumData(2048, 0, FFTWindow.Hamming);
        for (int i = 0; i < _mSpectrumCnt; i++)
        {
            float targetPos = SpectrumData[i] * 25;

            Vector2 targetVec = spectrumPos[i] + (Vector2)spectrumLine[i].transform.up * 0.2f + (Vector2)spectrumLine[i].transform.up * targetPos;
            spectrumLine[i].transform.position = Vector2.MoveTowards(spectrumLine[i].transform.position, targetVec, 0.1f);
            _mSpectrumLine.SetPosition(i, spectrumLine[i].transform.position);
        }
        _mSpectrumLine.SetPosition(_mSpectrumCnt, spectrumLine[0].transform.position);
    }
}