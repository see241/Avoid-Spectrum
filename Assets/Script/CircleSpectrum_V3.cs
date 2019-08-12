using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpectrum_V3 : MonoBehaviour
{
    public static CircleSpectrum_V3 instance;
    private Vector3 world_size;

    public float samplingrateCover;
    private List<GameObject> targetGrids = new List<GameObject>();
    private List<GameObject> spectrumPos = new List<GameObject>();
    public int spectrumCnt;

    private float[] preSpectrum;

    public float sens;
    public float rotateSpeed;
    private bool isReverseCooltime;

    public float reverseCooltime;
    private int sw = 1;

    [SerializeField]
    private LineRenderer linePrefab;

    public float bulletCoolTime;

    [Range(0, 1)]
    public float minLen;

    [Range(1, 25)]
    public float sensitive;

    [HideInInspector]
    public float applyBulletSpeed;

    public float bulletSpeed;
    public float defaultBulletSpeed;

    [Range(0, 1f)]
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
        preSpectrum = new float[spectrumCnt];
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
        float[] SpectrumData = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming);
        for (int i = 0; i < spectrumCnt; i++)
        {
            sens += SpectrumData[i];
            float targetPos = SpectrumData[i] * sensitive*samplingrateCover;

            Vector3 targetVec = targetGrids[i].transform.position + targetGrids[i].transform.up * minLen + targetGrids[i].transform.up * targetPos;
            spectrumPos[i].transform.position = Vector3.MoveTowards(spectrumPos[i].transform.position, targetVec, 0.07f);
            //spectrumPos[i].transform.position = targetVec;
            lines[i].SetPosition(0, targetGrids[i].transform.position);
            lines[i].SetPosition(1, spectrumPos[i].transform.position);
            if (InGameManager.instance.isMusicStarted)
            {
                if (SpectrumData[i] * samplingrateCover - preSpectrum[i] > diff / 2) coolTime[i] = false;
                if (SpectrumData[i] * samplingrateCover > diff && !coolTime[i])
                {
                    GameObject poppingBullet = PoolManager.instance.PopObject();
                    PoolManager.instance.listed.Add(poppingBullet);
                    poppingBullet.transform.position = lines[i].GetPosition(1);
                    poppingBullet.GetComponent<Bullet>().Init(lines[i].GetPosition(1) - lines[i].GetPosition(0));
                    poppingBullet.GetComponent<Bullet>().speed = SpectrumData[i] * samplingrateCover * bulletSpeed;
                    StartCoroutine(iStartCooltime(i));
                }

                preSpectrum[i] = SpectrumData[i] * samplingrateCover;
            }
        }
        if (InGameManager.instance.state == GameState.InGame)
        {
            if (sens > 2.5 && isReverseCooltime == false)
            {
                sw *= -1;
                StartCoroutine(StartCooltime());
                Debug.Log("reverse");
            }
            //rotateSpeed = Mathf.MoveTowards(rotateSpeed, sens * 45 * sw, 1);
            rotateSpeed = sens * 30 * sw;
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }

        applyBulletSpeed = sens / 2 * bulletSpeed;
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
        isReverseCooltime = true;
        yield return new WaitForSeconds(reverseCooltime);
        isReverseCooltime = false;
    }
}