using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    public static Joystick instance;
    public GameObject joystick;
    public GameObject joystickBackGround;
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
        if (InGameManager.instance.controlType == ControlType.Joystick)
            ControlJoystic();
    }

    private void ControlJoystic()
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
                if (Vector2.Distance(curJoystick.transform.position, curJoystickBackGround.transform.position) > 1)
                {
                    curJoystick.transform.position = curJoystickBackGround.transform.position + (curJoystick.transform.position - curJoystickBackGround.transform.position).normalized * 1f;
                }
                moveVec = (curJoystick.transform.position - curJoystickBackGround.transform.position);
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