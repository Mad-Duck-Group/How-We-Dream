using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using TMPro;
using TNRD;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public interface IUIPage
{
    void Initialize();
    bool Open();
    bool Close();
}
public enum PageTypes
{
    Null,
    Summary,
    BusinessPlan,
    Shop
}
public class UIPageManager : PersistentMonoSingleton<UIPageManager>
{
    [SerializedDictionary("Page Type", "Page")] 
    public SerializedDictionary<PageTypes, SerializableInterface<IUIPage>> pageDictionary;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField, ReadOnly] private PageTypes currentPage = PageTypes.Null;

    private bool fromGameplay;
    public bool FromGameplay => fromGameplay;
    private bool PassQuota => LevelManager.Instance.PassQuota;


    private void OnEnable()
    {
        SceneManagerPersistent.OnFinishFadeOut += OnFinishFadeOut;
    }
    
    private void OnDisable()
    {
        SceneManagerPersistent.OnFinishFadeOut -= OnFinishFadeOut;
    }
    
    private void OnFinishFadeOut()
    {
        ChangePage(PageTypes.Null, fromGameplay);
    }

    private void Start()
    {
        foreach (var page in pageDictionary.Values)
        {
            page.Value.Initialize();
        }
        nextButton.onClick.AddListener(Next);
        backButton.onClick.AddListener(Back);
        nextButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    private void Next()
    {
        switch (currentPage)
        {
            case PageTypes.Summary:
                ChangePage(PageTypes.BusinessPlan, fromGameplay);
                break;
            case PageTypes.BusinessPlan:
                ChangePage(PageTypes.Shop, fromGameplay);
                break;   
            case PageTypes.Shop when fromGameplay && !PassQuota:
                //ChangePage(PageTypes.Null, fromGameplay);
                ProgressionManager.Instance.ReplayLevel();
                break;
            case PageTypes.Shop when fromGameplay && PassQuota:
                //ChangePage(PageTypes.Null, fromGameplay);
                ProgressionManager.Instance.NextLevel();
                break;
            case PageTypes.Shop:
                ChangePage(PageTypes.Null, fromGameplay);
                break;
        }
    }
    
    private void Back()
    {
        switch (currentPage)
        {
            case PageTypes.Shop:
                ChangePage(PageTypes.BusinessPlan, fromGameplay);
                break;
            case PageTypes.BusinessPlan:
                ChangePage(PageTypes.Summary, fromGameplay);
                break;
        }
    }
    
    public void ChangePage(PageTypes page, bool fromGameplay = true)
    {
        this.fromGameplay = fromGameplay;
        if (currentPage != PageTypes.Null) 
            pageDictionary[currentPage].Value.Close();
        if (page == PageTypes.Null)
        {
            currentPage = PageTypes.Null;
            nextButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            return;
        }
        bool openSuccessful = pageDictionary[page].Value.Open();
        if (openSuccessful) currentPage = page;
        SetActiveButton(backButton, currentPage != PageTypes.Summary);
        nextButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }
    
    private void SetActiveButton(Button button, bool active)
    {
        button.interactable = active;
    }
}
