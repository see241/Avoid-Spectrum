using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    private List<GameObject> list = new List<GameObject>();
    public List<GameObject> listed = new List<GameObject>();

    public int listCount;
    public GameObject go;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        InitPool();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void InitPool()
    {
        for (int i = 0; i < listCount; i++)
        {
            PushObject(Instantiate(go, transform));
        }
    }

    public GameObject PopObject()
    {
        if (list.Count > 0)
        {
            GameObject poolingObejct = list[0];
            list.RemoveAt(0);
            poolingObejct.SetActive(true);
            return poolingObejct;
        }
        else
        {
            return Instantiate(go, transform);
        }
    }

    public void PushObject(GameObject _go)
    {
        _go.SetActive(false);
        list.Add(_go);
        listed.Remove(_go);
    }

    public void CleanPool()
    {
        while (listed.Count > 0)
        {
            PushObject(listed[0]);
        }
    }
}