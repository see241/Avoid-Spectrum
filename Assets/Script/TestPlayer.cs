using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // Use this for initialization
    private Vector2 beginPos;

    private Vector2 lastPos;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        MoveToTouch();
    }

    private void MoveToTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                beginPos = Camera.main.ScreenToWorldPoint(touch.position);
            }
            if (touch.phase == TouchPhase.Moved)
            {
                if (!InGameManager.instance.isPause)
                {
                    transform.position = lastPos + ((Vector2)Camera.main.ScreenToWorldPoint(touch.position) - beginPos) * Player.instance.GetMoveSensitive();
                    if (Vector2.Distance(transform.position, Vector2.zero) > 4.25f)
                    {
                        transform.position = transform.position.normalized * 4.24f;
                    }
                }
            }
            if (touch.phase == TouchPhase.Ended)
            {
                lastPos = transform.position;
            }
        }
    }
}