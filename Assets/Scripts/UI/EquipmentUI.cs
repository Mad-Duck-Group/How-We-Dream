using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IIngredientContainer, IPointerClickHandler
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private CookStates equipmentType;
    //[SerializeField, ReadOnly, Expandable] 
    private IngredientSO ingredient;
    private IngredientUI ingredientUI;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public bool SetIngredient(IngredientSO ingredient)
    {
        if (this.ingredient) return false;
        this.ingredient = ingredient;
        spriteRenderer.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredient == null) return;
        ingredientUI = Instantiate(ingredientUIPrefab);
        ingredientUI.Initialize(ingredient, gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredient == null) return;
        ingredientUI.Drag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ingredientUI == null) return;
        bool success = ingredientUI.EndDragCheck(eventData);
        Destroy(ingredientUI.gameObject);
        if (success) ingredient = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredient == null) return;
        if (ingredient.CookState == equipmentType) return;
        ingredient.CookState = equipmentType;
        transform.DOScale(1.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
    }
}
