using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashUI : MonoBehaviour, IIngredientContainer, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private BottleUI bottleUI;
    private Bumpable bumpable;

    private void Awake()
    {
        bumpable = GetComponent<Bumpable>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
    }

    public bool SetIngredient(IngredientSO ingredient)
    {
        ingredient.Use();
        bumpable.BumpDown();
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bottleUI.HasIngredient && !IngredientUI.Holding) return;
        bumpable.BumpUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bumpable.BumpDown();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        bottleUI.ClearAll();
    }
}
