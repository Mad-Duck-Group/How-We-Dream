using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour, IUIPage
{
    [SerializeField] private CanvasGroup skillTreeCanvasGroup;
    [SerializeField] private TMP_Text skillPointsText;
    // Start is called before the first frame update
    
    private void OnEnable()
    {
        ProgressionManager.OnSkillPointChanged += OnSkillPointChanged;
        OnSkillPointChanged(ProgressionManager.Instance.SkillPoints);
    }
    
    private void OnDisable()
    {
        ProgressionManager.OnSkillPointChanged -= OnSkillPointChanged;
    }

    private void OnSkillPointChanged(int points)
    {
        skillPointsText.text = $"Skill Points: {points}";
    }

    public void Initialize()
    {
        skillTreeCanvasGroup.gameObject.SetActive(true);
        skillTreeCanvasGroup.gameObject.SetActive(false);
    }
    
    public bool Open()
    {
        skillTreeCanvasGroup.gameObject.SetActive(true);
        return true;
    }

    public bool Close()
    {
        skillTreeCanvasGroup.gameObject.SetActive(false);
        return true;
    }
}
