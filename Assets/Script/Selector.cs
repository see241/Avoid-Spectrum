using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public static Selector instance;

    public KeyButton key;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(key.key))
        {
            if (key.button != null)
            {
                key.button.onClick.Invoke();
            }
        }
    }
}