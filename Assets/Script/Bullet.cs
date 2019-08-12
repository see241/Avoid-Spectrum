using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 dir;

    public float speed;
    Transform tf;
    private float _speed;
    const float powedDistance = 20.25f;
    // Use this for initialization
    private void Start()
    {
        tf =transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!InGameManager.instance.isPause)
        {
            speed = CircleSpectrum_V3.instance.applyBulletSpeed + 0.1f;
            tf.Translate(dir * Time.deltaTime * speed);
        }
        if (GetDistance(tf.position, Vector2.zero) > powedDistance)
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
        return (a.x - b.x)* (a.x - b.x) + (a.y - b.y)* (a.y - b.y);
    }
}