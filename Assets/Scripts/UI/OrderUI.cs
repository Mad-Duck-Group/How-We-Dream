using System;
using System.Collections;
using System.Collections.Generic;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image flag;
    [SerializeField] private TMP_Text orderText;
    [SerializeField] private Image clock;
    [SerializeField] private OrderPageUI orderPageUIPrefab;

    private RecipeSO recipe;
    private float timer;
    private Moroutine timerCoroutine;
    private OrderPageUI orderPageUI;
    public delegate void OnDestroyOrder();
    public static event OnDestroyOrder OnOrderDestroyed;

    public void Initialize(RecipeSO recipeSo)
    {
        recipe = recipeSo;
        orderText.text = recipe.OrderName;
        clock.enabled = recipe.HasTimeLimit;
        RecipeManager.OnRecipeChanged += OnRecipeChange;
        RecipeManager.OnRecipeComplete += OnRecipeComplete;
        flag.color = Color.gray;
        orderPageUI = Instantiate(orderPageUIPrefab, transform.root);
        orderPageUI.Initialize(recipe);
        orderPageUI.OnConfirmation += OnOrderConfirmation;
        orderPageUI.gameObject.SetActive(false);
        if (!recipe.HasTimeLimit) return;
        timer = recipe.TimeLimit;
        timerCoroutine = Moroutine.Run(gameObject, Timer());
        timerCoroutine.OnCompleted(_ => Reject());
    }
    
    private IEnumerator Timer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            clock.fillAmount = timer / recipe.TimeLimit;
            yield return null;
        }
    }

    private void OnRecipeChange(RecipeSO recipeSo, bool active)
    {
        switch (active)
        {
            case true when recipe == recipeSo :
                flag.color = Color.yellow;
                break;
            case true when recipe != recipeSo:
                flag.color = Color.gray;
                orderPageUI.ResetToggle();
                break;
            case false when recipe == recipeSo:
                flag.color = Color.gray;
                break;
        }
    }

    private void OnOrderConfirmation(ConfirmationStates state)
    {
        switch (state)
        {
            case ConfirmationStates.Accept:
                Accept();
                break;
            case ConfirmationStates.Cancel:
                Cancel();
                break;
            case ConfirmationStates.Reject:
                Reject();
                break;
        }
    }
    
    private void OnRecipeComplete(RecipeSO recipeSo, bool success)
    {
        if (recipe != recipeSo) return;
        if (success)
        {
            RecipeManager.Instance.UnsetActiveRecipe(recipe);
            DestroyOrder();
        }
    }

    private void Accept()
    {
        RecipeManager.Instance.SetActiveRecipe(recipe);
    }

    private void Cancel()
    {
        RecipeManager.Instance.UnsetActiveRecipe(recipe);
    }

    private void Reject()
    {
        RecipeManager.Instance.UnsetActiveRecipe(recipe);
        DestroyOrder();
    }

    private void DestroyOrder()
    {
        RecipeManager.OnRecipeChanged -= OnRecipeChange;
        RecipeManager.OnRecipeComplete -= OnRecipeComplete;
        OnOrderDestroyed?.Invoke();
        Destroy(orderPageUI.gameObject);
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (orderPageUI.gameObject.activeSelf) return;
        orderPageUI.gameObject.SetActive(true);
    }
}
