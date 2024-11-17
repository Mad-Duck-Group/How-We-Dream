using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Redcode.Moroutines;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField, Expandable, ReadOnly] private LevelSO level;
    public LevelSO Level => level;
    [SerializeField] private Image clock;
    [SerializeField, ReadOnly] private int accumulatedCurrency;
    
    private Moroutine gameTimer;

    private void OnEnable()
    {
        InventoryManager.onCurrencyChanged += OnCurrencyChange;
    }

    private void OnDisable()
    {
        InventoryManager.onCurrencyChanged -= OnCurrencyChange;
    }
    
    private void OnCurrencyChange(int change, int currency)
    {
        accumulatedCurrency += change;
        CheckQuota();
    }
    
    private void CheckQuota()
    {
        if (!level.HasQuota) return;
        if (accumulatedCurrency < level.Quota) return;
        Win();
    }

    private void Start()
    {
        level = Instantiate(ProgressionManager.Instance.CurrentLevel);
        gameTimer = Moroutine.Run(gameObject, GameTimer());
        gameTimer.OnCompleted(_ => Lose());
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

    private void Win()
    {
        gameTimer.Stop();
        Debug.Log("You Win!");
    }

    private void Lose()
    {
        Debug.Log("You Lose!");
    }
}
