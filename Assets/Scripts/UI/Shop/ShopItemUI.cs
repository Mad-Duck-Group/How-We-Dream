using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image icon;

    private IngredientTypes ingredientType;
    private IngredientSO ingredientSO;
    private bool canBuy;

    public void Initialize(IngredientTypes ingredient)
    {
        InventoryManager.onCurrencyChanged += (_, current) => UpdateAvailability(current);
        var ingredientSO = InventoryManager.Instance.GetIngredientData(ingredient).Ingredient;
        ingredientType = ingredient;
        this.ingredientSO = ingredientSO;
        icon.sprite = ingredientSO.GetSprite(CookStates.Raw);
        nameText.text = ingredient.ToString();
        priceText.text = ingredientSO.BasePrice.ToString(CultureInfo.InvariantCulture);
        gameObject.SetActive(true);
        UpdateAvailability(InventoryManager.Instance.Currency);
    }
    
    private void UpdateAvailability(int currentCurrency)
    { 
        if (!gameObject.activeSelf) return;
        canBuy = currentCurrency >= ingredientSO.BasePrice;
        priceText.color = canBuy ? Color.black : Color.red;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canBuy) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ShopManager.Instance.BuyIngredient(ingredientType);
    }
}
