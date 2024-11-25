using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Manual : MonoSingleton<Manual>
{

    [Header("ClickableArea Buttons")]
    [SerializeField] private ClickableArea manualButton;
    [SerializeField] private ClickableArea closeManualButton;
    [SerializeField] private ClickableArea nextPageButton;
    [SerializeField] private ClickableArea previousPageButton;
    [SerializeField] private ClickableArea topic1Button;
    [SerializeField] private ClickableArea topic2Button;
    [SerializeField] private ClickableArea topic3Button;
    
    [SerializeField] private CanvasGroup manualPanel;
    
    // GameObjects for manual topic sets
    [Header("Manual Topic Sets")]
    [SerializeField] private GameObject manualTopicSet1;
    [SerializeField] private GameObject manualTopicSet2;
    [SerializeField] private GameObject manualTopicSet3;
    
    [Header("Next and Previous Buttons Images")]
    [SerializeField] private Image nextPageImage;
    [SerializeField] private Image previousPageImage;
    public Image NextPageImage => nextPageImage;
    public Image PreviousPageImage => previousPageImage;
    
    // Images for topics
    [Header("Topic Images")]
    [SerializeField] private Image topicImage1;
    [SerializeField] private Image topicImage2;
    [SerializeField] private Image topicImage3;
    private int topicIndex;
    
    // Images on manual
    [Header("Images Manual")]
    [SerializeField] private ImagePageSet imagePage;
    [SerializeField] private ImagePageSet imagePage2;
    [SerializeField] private ImagePageSet imagePage3;

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
        topic1Button.OnClickEvent += Topic1;
        topic2Button.OnClickEvent += Topic2;
        topic3Button.OnClickEvent += Topic3;
        LevelManager.OnLevelComplete += OnCloseManualButtonClick;
    }
    
    private void OnDisable()
    {
        manualButton.OnClickEvent -= OnManualButtonClick;
        closeManualButton.OnClickEvent -= OnCloseManualButtonClick;
        nextPageButton.OnClickEvent -= OnNextPageButtonClick;
        previousPageButton.OnClickEvent -= OnPreviousPageButtonClick;
        topic1Button.OnClickEvent -= Topic1;
        topic2Button.OnClickEvent -= Topic2;
        topic3Button.OnClickEvent -= Topic3;
        LevelManager.OnLevelComplete -= OnCloseManualButtonClick;
    }

    private void OnManualButtonClick()
    {
        manualPanel.gameObject.SetActive(true);
        if (_fadeTween.IsActive()) _fadeTween.Kill();
        manualPanel.alpha = 0;
        _fadeTween = manualPanel.DOFade(1, 0.5f);
        
        //Update
        UpdateTopicPage();
        switch (topicIndex)
        {
            case 0 :
                imagePage.UpdateImageManualPage();
                break;
            case 1 :
                imagePage2.UpdateImageManualPage();
                break;
            case 2 :
                //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
                //textPage.UpdateManualPage();
                imagePage3.UpdateImageManualPage();
                break;
        }
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
        if (manualTopicSet1.activeSelf)
        {
            imagePage.NextImage();
            imagePage.UpdateImageManualPage();
        }

        if (manualTopicSet2.activeSelf)
        {
            imagePage2.NextImage();
            imagePage2.UpdateImageManualPage();
        }

        if (manualTopicSet3.activeSelf)
        {
            //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
            /*
            textPage.NextPage();
            textPage.UpdateManualPage();
            _textIndex = textPage.TextIndex;
            */
            imagePage3.NextImage();
            imagePage3.UpdateImageManualPage();
        }
    }
    
    // Set the previous page button
    private void OnPreviousPageButtonClick()
    {
        if (manualTopicSet1.activeSelf)
        {
            imagePage.PreviousImage();
            imagePage.UpdateImageManualPage();
        }

        if (manualTopicSet2.activeSelf)
        {
            imagePage2.PreviousImage();
            imagePage2.UpdateImageManualPage();
        }

        if (manualTopicSet3.activeSelf)
        {
            //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
            /*
            textPage.PreviousPage();
            textPage.UpdateManualPage();
            _textIndex = textPage.TextIndex;
            */
            imagePage3.PreviousImage();
            imagePage3.UpdateImageManualPage();
        }

    }
    
    private void Topic1()
    {
        topicIndex = 0;
        UpdateTopicPage();
        imagePage.UpdateImageManualPage();
    }
    
    private void Topic2()
    {
        topicIndex = 1;
        UpdateTopicPage();
        imagePage2.UpdateImageManualPage();
    }
    
    private void Topic3()
    {
        topicIndex = 2;
        UpdateTopicPage();
        //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
        //textPage.UpdateManualPage();
        imagePage3.UpdateImageManualPage();
    }
    
    private void UpdateTopicPage()
    {
        topicImage1.color = topicShaderColor;
        topicImage2.color = topicShaderColor;
        topicImage3.color = topicShaderColor;
        manualTopicSet1.SetActive(false);
        manualTopicSet2.SetActive(false);
        manualTopicSet3.SetActive(false);
        switch (topicIndex)
        {
            case 0 :
                topicImage1.color = Color.white;
                manualTopicSet1.SetActive(true);
                break;
            
            case 1 :
                topicImage2.color = Color.white;
                manualTopicSet2.SetActive(true);
                break;
            
            case 2 :
                topicImage3.color = Color.white;
                manualTopicSet3.SetActive(true);
                break;
        }
    }
}
