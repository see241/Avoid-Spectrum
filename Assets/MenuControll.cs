using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct KeyButton
{
    public KeyCode key;
    public Button button;
}

public class MenuControll : MonoBehaviour
{
    public KeyButton keyButton;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(keyButton.key))
        {
            if (keyButton.button != null)
                keyButton.button.onClick.Invoke();
        }
    }
}