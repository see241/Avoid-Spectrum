using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 dir;

    public float speed;
    private float _speed;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!InGameManager.instance.isPause)
            transform.Translate(dir * Time.deltaTime * speed);
        else
        {
            Debug.Log("now pausing");
        }
        if (Vector2.Distance(transform.position, Vector2.zero) > 4.5)
        {
            PoolManager.instance.PushObject(gameObject);
        }
    }

    public void Init(Vector2 spectrum_dir)
    {
        dir = spectrum_dir.normalized;
    }

    private float GetDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2);
    }
}