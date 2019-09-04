using UnityEngine;

public class MainUI : MonoBehaviour
{
    public GameObject panel;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Thanks()
    {
        panel.SetActive(!panel.active);
    }
}