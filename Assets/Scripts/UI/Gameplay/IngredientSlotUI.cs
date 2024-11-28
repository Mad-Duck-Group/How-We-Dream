using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngredientSlotUI : MonoBehaviour, IIngredientContainer
{
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField, ReadOnly, Expandable] private IngredientSO ingredient;
    
    private IngredientTypes ingredientType;
    public IngredientTypes IngredientType => ingredientType;
    private Image image;
    private IngredientUI ingredientUI;
    private IngredientData data;
    // Start is called before the first frame update
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        InventoryManager.OnIngredientAmountChanged += OnIngredientAmountChanged;
    }
    
    private void OnDisable()
    {
        InventoryManager.OnIngredientAmountChanged -= OnIngredientAmountChanged;
    }
    
    private void OnIngredientAmountChanged(IngredientTypes type, int amount)
    {
        UpdateAmountText(type);
    }

    public void Initialize(IngredientSO ingredient)
    {
        this.ingredient = ingredient;
        ingredientType = ingredient.IngredientType;
        nameText.text = ingredient.IngredientType.ToString();
        image.sprite = ingredient.GetSprite(CookStates.Raw);
        UpdateAmountText(ingredientType);
    }
    
    public bool SetIngredient(IngredientSO ingredient)
    {
        this.ingredient = ingredient;
        InventoryManager.Instance.ChangeIngredientAmount(ingredient.IngredientType, 1);
        UpdateAmountText(ingredientType);
        return true;
    }

    private void UpdateAmountText(IngredientTypes type)
    {
        if (type != ingredientType) return;
        if (!InventoryManager.Instance.IngredientData.ContainsKey(ingredientType)) return;
        data = InventoryManager.Instance.IngredientData[ingredientType];
        amountText.text = data.Amount.ToString();
        gameObject.SetActive(data.Amount != 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data.Amount == 0) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ingredientUI = Instantiate(ingredientUIPrefab);
        var ingredientInstance = Instantiate(ingredient);
        ingredientUI.Initialize(ingredientInstance, gameObject);
        ingredientUI.BeingDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (data.Amount == 0) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ingredientUI.Drag();
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
