using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CallBack();

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public UnityEngine.UI.Text colorNotieText;
    public UnityEngine.UI.Text shapeNotieText;
    public UnityEngine.UI.Text buyFailNoticeText;
    public UnityEngine.UI.Image buyFailNoticeImage;
    public UnityEngine.UI.Image shapeNotieImage;
    public Dictionary<int, Sprite> shapes;
    private bool duringChanging;
    public Color latestColor;

    private void Awake()
    {
        instance = this;
        shapes = new Dictionary<int, Sprite>();
        for (int i = 0; i < 10; i++)
        {
            ShopItem item = transform.GetChild(i).GetComponent<ShopItem>();
            if (item.itemtype == ItemType.Shape)
                shapes.Add(item.itemCode, item.image.sprite);
        }
    }

    public void Interaction(ShopItem item)
    {
        if (item.isLock)
        {
            BuyItem(item);
        }
        else if (!item.isLock)
        {
            switch (item.itemtype)
            {
                case ItemType.Color:
                    ChangeColorSkin(item);
                    break;

                case ItemType.Shape:
                    ChangeShapeSkin(item);
                    break;
            }
            Vibration.Vibrate(1);
        }
    }

    private void BuyItem(ShopItem item)
    {
        if (InGameManager.instance.Gold >= item.cost)
        {
            item.SuccessToBuy();
            InGameManager.instance.Gold -= item.cost;
        }
        else
        {
            item.FailToBuy();
            StartCoroutine(BuyFailNotice());
        }
    }

    private void ChangeColorSkin(ShopItem item)
    {
        float r = item.image.color.r;
        float g = item.image.color.g;
        float b = item.image.color.b;
        Player.instance.SetPlayerColor(item.image.color);
        PlayerPrefs.SetFloat("PlayerColorR", r);
        PlayerPrefs.SetFloat("PlayerColorG", g);
        PlayerPrefs.SetFloat("PlayerColorB", b);
        PlayerPrefs.Save();
        latestColor = item.image.color;
        StartCoroutine(ColorChaneNotice(item.image.color));
    }

    private void ChangeShapeSkin(ShopItem item)
    {
        Player.instance.SetPlayerShape(item);
        StartCoroutine(ShapeChangeNotice(item));
    }

    private IEnumerator ColorChaneNotice(Color c)
    {
        duringChanging = true;
        Color color = new Color(c.r, c.g, c.b, 1);
        colorNotieText.color = color;
        float turm = 0.1f * InGameManager.instance.applyTimeScale;
        yield return new WaitForSeconds(turm);
        while (color.a > 0)
        {
            color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime / InGameManager.instance.applyTimeScale * 5f);
            colorNotieText.color = color;
            yield return null;
        }
        duringChanging = false;
    }

    private IEnumerator ShapeChangeNotice(ShopItem item)
    {
        duringChanging = true;
        Color color = new Color(latestColor.r, latestColor.g, latestColor.b, 1);
        shapeNotieImage.sprite = item.image.sprite;
        shapeNotieText.color = color;
        shapeNotieImage.color = color;
        float turm = 0.1f * InGameManager.instance.applyTimeScale;
        yield return new WaitForSeconds(turm);
        while (color.a > 0)
        {
            color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime / InGameManager.instance.applyTimeScale * 5f);
            shapeNotieText.color = color;
            shapeNotieImage.color = color;
            yield return null;
        }
        duringChanging = false;
    }

    private IEnumerator BuyFailNotice()
    {
        Color color = new Color(1, 1, 0.5f);
        buyFailNoticeText.color = color;
        buyFailNoticeImage.color = color;
        float turm = 0.1f * InGameManager.instance.applyTimeScale;
        yield return new WaitForSeconds(turm);
        while (color.a > 0)
        {
            color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime / InGameManager.instance.applyTimeScale * 2.5f);
            buyFailNoticeText.color = color;
            buyFailNoticeImage.color = color;
            yield return null;
        }
    }
}