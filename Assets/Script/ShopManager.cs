using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    public static ShopManager instance;
    public UnityEngine.UI.Text text;
    bool duringChanging;

    void Awake()
    {
        instance = this;
    }
    public void Interaction(ShopItem item)
    {
        if (item.isLock)
        {
            BuyItem(item);
        }
        if (!item.isLock)
        {
            ChangeSkin(item);
        }
    }


    void BuyItem(ShopItem item)
    {
        if (InGameManager.instance.Gold >= item.cost)
        {
            item.SuccessToBuy();
            InGameManager.instance.Gold -= item.cost;
        }
        else
        {
            item.FailToBuy();
        }
    }
    void ChangeSkin(ShopItem item)
    {
        float r = item.image.color.r;
        float g = item.image.color.g;
        float b = item.image.color.b;
        Player.instance.SetPlayerColor(item.image.color);
        PlayerPrefs.SetFloat("PlayerColorR", r);
        PlayerPrefs.SetFloat("PlayerColorG", g);
        PlayerPrefs.SetFloat("PlayerColorB", b);
        PlayerPrefs.Save();
        StartCoroutine(ColorChaneNotice(item.image.color));
    }
    IEnumerator ColorChaneNotice(Color c)
    {
        duringChanging = true;
           Color color = new Color(c.r, c.g, c.b, 1);
        text.color = color;
        yield return new WaitForSeconds(0.1f);
        while (color.a > 0)
        {
            text.color = color;
            color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime*3.5f);
            yield return null;
        }
        duringChanging = false;
    }
}
