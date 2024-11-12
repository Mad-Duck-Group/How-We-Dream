using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TNRD;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IIngredientContainer, IPointerClickHandler
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private CookStates equipmentType;
    [SerializeField] private SerializableInterface<IMinigame> minigame;
    //[SerializeField, ReadOnly, Expandable] 
    private IngredientSO ingredient;
    private IngredientUI ingredientUI;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (minigame.Value == null) return;
        minigame.Value.OnMinigameEnd += OnMinigameEnd;
    }
    
    private void OnDisable()
    {
        if (minigame.Value == null) return;
        minigame.Value.OnMinigameEnd -= OnMinigameEnd;
    }

    private void OnMinigameEnd(bool success)
    {
        if (success)
        {
            spriteRenderer.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
            ingredient.CookState = equipmentType;
        }
        else
        {
            spriteRenderer.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
            Destroy(ingredient);
            ingredient = null;
        }
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
        if (minigame.Value != null)
        {
            minigame.Value.StartMinigame();
        }
        else
        {
            spriteRenderer.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
            ingredient.CookState = equipmentType;
        }
        
    }
}
