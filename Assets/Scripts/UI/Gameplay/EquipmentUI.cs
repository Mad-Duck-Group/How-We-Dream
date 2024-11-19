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
    [SerializeField] private Sprite[] equipmentSprites;
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
        spriteRenderer.sprite = equipmentSprites[0];
    }

    private void OnEnable()
    {
        if (minigame.Value == null) return;
        LevelManager.OnLevelComplete += OnLevelComplete;
        minigame.Value.OnMinigameEnd += OnMinigameEnd;
    }
    
    private void OnDisable()
    {
        if (minigame.Value == null) return;
        LevelManager.OnLevelComplete -= OnLevelComplete;
        minigame.Value.OnMinigameEnd -= OnMinigameEnd;
    }
    
    private void OnLevelComplete()
    {
        minigame.Value.Halt();
    }
    
    private void OnMinigameEnd(bool success)
    {
        if (success)
        {
            transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            ingredient.CookState = equipmentType;
        }
        else
        {
            transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            Destroy(ingredient);
            UnsetIngredient();
        }
    }

    public bool SetIngredient(IngredientSO ingredient)
    {
        if (this.ingredient) return false;
        if (ingredient.CookState != CookStates.Raw) return false;
        this.ingredient = ingredient;
        transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
        spriteRenderer.sprite = equipmentSprites[1];
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
        if (success) UnsetIngredient();
    }
    
    private void UnsetIngredient()
    {
        spriteRenderer.sprite = equipmentSprites[0];
        ingredient = null;
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
            transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            ingredient.CookState = equipmentType;
        }
        
    }
}
