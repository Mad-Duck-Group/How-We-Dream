using System.Collections;
using System.Collections.Generic;
using Redcode.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryUI : MonoBehaviour, IUIPage
{
    [SerializeField] private CanvasGroup summaryCanvasGroup;
    [SerializeField] private Image quotaStamp;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private Slider sandManSlider;
    [SerializeField] private Image quotaLine;
    [SerializeField] private Slider boogeyManSlider;
    [SerializeField] private float sliderMaxModifier = 3;

    private SummaryData SummaryData => LevelManager.Instance.SummaryData;
    private LevelSO Level => LevelManager.Instance.Level;

    public void Initialize()
    {
        summaryCanvasGroup.gameObject.SetActive(true);
        summaryCanvasGroup.gameObject.SetActive(false);
    }
    
    private void ShowSummary()
    {
        summaryCanvasGroup.gameObject.SetActive(true);
        summaryText.text = Summary();
        LayoutRebuilder.ForceRebuildLayoutImmediate(summaryText.transform.parent as RectTransform);
        totalText.text = $"{SummaryData.AccumulatedCurrency}";
        float sandManMax = (SummaryData.AccumulatedCurrency > Level.Quota ? SummaryData.AccumulatedCurrency : Level.Quota) * sliderMaxModifier;
        sandManSlider.maxValue = sandManMax;
        //float boogeyManMax = Level.BoogeyManQuota;
        boogeyManSlider.maxValue = sandManMax;
        sandManSlider.value = SummaryData.AccumulatedCurrency;
        float sliderWidth = ((RectTransform)sandManSlider.transform).rect.width;
        float percentage = Level.Quota / sandManMax;
        quotaLine.rectTransform.SetAnchoredPositionX(sliderWidth * percentage);
        boogeyManSlider.value = Level.BoogeyManQuota;
        if (LevelManager.Instance.PassQuota)
        {
            quotaStamp.gameObject.SetActive(true);
            GlobalSoundManager.Instance.PlayUISFX("QuotaStamp");
        }
        else
        {
            quotaStamp.gameObject.SetActive(false);
        }
    }
    
     private string Summary()
    {
        string summary = $"Order:\n\n";
        summary += $"Total Order: {SummaryData.TotalCompletedOrder + SummaryData.TotalRejectedOrder + SummaryData.TotalFailedOrder}\n";
        summary += $"Completed: {SummaryData.TotalCompletedOrder}\n";
        summary += $"Rejected: {SummaryData.TotalRejectedOrder}\n";
        summary += $"Failed: {SummaryData.TotalFailedOrder}\n";
        summary += "\nIngredient Used:\n\n";
        foreach (var ingredient in SummaryData.IngredientData)
        {
            summary += $"{ingredient.Key}: {ingredient.Value}\n";
        }
        return summary;
    }

     public bool Open()
    {
        if (!UIPageManager.Instance.FromGameplay) return false;
        summaryCanvasGroup.gameObject.SetActive(true);
        ShowSummary();
        return true;
    }

    public bool Close()
    {
        summaryCanvasGroup.gameObject.SetActive(false);
        return true;
    }

   
}
