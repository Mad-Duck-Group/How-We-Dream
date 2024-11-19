using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour, IUIPage
{
    [SerializeField] private CanvasGroup skillTreeCanvasGroup;
    // Start is called before the first frame update
    public void Initialize()
    {
        skillTreeCanvasGroup.gameObject.SetActive(true);
        skillTreeCanvasGroup.gameObject.SetActive(false);
    }
    
    public bool Open()
    {
        Debug.Log("Open Skill Tree");
        skillTreeCanvasGroup.gameObject.SetActive(true);
        return true;
    }

    public bool Close()
    {
        Debug.Log("Close Skill Tree");
        skillTreeCanvasGroup.gameObject.SetActive(false);
        return true;
    }
}
