using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngredientSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private TMP_Text amountText;
    
    private IngredientTypes ingredientType;
    public IngredientTypes IngredientType => ingredientType;
    private IngredientSO ingredient;
    private Image image;
    private IngredientUI ingredientUI;
    // Start is called before the first frame update
    private void Awake()
    {
        image = GetComponent<Image>();
        InventoryManager.onIngredientAmountChanged += (_, _) => UpdateAmountText();
    }
    
    public void SetIngredient(IngredientSO ingredient, bool initialization = false)
    {
        this.ingredient = ingredient;
        if (!initialization)
            InventoryManager.Instance.ChangeIngredientAmount(ingredient.IngredientType, 1);
        else
        {
            ingredientType = ingredient.IngredientType;
        }
        UpdateAmountText();
    }
    
    private void UpdateAmountText()
    {
        if (!InventoryManager.Instance.IngredientData.ContainsKey(ingredientType)) return;
        var data = InventoryManager.Instance.IngredientData[ingredientType];
        amountText.text = data.Amount.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ingredientUI = Instantiate(ingredientUIPrefab, transform.root);
        ingredientUI.transform.position = Input.mousePosition;
        ingredientUI.SetIngredient(ingredient, gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ingredientUI.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ingredientUI == null) return;
        bool success = ingredientUI.EndDragCheck(eventData);
        Destroy(ingredientUI.gameObject);
        if (success)
        {
            InventoryManager.Instance.ChangeIngredientAmount(ingredient.IngredientType, -1);
        }
    }
}
