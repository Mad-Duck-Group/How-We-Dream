using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TNRD;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IIngredientContainer, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite[] equipmentSprites;
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private CookStates equipmentType;
    [SerializeField] private SerializableInterface<IMinigame> minigame;
    //[SerializeField, ReadOnly, Expandable] 
    private IngredientSO ingredient;
    private IngredientUI ingredientUI;
    private SpriteRenderer spriteRenderer;
    private float originalScale;
    private Tween bumpTween;
    private Bumpable bumpable;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bumpable = GetComponent<Bumpable>();
        spriteRenderer.sprite = equipmentSprites[0];
        originalScale = transform.localScale.x;
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
            GlobalSoundManager.Instance.PlayUISFX("CookSuccess");
        }
        else
        {
            transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            Destroy(ingredient);
            UnsetIngredient();
            GlobalSoundManager.Instance.PlayUISFX("CookFail");
        }
    }

    public bool SetIngredient(IngredientSO ingredient)
    {
        if (this.ingredient) return false;
        if (ingredient.CookState != CookStates.Raw) return false;
        this.ingredient = ingredient;
        transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
        spriteRenderer.sprite = equipmentSprites[1];
        switch (minigame.Value)
        {
            case FantasyOven:
                GlobalSoundManager.Instance.PlayUISFX("AddToOven");
                break;
            case AdventureSkillet:
                GlobalSoundManager.Instance.PlayUISFX("AddToPan");
                break;
            case JoyBlender:
                GlobalSoundManager.Instance.PlayUISFX("AddToBlender");
                break;
            case WhimsyBoiler:
                GlobalSoundManager.Instance.PlayUISFX("AddToBoiler");
                break;
        }
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredient == null) return;
        ingredientUI = Instantiate(ingredientUIPrefab);
        ingredientUI.Initialize(ingredient, gameObject);
        ingredientUI.BeingDrag();
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
        bumpable.BumpDown();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ingredient && IngredientUI.Holding) return;
        if (!ingredient && !IngredientUI.Holding) return;
        bumpable.BumpUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ingredient && IngredientUI.Holding) return;
        if (!ingredient && !IngredientUI.Holding) return;
        bumpable.BumpDown();
    }
}
