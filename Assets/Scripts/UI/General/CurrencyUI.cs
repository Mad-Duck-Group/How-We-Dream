using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    
    private Bumpable bumpable;
    
    private void Awake()
    {
        bumpable = GetComponent<Bumpable>();
    }

    private void OnEnable()
    {
        InventoryManager.OnCurrencyChanged += OnCurrencyChanged;
    }
    
    private void OnDisable()
    {
        InventoryManager.OnCurrencyChanged -= OnCurrencyChanged;
    }
    
    private void OnCurrencyChanged(int change, int current)
    {
        UpdateCurrency(current);
    }

    private void Start()
    {
        UpdateCurrency(InventoryManager.Instance.Currency);
    }

    private void UpdateCurrency(int current)
    {
        currencyText.text = $"{current}";
        bumpable.BumpUp();
        DOVirtual.DelayedCall(0.2f, () => bumpable.BumpDown());
    }
}
