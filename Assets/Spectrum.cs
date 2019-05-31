using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spectrum : MonoBehaviour
{
    public int CreateStickNum = 0;          //생성될 막대 개수
    public float interval = 0;              //생성될 막대 간격
    public List<GameObject> Sticks;         //GameObject 막대 생성

    public GameObject Yellow = null;        // 노란색 막대 선언

    private void Start()
    {
        for (int i = 0; i < CreateStickNum; i++)
        {
            GameObject obj = (GameObject)Instantiate(Yellow);                   //
            obj.transform.parent = GameObject.Find("Spectrum").transform;       //  노란색 막대를 interval 만큼의 간격으로
            obj.transform.localScale = Vector3.one;                             //  CreateStickNum개수만큼 생성
            obj.transform.localPosition = new Vector2(0 + interval * i, 0);     //

            Sticks.Add(obj);                                                    // 생성된 막대를 list에 추가
        }
    }

    private void Update()
    {
        float[] SpectrumData = AudioListener.GetSpectrumData(2048, 0, FFTWindow.Hamming);          // 스펙트럼데이터 배열에 오디오가 듣고있는 스펙트럼데이터를 대입
        for (int i = 0; i < Sticks.Count; i++)
        {
            Vector2 FirstScale = Sticks[i].transform.localScale;                                    // 처음 막대기 스케일을 변수로 생성
            FirstScale.y = 0.02f + SpectrumData[i] * 30;                                            // 막대기 y를 스펙트럼데이터에 맞게 늘림
            Sticks[i].transform.localScale = Vector2.MoveTowards(Sticks[i].transform.localScale, FirstScale, 0.1f);     // 스펙트럼데이터에 맞게 늘어난 스케일을 처음스케일로 0.1의 속도만큼 바꿈
        }
    }
}