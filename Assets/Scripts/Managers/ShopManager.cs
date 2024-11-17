using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : PersistentMonoSingleton<ShopManager>
{
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private ScrollRect shopScrollRect;
    [SerializeField] private ShopItemUI shopItemPrefab;
    [SerializeField] private Transform shopCapacityParent;
    [SerializeField] private ShopCapacityUI shopCapacityUIPrefab;

    private void Start()
    {
        Initialize();
        shopCanvasGroup.gameObject.SetActive(false);
    }

    private void Initialize()
    {
        var allIngredientTypes = Enum.GetValues(typeof(IngredientTypes));
        foreach (IngredientTypes ingredient in allIngredientTypes)
        {
            var shopItem = Instantiate(shopItemPrefab, shopScrollRect.content);
            shopItem.Initialize(ingredient);
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
    }
    
    [NaughtyAttributes.Button("Open Shop")]
    public void OpenShop()
    {
        shopCanvasGroup.gameObject.SetActive(true);
    }
    
    [NaughtyAttributes.Button("Close Shop")]
    public void CloseShop()
    {
        shopCanvasGroup.gameObject.SetActive(false);
    }
}
