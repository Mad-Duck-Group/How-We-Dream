using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ConfirmationStates
{
    Accept,
    Reject,
    Cancel
}
public class OrderPageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text orderNameText;
    [SerializeField] private TMP_Text orderDescriptionText;
    [SerializeField] private TMP_Text orderDreamTypeText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle acceptButton;
    [SerializeField] private Toggle rejectButton;
    [SerializeField] private Transform ingredientRack;
    [SerializeField] private IngredientIconUI ingredientIconPrefab;

    public delegate void Confirmation(ConfirmationStates state);
    public event Confirmation OnConfirmation;
    public void Initialize(RecipeSO recipe)
    {
        orderNameText.text = $"To: {recipe.OrderName}";
        orderDescriptionText.text = recipe.OrderDescription;
        orderDreamTypeText.text = $"Dream Type: {recipe.DreamType}";
        foreach (var ingredient in recipe.IngredientData)
        {
            if (ingredient.Value == 0) continue;
            var ingredientIcon = Instantiate(ingredientIconPrefab, ingredientRack);
            ingredientIcon.Initialize(ingredient.Key, ingredient.Value);
        }
        acceptButton.isOn = false;
        rejectButton.isOn = false;
        acceptButton.onValueChanged.AddListener(_ => Confirm(acceptButton, true));
        rejectButton.onValueChanged.AddListener(_ => Confirm(rejectButton, false));
        closeButton.onClick.AddListener(Close);
    }

    private void Confirm(Toggle toggle, bool accepted)
    {
        if (accepted)
        {
            OnConfirmation?.Invoke(!toggle.isOn ? ConfirmationStates.Cancel : ConfirmationStates.Accept);
        }
        else
        {
            OnConfirmation?.Invoke(ConfirmationStates.Reject);
        }
        Close();
    }

    public void ResetToggle()
    {
        acceptButton.isOn = false;
        rejectButton.isOn = false;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
