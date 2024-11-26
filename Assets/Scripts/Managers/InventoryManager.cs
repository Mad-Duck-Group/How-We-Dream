using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityCommunity.UnitySingleton;
using UnityEngine;

[Serializable]
public class IngredientData
{
    [SerializeField] private IngredientSO ingredient;
    public IngredientSO Ingredient => ingredient;
    [SerializeField] private int amount;
    public int Amount {get => amount; set => amount = value;}
    [SerializeField] private int maxAmount = 30;
    public int MaxAmount => maxAmount;
    
    public IngredientData GetCopy()
    {
        return new IngredientData
        {
            ingredient = ingredient,
            amount = amount,
            maxAmount = maxAmount
        };
    }
}
public class InventoryManager : PersistentMonoSingleton<InventoryManager>
{
    [SerializedDictionary("Ingredient Type", "Ingredient Data")] 
    public SerializedDictionary<IngredientTypes, IngredientData> ingredientData;
    public SerializedDictionary<IngredientTypes, IngredientData> IngredientData => ingredientData;
    [SerializeField] private int currency;
    public int Currency => currency;
    public delegate void IngredientAmountChanged(IngredientTypes ingredient, int amount);
    public static event IngredientAmountChanged OnIngredientAmountChanged;
    public delegate void CurrencyChanged(int change, int current);
    public static event CurrencyChanged OnCurrencyChanged;
    
    private Dictionary<IngredientTypes, IngredientData> startingData = new();
    private int startingCurrency;

    private void Start()
    {
        var dict = ingredientData.ToDictionary(pair => pair.Key, pair => pair.Value.GetCopy());
        startingData = new Dictionary<IngredientTypes, IngredientData>(dict);
        startingCurrency = currency;
    }
    public IngredientData GetIngredientData(IngredientTypes ingredient)
    {
        if (!ingredientData.ContainsKey(ingredient)) return null;
        return ingredientData[ingredient];
    }

    public void ChangeIngredientAmount(IngredientTypes ingredient, int amount)
    {
        if (!ingredientData.ContainsKey(ingredient)) return;
        var data = ingredientData[ingredient];
        if (amount < 0 && data.Amount == 0) return;
        data.Amount += amount;
        data.Amount = Mathf.Clamp(data.Amount, 0, data.MaxAmount);
        OnIngredientAmountChanged?.Invoke(ingredient, data.Amount);
    }
    
    public void ChangeCurrency(int amount)
    {
        currency += amount;
        currency = Mathf.Clamp(currency, 0, int.MaxValue);
        OnCurrencyChanged?.Invoke(amount, currency);
    }
    
    public void ResetInventory()
    {
        var dict = startingData.ToDictionary(pair => pair.Key, pair => pair.Value.GetCopy());
        ingredientData = new SerializedDictionary<IngredientTypes, IngredientData>(dict);
        currency = startingCurrency;
    }
}
