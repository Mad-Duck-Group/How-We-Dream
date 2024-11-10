using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientRackUI : MonoBehaviour
{
    [SerializeField] private IngredientSlotUI ingredientSlotPrefab;
    private Dictionary<IngredientTypes, IngredientSlotUI> ingredientSlots = new();
    
    // Start is called before the first frame update
    void Start()
    {
        PopulateRack();
    }
    
    private void PopulateRack()
    {
        foreach (var ingredientData in InventoryManager.Instance.IngredientData)
        {
            var ingredientSlot = Instantiate(ingredientSlotPrefab, transform);
            ingredientSlot.SetIngredient(ingredientData.Value.Ingredient, true);
            ingredientSlots.Add(ingredientData.Key, ingredientSlot);
        }
    }

    public void SetIngredient(IngredientSO ingredient)
    {
        if (!ingredientSlots.ContainsKey(ingredient.IngredientType)) return;
        var slot = ingredientSlots[ingredient.IngredientType];
        slot.SetIngredient(ingredient);
    }
}
