using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpectrum_V3 : MonoBehaviour
{
    public static CircleSpectrum_V3 instance;
    private Vector3 world_size;

    private List<GameObject> targetGrids = new List<GameObject>();
    private List<GameObject> spectrumPos = new List<GameObject>();
    public int spectrumCnt;

    private float sens;
    public float rotateSpeed;
    private bool rotateCooltime;
    private int sw = 1;

    [SerializeField]
    private LineRenderer linePrefab;

    public float bulletCoolTime;

    [Range(0, 1)]
    public float minLen;

    [Range(1, 25)]
    public float sensitive;

    [Range(0, 0.2f)]
    public float diff;

    public float radius;

    private List<LineRenderer> lines = new List<LineRenderer>();
    private List<bool> coolTime = new List<bool>();

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        linePrefab.positionCount = 2;
        for (int i = 0; i < spectrumCnt; i++)
        {
            targetGrids.Add(Instantiate(linePrefab, transform).gameObject);
            targetGrids[i].transform.Rotate(0, 0, 360 / (float)spectrumCnt * i);
            targetGrids[i].transform.Translate(Vector3.down * radius / 2);
            spectrumPos.Add(targetGrids[i].transform.GetChild(0).gameObject);
            lines.Add(targetGrids[i].GetComponent<LineRenderer>());
            lines[i].SetPosition(0, targetGrids[i].transform.position);
            coolTime.Add(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        float[] SpectrumData = AudioListener.GetSpectrumData(2048, 0, FFTWindow.Hamming);
        for (int i = 0; i < spectrumCnt; i++)
        {
            sens += SpectrumData[i];
            float targetPos = SpectrumData[i] * sensitive;

            Vector3 targetVec = targetGrids[i].transform.position + targetGrids[i].transform.up * minLen + targetGrids[i].transform.up * targetPos;
            spectrumPos[i].transform.position = Vector3.MoveTowards(spectrumPos[i].transform.position, targetVec, 0.1f);
            lines[i].SetPosition(0, targetGrids[i].transform.position);
            lines[i].SetPosition(1, spectrumPos[i].transform.position);
            if (InGameManager.instance.isMusicStarted)
            {
                if (SpectrumData[i] > 0.05 && !coolTime[i])
                {
                    GameObject poppingBullet = PoolManager.instance.PopObject();
                    PoolManager.instance.listed.Add(poppingBullet);
                    poppingBullet.transform.position = lines[i].GetPosition(1);
                    poppingBullet.GetComponent<Bullet>().Init(lines[i].GetPosition(1) - lines[i].GetPosition(0));
                    StartCoroutine(iStartCooltime(i));
                }
            }
        }
        if (InGameManager.instance.state == GameState.InGame)
        {
            if (sens > 2.5 && rotateCooltime == false)
            {
                sw *= -1;
                StartCoroutine(StartCooltime());
                Debug.Log("reverse");
            }
            rotateSpeed = Mathf.MoveTowards(rotateSpeed, sens * 45 * sw, 25);
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
        sens = 0;
    }

    private IEnumerator iStartCooltime(int t)
    {
        coolTime[t] = true;
        yield return new WaitForSeconds(bulletCoolTime);
        coolTime[t] = false;
    }

    private IEnumerator StartCooltime()
    {
        rotateCooltime = true;
        yield return new WaitForSeconds(0.1f);
        rotateCooltime = false;
    }
}