using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using Redcode.Moroutines;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField, Expandable, ReadOnly] private LevelSO level;
    
    [SerializeField] private CanvasGroup summaryCanvasGroup;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private Slider sandManSlider;
    [SerializeField] private Slider boogeyManSlider;
    [SerializeField] private float sliderMaxModifier = 3;
    [SerializeField] private Button nextButton;
    public LevelSO Level => level;
    [SerializeField] private Image clock;
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private int accumulatedCurrency;
    [SerializeField, ReadOnly] private int totalCompletedOrder;
    [SerializeField, ReadOnly] private int totalRejectedOrder;
    [SerializeField, ReadOnly] private int totalFailedOrder;
    [SerializedDictionary("Ingredient Type", "Amount"), ReadOnly]
    public SerializedDictionary<IngredientTypes, int> ingredientData;
    
    public delegate void LevelComplete();
    public static event LevelComplete OnLevelComplete;
    private Moroutine gameTimer;
    private bool passQuota;

    private void OnEnable()
    {
        InventoryManager.OnCurrencyChanged += OnCurrencyChange;
        OrderRackUI.OnOutOfRecipe += ShowSummary;
        OrderUI.OnOrderReject += () => totalRejectedOrder++;
        OrderUI.OnOrderComplete += success =>
        {
            if (success) totalCompletedOrder++;
            else totalFailedOrder++;
        };
        IngredientSO.OnIngredientUsed += OnIngredientUsed;
    }

    private void OnDisable()
    {
        InventoryManager.OnCurrencyChanged -= OnCurrencyChange;
        OrderRackUI.OnOutOfRecipe -= ShowSummary;
        IngredientSO.OnIngredientUsed -= OnIngredientUsed;
    }

    private void OnCurrencyChange(int change, int currency)
    {
        accumulatedCurrency += change;
        CheckQuota();
    }
    
    private void OnIngredientUsed(IngredientTypes ingredientType)
    {
        if (ingredientData.ContainsKey(ingredientType))
        {
            ingredientData[ingredientType]++;
        }
        else
        {
            ingredientData.Add(ingredientType, 1);
        }
    }
    
    private void CheckQuota()
    {
        if (!level.HasQuota) return;
        if (accumulatedCurrency < level.Quota) return;
        passQuota = true;
        ShowSummary();
    }

    private void Start()
    {
        level = Instantiate(ProgressionManager.Instance.CurrentLevel);
        gameTimer = Moroutine.Run(gameObject, GameTimer());
        gameTimer.OnCompleted(_ => ShowSummary());
        summaryCanvasGroup.gameObject.SetActive(false);
        nextButton.onClick.AddListener(Next);
    }

    private IEnumerator GameTimer()
    {
        float timer = level.TimeLimit;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            clock.fillAmount = timer / level.TimeLimit;
            yield return null;
        }
    }

    private void ShowSummary()
    {
        gameTimer.Stop();
        OnLevelComplete?.Invoke();
        summaryCanvasGroup.gameObject.SetActive(true);
        summaryText.text = Summary();
        totalText.text = $"Total: {accumulatedCurrency}";
        float max = accumulatedCurrency > level.Quota ? accumulatedCurrency : level.Quota;
        sandManSlider.maxValue = max * sliderMaxModifier;
        boogeyManSlider.maxValue = max * sliderMaxModifier;
        sandManSlider.value = accumulatedCurrency;
        boogeyManSlider.value = level.Quota;
    }

    private string Summary()
    {
        string summary = $"Order:\n\n";
        summary += $"Total Order: {totalCompletedOrder + totalRejectedOrder + totalFailedOrder}\n";
        summary += $"Completed: {totalCompletedOrder}\n";
        summary += $"Rejected: {totalRejectedOrder}\n";
        summary += $"Failed: {totalFailedOrder}\n";
        summary += "\nIngredient Used:\n\n";
        foreach (var ingredient in ingredientData)
        {
            summary += $"{ingredient.Key}: {ingredient.Value}\n";
        }
        return summary;
    }

    private void Next()
    {
        if (passQuota)
        {
            // To Business Plan
        }
        else
        {
            // To Shop
        }
    }
}
