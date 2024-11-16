using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    
    private void OnEnable()
    {
        InventoryManager.onCurrencyChanged += (_, current) => UpdateCurrency(current);
    }

    private void Start()
    {
        UpdateCurrency(InventoryManager.Instance.Currency);
    }

    private void UpdateCurrency(int current)
    {
        currencyText.text = $"Souls: {current}";
    }
}
