using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Redcode.Extensions;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ManualManager : MonoSingleton<ManualManager>
{
    [Header("ClickableArea Buttons")]
    [SerializeField] private ClickableArea manualButton;
    [SerializeField] private ClickableArea closeManualButton;
    [SerializeField] private ClickableArea nextPageButton;
    [SerializeField] private ClickableArea previousPageButton;
    [SerializeField] private ClickableArea[] topicButtons;
    [SerializeField] private CanvasGroup manualPanel;
    [SerializedDictionary("Clickable Area", "Sprite")]
    public SerializedDictionary<ClickableArea, Sprite[]> topicSpriteDictionary;
    
    // GameObjects for manual topic sets
    [Header("Manual Topic Sets")]
    [SerializeField] private GameObject[] manualTopicSet;

    [Header("Next and Previous Buttons Images")]
    [SerializeField] private Image nextPageImage;
    [SerializeField] private Image previousPageImage;
    [SerializedDictionary("Image", "Sprite")]
    public SerializedDictionary<Image, Sprite[]> changePageSpriteDictionary;

    // Images for topics
    [Header("Topic Images")]
    [SerializeField] private Image[] topicImage;
    private int topicIndex;
    
    // Images on manual
    [Header("Images Manual")]
    [SerializeField] private ImagePageSet[] imagePage;

    // Texts on manual
    [Header("Texts Manual")]
    [SerializeField] private TextPageSet textPage;
    private int _textIndex;
    private int maxTextIndex;
    
    [Header("Colors")]
    [SerializeField] private Color topicShaderColor;
    public Color TopicShaderColor => topicShaderColor;
    
    private Tween _fadeTween;

    private void OnEnable()
    {
        manualButton.OnClickEvent += OnManualButtonClick;
        closeManualButton.OnClickEvent += OnCloseManualButtonClick;
        nextPageButton.OnClickEvent += OnNextPageButtonClick;
        previousPageButton.OnClickEvent += OnPreviousPageButtonClick;
        for (var i = 0; i < topicButtons.Length; i++)
        {
            var topicButton = topicButtons[i];
            var index = i;
            topicButton.OnClickEvent += () => ChangeTopic(index);
        }
        LevelManager.OnLevelComplete += OnCloseManualButtonClick;
    }
    
    private void OnDisable()
    {
        manualButton.OnClickEvent -= OnManualButtonClick;
        closeManualButton.OnClickEvent -= OnCloseManualButtonClick;
        nextPageButton.OnClickEvent -= OnNextPageButtonClick;
        previousPageButton.OnClickEvent -= OnPreviousPageButtonClick;
        LevelManager.OnLevelComplete -= OnCloseManualButtonClick;
        foreach (var topicButton in topicButtons)
        {
            topicButton.UnsubscribeAll();
        }
    }

    private void Start()
    {
        manualPanel.gameObject.SetActive(false);
    }

    private void OnManualButtonClick()
    {
        manualPanel.gameObject.SetActive(true);
        if (_fadeTween.IsActive()) _fadeTween.Kill();
        manualPanel.alpha = 0;
        _fadeTween = manualPanel.DOFade(1, 0.5f);
        
        //Update
        UpdateTopicPage();
    }
    
    private void OnCloseManualButtonClick()
    {
        Debug.Log("Close manual button clicked!");
        if (_fadeTween.IsActive()) _fadeTween.Kill();
        manualPanel.alpha = 1;
        _fadeTween = manualPanel.DOFade(0f, 0.5f).OnComplete(() => manualPanel.gameObject.SetActive(false));
    }
    
    // Set the next page button
    private void OnNextPageButtonClick()
    {
        for (var i = 0; i < manualTopicSet.Length; i++)
        {
            var manualTopic = manualTopicSet[i];
            if (!manualTopic.activeSelf) continue;
            imagePage[i].NextImage();
            GlobalSoundManager.Instance.PlayUISFX("OpenOrder");
            //imagePage[i].UpdateImageManualPage();
        }
    }
    
    // Set the previous page button
    private void OnPreviousPageButtonClick()
    {
        for (var i = 0; i < manualTopicSet.Length; i++)
        {
            var manualTopic = manualTopicSet[i];
            if (!manualTopic.activeSelf) continue;
            imagePage[i].PreviousImage();
            GlobalSoundManager.Instance.PlayUISFX("OpenOrder");
            //imagePage[i].UpdateImageManualPage();
        }
    }

    public void UpdateChangePageButton(bool previousActive, bool nextActive)
    {
        var previousSprites = changePageSpriteDictionary[previousPageImage];
        previousPageImage.sprite = previousSprites[previousActive ? 1 : 0];
        previousPageButton.Interactable = previousActive;
        var nextSprites = changePageSpriteDictionary[nextPageImage];
        nextPageImage.sprite = nextSprites[nextActive ? 1 : 0];
        nextPageButton.Interactable = nextActive;
    }

    private void ChangeTopic(int index)
    {
        topicIndex = index;
        UpdateTopicPage();
    }

    private void UpdateTopicPage()
    {
        for (int i = 0; i < topicImage.Length; i++)
        {
            var topic = topicImage[i];
            var sprite = topicSpriteDictionary[topicButtons[i]];
            topic.sprite = sprite[i == topicIndex ? 1 : 0];
        }
        manualTopicSet.ForEach(x => x.SetActive(false));
        manualTopicSet[topicIndex].SetActive(true);
        imagePage[topicIndex].UpdateImageManualPage();
    }
}
