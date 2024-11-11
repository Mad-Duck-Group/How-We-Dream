using System;
using System.Collections;
using System.Collections.Generic;
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
}
public class InventoryManager : PersistentMonoSingleton<InventoryManager>
{
    [SerializedDictionary("Ingredient Type", "Ingredient Data")] 
    public SerializedDictionary<IngredientTypes, IngredientData> ingredientData;
    public SerializedDictionary<IngredientTypes, IngredientData> IngredientData => ingredientData;
    public delegate void OnIngredientAmountChanged(IngredientTypes ingredient, int amount);
    public static event OnIngredientAmountChanged onIngredientAmountChanged;
    public void ChangeIngredientAmount(IngredientTypes ingredient, int amount)
    {
        if (!ingredientData.ContainsKey(ingredient)) return;
        var data = ingredientData[ingredient];
        if (amount < 0 && data.Amount == 0) return;
        data.Amount += amount;
        onIngredientAmountChanged?.Invoke(ingredient, data.Amount);
    }
}
