using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableArea : MonoBehaviour, IPointerClickHandler
{
    public delegate void OnClick();
    public event OnClick OnClickEvent;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnClickEvent?.Invoke();
    }
}
