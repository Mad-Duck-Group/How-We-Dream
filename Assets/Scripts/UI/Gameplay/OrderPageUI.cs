using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private Image timeLimitIcon;
    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle acceptButton;
    [SerializeField] private Toggle rejectButton;
    [SerializeField] private Transform ingredientRack;
    [FormerlySerializedAs("ingredientIconPrefab")] [SerializeField] private IngredientOrderIconUI ingredientOrderIconPrefab;

    public delegate void Confirmation(ConfirmationStates state);
    public event Confirmation OnConfirmation;
    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void Initialize(RecipeSO recipe)
    {
        orderNameText.text = $"{recipe.OrderName}";
        orderDescriptionText.text = recipe.OrderDescription;
        var dreamName = RecipeManager.Instance.DreamData[recipe.DreamType].DreamTypeName;
        orderDreamTypeText.text = $"{dreamName}";
        timeLimitIcon.enabled = recipe.HasTimeLimit;
        foreach (var ingredient in recipe.IngredientData)
        {
            if (ingredient.Value == 0) continue;
            var ingredientIcon = Instantiate(ingredientOrderIconPrefab, ingredientRack);
            ingredientIcon.Initialize(ingredient.Key, ingredient.Value);
        }
        acceptButton.isOn = false;
        rejectButton.isOn = false;
        acceptButton.onValueChanged.AddListener(_ => Confirm(acceptButton, true));
        rejectButton.onValueChanged.AddListener(_ => Confirm(rejectButton, false));
        closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
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
        canvasGroup.DOFade(0f, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}
