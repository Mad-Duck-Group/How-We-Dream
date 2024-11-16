using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class BottleUI : MonoBehaviour, IIngredientContainer
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private float beadSize = 0.5f;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private Vector2 randomOffset;
    
    [SerializeField, ReadOnly, Expandable] private IngredientSO ingredient;
    
    private Stack<KeyValuePair<IngredientSO, IngredientUI>> ingredients = new();
    private IngredientUI ingredientUI;

    public bool SetIngredient(IngredientSO ingredient)
    {
        var ingredientUI = Instantiate(ingredientUIPrefab, RandomDropPoint(), Quaternion.identity);
        ingredientUI.Initialize(ingredient, gameObject);
        ingredientUI.SetSizeAndPhysics(true, false, beadSize);
        ingredients.Push(new KeyValuePair<IngredientSO, IngredientUI>(ingredient, ingredientUI));
        return true;
    }
    
    private void ReturnToDropPoint()
    {
        ingredientUI.transform.position = RandomDropPoint();
        ingredientUI.SetSizeAndPhysics(true, false, beadSize);
    }
    
    private Vector3 RandomDropPoint()
    {
        var offset = new Vector3(Random.Range(-randomOffset.x, randomOffset.x), Random.Range(-randomOffset.y, randomOffset.y), 0);
        return dropPoint.position + offset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredients.Count == 0) return;
        ingredient = ingredients.Peek().Key;
        ingredientUI = ingredients.Peek().Value;
        ingredientUI.SetSizeAndPhysics(false, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (ingredients.Count == 0) return;
        ingredientUI.Drag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ingredientUI == null) return;
        bool success = ingredientUI.EndDragCheck(eventData);
        if (!success)
        {
            ReturnToDropPoint();
            return;
        }
        var ui = ingredients.Pop().Value;
        Destroy(ui.gameObject);
        ingredient = null;
    }

    public void Submit()
    {
        RecipeManager.Instance.CheckRecipe(ingredients.Select(pair => pair.Key).ToList());
        foreach (var pair in ingredients)
        {
            Destroy(pair.Value.gameObject);
        }
        ingredients.Clear();
    }
}
