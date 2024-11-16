using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableArea : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public delegate void OnClick();
    public event OnClick OnClickEvent;
    public delegate void OnDown();
    public event OnDown OnDownEvent;
    public delegate void OnUp();
    public event OnUp OnUpEvent;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnClickEvent?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnUpEvent?.Invoke();
    }
}
