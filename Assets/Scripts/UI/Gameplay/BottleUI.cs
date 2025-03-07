using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BottleUI : MonoBehaviour, IIngredientContainer, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private float beadSize = 0.5f;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private Vector2 randomOffset;
    [SerializeField] private Button submitButton;
    
    [SerializeField, ReadOnly, Expandable] private IngredientSO ingredient;
    
    private Stack<KeyValuePair<IngredientSO, IngredientUI>> ingredients = new();
    private IngredientUI ingredientUI;
    private Bumpable bumpable;
    public bool HasIngredient => ingredients.Count > 0;

    private void OnEnable()
    {
        RecipeManager.OnRecipeChanged += OnRecipeChange;
    }
    
    private void OnDisable()
    {
        RecipeManager.OnRecipeChanged -= OnRecipeChange;
    }

    private void OnRecipeChange(RecipeSO current, bool active)
    {
        submitButton.interactable = active;
    }

    private void Awake()
    {
        bumpable = GetComponent<Bumpable>();
    }

    private void Start()
    {
        submitButton.onClick.AddListener(Submit);
        submitButton.interactable = false;
    }

    public bool SetIngredient(IngredientSO ingredient)
    {
        var ingredientUI = Instantiate(ingredientUIPrefab, RandomDropPoint(), Quaternion.identity);
        ingredientUI.Initialize(ingredient, gameObject);
        ingredientUI.SetPhysics(true);
        ingredientUI.SetSize(size: beadSize);
        ingredients.Push(new KeyValuePair<IngredientSO, IngredientUI>(ingredient, ingredientUI));
        GlobalSoundManager.Instance.PlayUISFX("Jar");
        bumpable.BumpDown();
        return true;
    }
    
    private void ReturnToDropPoint()
    {
        ingredientUI.transform.position = RandomDropPoint();
        ingredientUI.SetPhysics(true);
        ingredientUI.SetSize(size: beadSize);
        bumpable.BumpDown();
    }
    
    private Vector3 RandomDropPoint()
    {
        var offset = new Vector3(Random.Range(-randomOffset.x, randomOffset.x), Random.Range(-randomOffset.y, randomOffset.y), 0);
        return dropPoint.position + offset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredients.Count == 0) return;
        ingredient = ingredients.Peek().Key;
        ingredientUI = ingredients.Peek().Value;
        ingredientUI.SetPhysics(false);
        ingredientUI.SetSize(true);
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
        if (ingredientUI == null) return;
        bool success = ingredientUI.EndDragCheck(eventData);
        if (!success)
        {
            ReturnToDropPoint();
            return;
        }
        var ui = ingredients.Pop().Value;
        Destroy(ui.gameObject);
        ingredient = null;
    }

    public void Submit()
    {
        RecipeManager.Instance.CheckRecipe(ingredients.Select(pair => pair.Key).ToList());
        ClearAll();
    }
    
    public void ClearAll()
    {
        foreach (var pair in ingredients)
        {
            pair.Key.Use();
            Destroy(pair.Value.gameObject);
        }
        ingredients.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IngredientUI.Holding) return;
        bumpable.BumpUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IngredientUI.Holding) return;
        bumpable.BumpDown();
    }
}
