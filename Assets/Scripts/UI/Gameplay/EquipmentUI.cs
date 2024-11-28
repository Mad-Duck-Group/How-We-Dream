using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using Redcode.Extensions;
using TNRD;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IIngredientContainer, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite[] equipmentSprites;
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private CookStates equipmentType;
    [SerializeField] private int maxIngredients = 3;
    [SerializeField] private SerializableInterface<IMinigame> minigame;
    //[SerializeField, ReadOnly, Expandable] 
    private Stack<IngredientSO> ingredients = new();
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
            //transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            ingredients.ForEach(x => x.CookState = equipmentType);
            GlobalSoundManager.Instance.PlayUISFX("CookSuccess");
        }
        else
        {
            //transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            ingredients.ForEach(Destroy);
            UnsetIngredient();
            GlobalSoundManager.Instance.PlayUISFX("CookFail");
        }
    }

    public bool SetIngredient(IngredientSO ingredient)
    {
        if (ingredients.Count >= maxIngredients) return false;
        if (ingredients.Any(x => x.CookState != CookStates.Raw)) return false;
        if (ingredient.CookState != CookStates.Raw)
        {
            bumpable.BumpDown();
            return false;
        }
        ingredients.Push(ingredient);
        //transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
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
        if (ingredients.Count == 0) return;
        ingredientUI = Instantiate(ingredientUIPrefab);
        var peek = ingredients.Peek();
        ingredientUI.Initialize(peek, gameObject);
        ingredientUI.BeingDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredients.Count == 0) return;
        ingredientUI.Drag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ingredients.Count == 0) return;
        bool success = ingredientUI.EndDragCheck(eventData);
        Destroy(ingredientUI.gameObject);
        if (success)
        {
            ingredients.Pop();
            if (ingredients.Count == 0)
            {
                UnsetIngredient();
            }
        }
        bumpable.BumpDown();
    }
    
    private void UnsetIngredient()
    {
        spriteRenderer.sprite = equipmentSprites[0];
        ingredients.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredients.Count == 0) return;
        if (ingredients.All(x => x.CookState == equipmentType)) return;
        if (minigame.Value != null)
        {
            minigame.Value.StartMinigame();
        }
        else
        {
            //transform.DOScale(0.2f, 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);
            ingredients.ForEach(x => x.CookState = equipmentType);
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ingredients.Count >= maxIngredients && IngredientUI.Holding) return;
        if (ingredients.Count == 0 && !IngredientUI.Holding) return;
        bumpable.BumpUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ingredients.Count >= maxIngredients && IngredientUI.Holding) return;
        if (ingredients.Count == 0 && !IngredientUI.Holding) return;
        bumpable.BumpDown();
    }
}
