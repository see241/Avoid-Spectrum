using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject joystick;
    public GameObject joystickBackGround;
    public ParticleSystem dieEffect;

    private GameObject curJoystick;
    private GameObject curJoystickBackGround;

    public Vector2 moveVec;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        curJoystick = Instantiate(joystick);
        curJoystickBackGround = Instantiate(joystickBackGround);
    }

    // Update is called once per frame
    private void Update()
    {
        //CreateJoystic();
    }

    public void PlayerDie(GameObject player)
    {
        ParticleSystem ptc = Instantiate(dieEffect);
        ptc.transform.position = player.transform.position;
        Destroy(ptc, ptc.duration + ptc.startLifetime);
    }

    private void CreateJoystic()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                curJoystick.SetActive(true);
                curJoystick.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
                curJoystickBackGround.SetActive(true);
                curJoystickBackGround.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
            }
            if (touch.phase == TouchPhase.Moved)
            {
                curJoystick.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
                if (Vector2.Distance(curJoystick.transform.position, curJoystickBackGround.transform.position) > 1.5)
                {
                    curJoystick.transform.position = curJoystickBackGround.transform.position + (curJoystick.transform.position - curJoystickBackGround.transform.position).normalized * 1.5f;
                }
                moveVec = (curJoystick.transform.position - curJoystickBackGround.transform.position).normalized;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                curJoystickBackGround.SetActive(false);
                curJoystick.SetActive(false);
                moveVec = Vector2.zero;
            }
        }
    }
}