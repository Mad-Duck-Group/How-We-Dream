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
    private ShopUI shopUI;
    private bool canBuy;

    private void OnEnable()
    {
        InventoryManager.OnCurrencyChanged += OnCurrencyChanged;
    }
    private void OnDisable()
    {
        InventoryManager.OnCurrencyChanged -= OnCurrencyChanged;
    }
    public void Initialize(IngredientTypes ingredient, ShopUI shopUI)
    {
        var ingredientSO = InventoryManager.Instance.GetIngredientData(ingredient).Ingredient;
        ingredientType = ingredient;
        this.ingredientSO = ingredientSO;
        icon.sprite = ingredientSO.GetSprite(CookStates.Raw);
        nameText.text = ingredient.ToString();
        priceText.text = ingredientSO.CurrentPrice.ToString(CultureInfo.InvariantCulture);
        this.shopUI = shopUI;
        gameObject.SetActive(true);
        UpdateAvailability(InventoryManager.Instance.Currency);
    }

    private void OnCurrencyChanged(int change, int currentCurrency)
    {
        UpdateAvailability(currentCurrency);
    }
    
    private void UpdateAvailability(int currentCurrency)
    {
        canBuy = currentCurrency >= ingredientSO.CurrentPrice;
        Debug.Log($"currentCurrency: {currentCurrency}, ingredientSO.CurrentPrice: {ingredientSO.CurrentPrice}, canBuy: {canBuy}");
        priceText.color = canBuy ? Color.black : Color.red;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (canBuy)
        {
            shopUI.BuyIngredient(ingredientType);
            return;
        }
        GlobalSoundManager.Instance.PlayUISFX("Locked");
    }
}
