using UnityEngine;
using UnityEngine.EventSystems;

public class SliderGrid : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        InGameManager.instance.SetPerfactSliderValue();
    }
}