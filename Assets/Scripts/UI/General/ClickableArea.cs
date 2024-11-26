using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableArea : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool interactable = true;
    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            OnInteractChangeEvent?.Invoke(interactable);
        }
    }

    public delegate void OnClick();
    public event OnClick OnClickEvent;
    public delegate void OnDown();
    public event OnDown OnDownEvent;
    public delegate void OnUp();
    public event OnUp OnUpEvent;
    public delegate void OnInteractChange(bool interactable);
    public event OnInteractChange OnInteractChangeEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnClickEvent?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnUpEvent?.Invoke();
    }
    
    public void UnsubscribeAll()
    {
        var clickEvents = OnClickEvent?.GetInvocationList();
        if (clickEvents != null)
            foreach (var clickEvent in clickEvents)
            {
                OnClickEvent -= (OnClick) clickEvent;
            }
        var downEvents = OnDownEvent?.GetInvocationList();
        if (downEvents != null)
            foreach (var downEvent in downEvents)
            {
                OnDownEvent -= (OnDown) downEvent;
            }
        var upEvents = OnUpEvent?.GetInvocationList();
        if (upEvents != null)
            foreach (var upEvent in upEvents)
            {
                OnUpEvent -= (OnUp) upEvent;
            }
    }
}
