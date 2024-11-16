using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientOrderIconUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text ingredientAmountText;
    [SerializeField] private TMP_Text ingredientNameText;

    public void Initialize(IngredientTypes ingredient, int amount)
    {
        //icon.sprite = InventoryManager.Instance.GetIngredientData(ingredient).Ingredient.GetSprite(CookStates.Raw);
        ingredientAmountText.text = amount.ToString();
        ingredientNameText.text = ingredient.ToString();
    }
}
