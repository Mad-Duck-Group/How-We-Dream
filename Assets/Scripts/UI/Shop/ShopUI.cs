using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour, IUIPage
{
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private ScrollRect shopScrollRect;
    [SerializeField] private ShopItemUI shopItemPrefab;
    [SerializeField] private Transform shopCapacityParent;
    [SerializeField] private ShopCapacityUI shopCapacityUIPrefab;

    public void Initialize()
    {
        shopCanvasGroup.gameObject.SetActive(true);
        InitializeShop();
        shopCanvasGroup.gameObject.SetActive(false);
    }

    private void InitializeShop()
    {
        var allIngredientTypes = Enum.GetValues(typeof(IngredientTypes));
        foreach (IngredientTypes ingredient in allIngredientTypes)
        {
            var shopItem = Instantiate(shopItemPrefab, shopScrollRect.content);
            shopItem.Initialize(ingredient, this);
            var shopCapacity = Instantiate(shopCapacityUIPrefab, shopCapacityParent);
            shopCapacity.Initialize(ingredient);
        }
    }
    public void BuyIngredient(IngredientTypes ingredient)
    {
        var ingredientSO = InventoryManager.Instance.GetIngredientData(ingredient).Ingredient;
        var price = ingredientSO.CurrentPrice;
        if (InventoryManager.Instance.Currency < price) return;
        InventoryManager.Instance.ChangeCurrency(-price);
        InventoryManager.Instance.ChangeIngredientAmount(ingredient, 1);
        GlobalSoundManager.Instance.PlayUISFX("Purchase");
    }
    public bool Open()
    {
        shopCanvasGroup.gameObject.SetActive(true);
        return true;
    }
    
    public bool Close()
    {
        shopCanvasGroup.gameObject.SetActive(false);
        return true;
    }
}
