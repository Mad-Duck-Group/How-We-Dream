using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCapacityUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text ingredientCapacityText;

    private IngredientData ingredientData;

    private void OnEnable()
    {
        InventoryManager.OnIngredientAmountChanged += OnIngredientAmountChanged;
        if (ingredientData != null)
            GetIngredientData();
        UpdateIngredientCapacity();
    }

    private void OnDisable()
    {
        InventoryManager.OnIngredientAmountChanged -= OnIngredientAmountChanged;
    }

    private void OnIngredientAmountChanged(IngredientTypes ingredientType, int amount)
    {
        if (ingredientData.Ingredient.IngredientType != ingredientType) return;
        GetIngredientData(ingredientType);
        UpdateIngredientCapacity();
    }

    public void Initialize(IngredientTypes ingredientType)
    {
        GetIngredientData(ingredientType);
        icon.sprite = ingredientData.Ingredient.GetSprite(CookStates.Raw);
        UpdateIngredientCapacity();
    }
    
    private void UpdateIngredientCapacity()
    {
        if (ingredientData == null) return;
        ingredientCapacityText.text = $"{ingredientData.Amount}/{ingredientData.MaxAmount}";
    }

    private void GetIngredientData(IngredientTypes type = default)
    {
        ingredientData = InventoryManager.Instance.GetIngredientData(ingredientData == null ? type : ingredientData.Ingredient.IngredientType);
    }
}