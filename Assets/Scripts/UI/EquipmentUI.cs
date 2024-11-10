using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    private IngredientSO ingredient;
    private IngredientUI ingredientUI;
    // Start is called before the first frame update
    public void SetIngredient(IngredientSO ingredient)
    {
        this.ingredient = ingredient;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredient == null) return;
        ingredientUI = Instantiate(ingredientUIPrefab, transform.root);
        ingredientUI.transform.position = Input.mousePosition;
        ingredientUI.SetIngredient(ingredient, gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredient == null) return;
        ingredientUI.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ingredientUI == null) return;
        bool success = ingredientUI.EndDragCheck(eventData);
        Destroy(ingredientUI.gameObject);
        if (success) ingredient = null;
    }
}
