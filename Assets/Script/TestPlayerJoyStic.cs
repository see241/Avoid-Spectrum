using UnityEngine;

public class TestPlayerJoyStic : MonoBehaviour
{
    private float speed;

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
}