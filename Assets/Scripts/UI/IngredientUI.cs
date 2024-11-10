using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngredientUI : MonoBehaviour
{
    private IngredientSO ingredient;
    private Image image;
    private GameObject owner;

    public void SetIngredient(IngredientSO ingredient, GameObject owner)
    {
        image = GetComponent<Image>();
        this.ingredient = ingredient;
        image.sprite = ingredient.GetSprite(ingredient.CookState);
        this.owner = owner;
    }
    
    public bool EndDragCheck(PointerEventData eventData)
    {
        if (eventData.hovered.Count == 0) return false;
        var top = eventData.hovered[0];
        if (top == owner) return false;
        if (top.TryGetComponent(out EquipmentUI equipmentUI))
        {
            equipmentUI.SetIngredient(ingredient);
            return true;
        }
        if (ingredient.CookState == CookStates.Raw)
        {
            if (top.TryGetComponent(out IngredientRackUI ingredientRackUI))
            {
                ingredientRackUI.SetIngredient(ingredient);
                return true;
            }
            if (top.TryGetComponent(out IngredientSlotUI ingredientSlotUI))
            {
                ingredientSlotUI.SetIngredient(ingredient);
                return true;
            }
        }
        Debug.Log(eventData.hovered[^1]);
        return false;
    }
    
}
