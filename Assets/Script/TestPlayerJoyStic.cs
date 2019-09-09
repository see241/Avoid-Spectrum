using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TestPlayerJoyStic : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        speed = 3;
    }

    // Update is called once per frame
    private void Update()
    {
        MoveToJoyStick();
    }

    private void MoveToJoyStick()
    {
        transform.Translate(Joystick.instance.moveVec * Time.deltaTime * speed * Player.instance.GetMoveSensitive());
    }

    public void AdaptTimeScale()
    {
        speed = 3 / Time.timeScale;
    }
}