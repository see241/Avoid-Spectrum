using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Color, Shape
}

public class ShopItem : MonoBehaviour
{
    public ItemType itemtype;
    public int cost;
    public bool isLock;
    public Button button;
    public Image image;
    public int itemCode;

    private void Start()
    {
        if (PlayerPrefs.GetInt("ItemCode" + itemCode, 0) < 1)
        {
            isLock = true;
        }
        if (PlayerPrefs.GetInt("ItemCode" + itemCode, 0) >= 1)
        {
            isLock = false;
            button.image.color = new Color(0.5f, 1, 0f, 1);
        }
    }

    public void FailToBuy()
    {
        StartCoroutine(FailWarning());
    }

    public void SuccessToBuy()
    {
        isLock = false;
        button.image.color = new Color(0.5f, 1, 0f, 1);
        PlayerPrefs.SetInt("ItemCode" + itemCode, 1);
    }

    private IEnumerator FailWarning()
    {
        for (int i = 0; i < 2; i++)
        {
            Vibration.Vibrate(10);
            button.image.color = new Color(0, 0, 0);
            yield return new WaitForSeconds(0.05f);
            button.image.color = new Color(1, 0, 0.5f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}