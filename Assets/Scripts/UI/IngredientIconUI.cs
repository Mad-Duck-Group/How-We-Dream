using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientIconUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text ingredientAmountText;
    [SerializeField] private TMP_Text ingredientNameText;

    public void Initialize(IngredientTypes ingredient, int amount)
    {
        //icon.sprite = RecipeManager.Instance.GetIngredientSprite(ingredient, CookStates.Raw);
        ingredientAmountText.text = amount.ToString();
        ingredientNameText.text = ingredient.ToString();
    }
}
