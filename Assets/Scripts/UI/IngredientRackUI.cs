using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientRackUI : MonoBehaviour, IIngredientContainer
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
            ingredientSlot.Initialize(ingredientData.Value.Ingredient);
            ingredientSlot.gameObject.name = ingredientData.Key.ToString();
            ingredientSlots.Add(ingredientData.Key, ingredientSlot);
        }
    }

    public bool SetIngredient(IngredientSO ingredient)
    {
        if (!ingredientSlots.ContainsKey(ingredient.IngredientType)) return false;
        var slot = ingredientSlots[ingredient.IngredientType];
        slot.SetIngredient(ingredient);
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
       
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
}
