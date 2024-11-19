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


[Serializable]
public class SummaryData
{ 
    [SerializeField, ReadOnly] private int accumulatedCurrency;
    public int AccumulatedCurrency { get => accumulatedCurrency; set => accumulatedCurrency = value; }
    [SerializeField, ReadOnly] private int totalCompletedOrder;
    public int TotalCompletedOrder { get => totalCompletedOrder; set => totalCompletedOrder = value; }
    [SerializeField, ReadOnly] private int totalRejectedOrder;
    public int TotalRejectedOrder { get => totalRejectedOrder; set => totalRejectedOrder = value; }
    [SerializeField, ReadOnly] private int totalFailedOrder;
    public int TotalFailedOrder { get => totalFailedOrder; set => totalFailedOrder = value; }

    [SerializedDictionary("Ingredient Type", "Amount"), ReadOnly]
    public SerializedDictionary<IngredientTypes, int> ingredientData = new();
    public SerializedDictionary<IngredientTypes, int> IngredientData => ingredientData;
    
    public void Reset()
    {
        accumulatedCurrency = 0;
        totalCompletedOrder = 0;
        totalRejectedOrder = 0;
        totalFailedOrder = 0;
        ingredientData.Clear();
    }
}
public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField, Expandable, ReadOnly] private LevelSO level;
    public LevelSO Level => level;
    [SerializeField] private Image clock;

    [Header("Debug")]
    [SerializeField, ReadOnly] private SummaryData summaryData;
    public SummaryData SummaryData => summaryData;
    
    public delegate void LevelComplete();
    public static event LevelComplete OnLevelComplete;
    private Moroutine gameTimer;
    private bool passQuota;
    public bool PassQuota => passQuota;

    private void OnEnable()
    {
        InventoryManager.OnCurrencyChanged += OnCurrencyChange;
        OrderRackUI.OnOutOfRecipe += ShowSummary;
        OrderUI.OnOrderReject += () => summaryData.TotalRejectedOrder++;
        OrderUI.OnOrderComplete += success =>
        {
            if (success) summaryData.TotalCompletedOrder++;
            else summaryData.TotalFailedOrder++;
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
        summaryData.AccumulatedCurrency += change;
        CheckQuota();
    }
    
    private void OnIngredientUsed(IngredientTypes ingredientType)
    {
        if (summaryData.IngredientData.ContainsKey(ingredientType))
        {
            summaryData.IngredientData[ingredientType]++;
        }
        else
        {
            summaryData.IngredientData.Add(ingredientType, 1);
        }
    }
    
    private void CheckQuota()
    {
        if (!level.HasQuota) return;
        if (summaryData.AccumulatedCurrency < level.Quota) return;
        passQuota = true;
        ProgressionManager.Instance.CanUpgradeSkill = true;
        ShowSummary();
    }

    private void Start()
    {
        level = Instantiate(ProgressionManager.Instance.CurrentLevel);
        summaryData = new SummaryData();
        gameTimer = Moroutine.Run(gameObject, GameTimer());
        gameTimer.OnCompleted(_ => ShowSummary());
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
        UIPageManager.Instance.ChangePage(PageTypes.Summary);
    }
}
