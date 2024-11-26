using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite[] scrollSprites;
    [SerializeField] private Image scroll;
    //[SerializeField] private Image flag;
    //[SerializeField] private TMP_Text orderText;
    [SerializeField] private OrderPageUI orderPageUIPrefab;

    private RecipeSO recipe;
    private float timer;
    private Moroutine timerCoroutine;
    private OrderPageUI orderPageUI;
    private Image clock;
    private CanvasGroup canvasGroup;
    private bool empty = true;
    public bool Empty => empty;
    private Vector2 showPosition;
    private Vector2 hidePosition;
    private RectTransform rectTransform;
    
    public delegate void OrderReject();
    public static event OrderReject OnOrderReject;
    public delegate void OrderComplete(bool success);
    public static event OrderComplete OnOrderComplete;
    public delegate void OrderDestroy(OrderUI orderUI);
    public static event OrderDestroy OnOrderDestroy;


    public void Initialize(Image clock)
    {
        this.clock = clock;
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        var width = rectTransform.rect.width;
        showPosition = rectTransform.anchoredPosition;
        hidePosition = new Vector2(showPosition.x + width, showPosition.y);
        rectTransform.anchoredPosition = hidePosition;
        gameObject.SetActive(false);
    }
    
    public void SetOrder(RecipeSO recipeSo)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        clock.color = Color.white;
        empty = false;
        recipe = recipeSo;
        clock.enabled = recipe.HasTimeLimit;
        LevelManager.OnLevelComplete += OnLevelComplete;
        RecipeManager.OnRecipeChanged += OnRecipeChange;
        RecipeManager.OnRecipeComplete += OnRecipeComplete;
        scroll.sprite = scrollSprites[0];
        orderPageUI = Instantiate(orderPageUIPrefab, transform.root);
        orderPageUI.Initialize(recipe);
        orderPageUI.OnConfirmation += OnOrderConfirmation;
        orderPageUI.gameObject.SetActive(false);
        rectTransform.DOAnchorPos(showPosition, 0.2f);
        GlobalSoundManager.Instance.PlayUISFX("NewOrder");
        if (!recipe.HasTimeLimit) return;
        timer = recipe.TimeLimit;
        timerCoroutine = Moroutine.Run(gameObject, Timer());
        timerCoroutine.OnCompleted(_ => Reject(true));
    }

    public void SetEmpty()
    {
        empty = true;
    }

    public void TweenOut(bool reject = false)
    {
        var sequence = DOTween.Sequence();
        if (reject)
            sequence.Append(rectTransform.DOAnchorPos(hidePosition, 0.2f));
        else
            sequence.Append(canvasGroup.DOFade(0f, 0.2f));
        sequence.Join(clock.DOFade(0f, 0.5f));
        sequence.Append(rectTransform.DOAnchorPos(hidePosition, 2f));
        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            clock.enabled = false;
            OnOrderDestroy?.Invoke(this);
        });
    }

    private void OnLevelComplete()
    {
        timerCoroutine?.Stop();
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
                //flag.color = Color.yellow;
                scroll.sprite = scrollSprites[1];
                break;
            case true when recipe != recipeSo:
                //flag.color = Color.gray;
                scroll.sprite = scrollSprites[0];
                orderPageUI.ResetToggle();
                break;
            case false when recipe == recipeSo:
                //flag.color = Color.gray;
                scroll.sprite = scrollSprites[0];
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
        OnOrderComplete?.Invoke(success);
        RecipeManager.Instance.UnsetActiveRecipe(recipe);
        TweenOut();
        DestroyOrder();
    }

    private void Accept()
    {
        RecipeManager.Instance.SetActiveRecipe(recipe);
        GlobalSoundManager.Instance.PlayUISFX("AcceptOrder");
    }

    private void Cancel()
    {
        RecipeManager.Instance.UnsetActiveRecipe(recipe);
    }

    private void Reject(bool outOfTime = false)
    {
        OnOrderReject?.Invoke();
        if (outOfTime)
        {
            //InventoryManager.Instance.ChangeCurrency(recipe.HasTimeLimit ? -50 : -10);
            OnOrderComplete?.Invoke(false);
        }
        GlobalSoundManager.Instance.PlayUISFX("DeclineOrder");
        RecipeManager.Instance.UnsetActiveRecipe(recipe);
        TweenOut(true);
        DestroyOrder();
    }

    private void DestroyOrder()
    {
        RecipeManager.OnRecipeChanged -= OnRecipeChange;
        RecipeManager.OnRecipeComplete -= OnRecipeComplete;
        SetEmpty();
        if (orderPageUI)
            Destroy(orderPageUI.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (orderPageUI.gameObject.activeSelf) return;
        GlobalSoundManager.Instance.PlayUISFX("OpenOrder");
        orderPageUI.Open();
    }
}
