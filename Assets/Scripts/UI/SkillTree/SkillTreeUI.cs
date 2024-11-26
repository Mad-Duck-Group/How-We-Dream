using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour, IUIPage
{
    [SerializeField] private CanvasGroup skillTreeCanvasGroup;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private ScrollRect skillTreeScrollRect;
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
        skillTreeScrollRect.normalizedPosition = new Vector2(0, 0);
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
