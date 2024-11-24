using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using NaughtyAttributes;
using Redcode.Moroutines;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private CanvasGroup startEndPanel;
    [SerializeField] private TMP_Text nightText;
    [SerializeField] private TMP_Text startEndText;
    [SerializeField] private Image clock;
    [SerializeField] private Transform clockHandRotator;

    [Header("Debug")]
    [SerializeField, ReadOnly] private SummaryData summaryData;
    public SummaryData SummaryData => summaryData;
    
    
    public delegate void LevelStart();
    public static event LevelStart OnLevelStart;
    public delegate void LevelComplete();
    public static event LevelComplete OnLevelComplete;
    private IEnumerator<VNPathSO> currentVN;
    private bool gameStart;
    private bool gameEnd;
    private Moroutine gameTimer;
    private bool passQuota;
    public bool PassQuota => passQuota;

    private void OnEnable()
    {
        InventoryManager.OnCurrencyChanged += OnCurrencyChange;
        OrderRackUI.OnOutOfRecipe += GameEnd;
        OrderUI.OnOrderReject += OnOrderReject;
        OrderUI.OnOrderComplete += OnOrderComplete;
        IngredientSO.OnIngredientUsed += OnIngredientUsed;
        VNManager.OnVNFinished += OnVNFinished;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManagerPersistent.OnFinishFadeIn += InitializeVN;
    }

    private void OnDisable()
    {
        InventoryManager.OnCurrencyChanged -= OnCurrencyChange;
        OrderRackUI.OnOutOfRecipe -= GameEnd;
        OrderUI.OnOrderReject -= OnOrderReject;
        OrderUI.OnOrderComplete -= OnOrderComplete;
        IngredientSO.OnIngredientUsed -= OnIngredientUsed;
        VNManager.OnVNFinished -= OnVNFinished;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManagerPersistent.OnFinishFadeIn -= InitializeVN;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeStartEnd();
    }
    
    private void OnOrderReject()
    {
        summaryData.TotalRejectedOrder++;
    }
    
    private void OnOrderComplete(bool success)
    {
        if (success) summaryData.TotalCompletedOrder++;
        else summaryData.TotalFailedOrder++;
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
    
    private void OnVNFinished(VNPathSO vnPath)
    {
        if (currentVN.Current != vnPath) return;
        bool canMoveNext = currentVN.MoveNext();
        switch (gameStart)
        {
            case false when !canMoveNext:
                StartGame();
                break;
            case false:
                VNManager.Instance.ShowVN(currentVN.Current);
                break;
        }
        switch (gameEnd)
        {
            case true when !canMoveNext:
                ShowEndPanel();
                break;
            case true:
                VNManager.Instance.ShowVN(currentVN.Current);
                break;
        }
    }

    private void StartGame()
    {
        gameTimer = Moroutine.Run(gameObject, GameTimer());
        gameTimer.OnCompleted(_ => GameEnd());
        gameStart = true;
        startEndPanel.gameObject.SetActive(true);
        nightText.text = $"Night {ProgressionManager.Instance.CurrentLevelIndex + 1}";
        startEndText.text = "Start Work!";
        GlobalSoundManager.Instance.PlayUISFX("StartWork");
        GlobalSoundManager.Instance.PlayBGM("GameplayBGM");
        var sequence = DOTween.Sequence();
        sequence.Append(startEndPanel.DOFade(1f, 0.5f));
        sequence.Append(startEndPanel.DOFade(1f, 2f));
        sequence.Append(startEndPanel.DOFade(0f, 0.5f));
        sequence.OnComplete(() =>
        {
            OnLevelStart?.Invoke();
            startEndPanel.gameObject.SetActive(false);
        });
    }
    
    private void CheckQuota()
    {
        if (gameEnd) return;
        if (!level.HasQuota) return;
        if (summaryData.AccumulatedCurrency < level.Quota) return;
        passQuota = true;
        ProgressionManager.Instance.SkillPoints++;
        GameEnd();
    }

    private void Start()
    {
        level = Instantiate(ProgressionManager.Instance.CurrentLevel);
        summaryData = new SummaryData();
        startEndPanel.DOFade(1f, 0f);
        if (SceneManagerPersistent.FirstSceneLoaded) return;
        InitializeVN();
    }

    private void InitializeStartEnd()
    {
        currentVN = level.BeforeStartVN.GetEnumerator();
        currentVN.MoveNext();
        if (currentVN.Current && !currentVN.Current.Played) startEndPanel.gameObject.SetActive(false);
        else startEndPanel.gameObject.SetActive(true);
    }

    private void InitializeVN()
    {
        if (level.ShowVNAtStart)
        {
            currentVN = level.BeforeStartVN.GetEnumerator();
            currentVN.MoveNext();
            if (VNManager.Instance.ShowVN(currentVN.Current))
            {
                startEndPanel.gameObject.SetActive(false);
            }
        }
        else
        {
            StartGame();
        }
    }

    private IEnumerator GameTimer()
    {
        float timer = level.TimeLimit;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            clock.fillAmount = timer / level.TimeLimit;
            clockHandRotator.localEulerAngles = new Vector3(0, 0, timer / level.TimeLimit * 360);
            yield return null;
        }
    }

    private void GameEnd()
    {
        if (gameEnd) return;
        gameEnd = true;
        gameTimer.Stop();
        OnLevelComplete?.Invoke();
        if (level.ShowVNWhenFail || level.ShowVNWhenSuccess)
        {
            currentVN = passQuota ? level.SuccessVN.GetEnumerator() : level.FailVN.GetEnumerator();
            currentVN.MoveNext();
            VNManager.Instance.ShowVN(currentVN.Current);
        }
        else
        {
            ShowEndPanel();
        }
    }

    private void ShowEndPanel()
    {
        startEndPanel.gameObject.SetActive(true);
        nightText.text = $"Night {ProgressionManager.Instance.CurrentLevelIndex + 1}";
        startEndText.text = "End Work";
        GlobalSoundManager.Instance.PlayUISFX("EndWork");
        var sequence = DOTween.Sequence();
        sequence.Append(startEndPanel.DOFade(1f, 0.5f).OnComplete(() => UIPageManager.Instance.ChangePage(PageTypes.Summary)));
        sequence.Append(startEndPanel.DOFade(1f, 2f));
        sequence.Append(startEndPanel.DOFade(0f, 0.5f));
        sequence.OnComplete(() =>
        {
            startEndPanel.gameObject.SetActive(false);
            GlobalSoundManager.Instance.PlayBGM("SummaryBGM", duration: 0.5f);
        });
    }
}
