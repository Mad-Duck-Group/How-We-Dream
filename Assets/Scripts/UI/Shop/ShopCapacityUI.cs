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

    private void OnIngredientAmountChanged(IngredientTypes ingredient)
    {
        if (ingredientData.Ingredient.IngredientType != ingredient) return;
        UpdateIngredientCapacity();
    }

    public void Initialize(IngredientTypes ingredient)
    {
        InventoryManager.onIngredientAmountChanged += (type, _) => OnIngredientAmountChanged(type);
        var data = InventoryManager.Instance.GetIngredientData(ingredient);
        ingredientData = data;
        //icon.sprite = ingredientData.Ingredient.GetSprite(CookStates.Raw);
        UpdateIngredientCapacity();
    }
    
    private void UpdateIngredientCapacity()
    {
        ingredientCapacityText.text = $"{ingredientData.Amount}/{ingredientData.MaxAmount}";
    }
}
