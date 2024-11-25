using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private Sprite pointSprite;
    [SerializeField] private Sprite availableSprite;
    [SerializeField] private Sprite unavailableSprite;

    private IngredientTypes ingredientType;
    private IngredientSO ingredientSO;
    private ShopUI shopUI;
    private bool canBuy;

    private void OnEnable()
    {
        InventoryManager.OnCurrencyChanged += OnCurrencyChanged;
        UpdateAvailability(InventoryManager.Instance.Currency);
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
        var price = ingredientSO.CurrentPrice.ToString(CultureInfo.InvariantCulture);
        priceText.text = $"Souls: {price}";
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
        if (!ingredientSO) return;
        canBuy = currentCurrency >= ingredientSO.CurrentPrice;
        background.sprite = canBuy ? availableSprite : unavailableSprite;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canBuy) return;
        background.sprite = pointSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.sprite = canBuy ? availableSprite : unavailableSprite;
    }
}
