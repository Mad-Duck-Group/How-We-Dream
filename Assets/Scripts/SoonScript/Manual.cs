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
    [SerializeField] private GameObject[] manualTopicSet;

    
    [Header("Next and Previous Buttons Images")]
    [SerializeField] private Image nextPageImage;
    [SerializeField] private Image previousPageImage;
    public Image NextPageImage => nextPageImage;
    public Image PreviousPageImage => previousPageImage;
    
    // Images for topics
    [Header("Topic Images")]
    [SerializeField] private Image[] topicImage;
    private int topicIndex = 0;
    
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
        imagePage[topicIndex].UpdateImageManualPage();
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
        if (manualTopicSet[0].activeSelf)
        {
            imagePage[0].NextImage();
            imagePage[0].UpdateImageManualPage();
        }

        if (manualTopicSet[1].activeSelf)
        {
            imagePage[1].NextImage();
            imagePage[1].UpdateImageManualPage();
        }

        if (manualTopicSet[2].activeSelf)
        {
            //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
            /*
            textPage.NextPage();
            textPage.UpdateManualPage();
            _textIndex = textPage.TextIndex;
            */
            imagePage[2].NextImage();
            imagePage[2].UpdateImageManualPage();
        }
    }
    
    // Set the previous page button
    private void OnPreviousPageButtonClick()
    {
        if (manualTopicSet[0].activeSelf)
        {
            imagePage[0].PreviousImage();
            imagePage[0].UpdateImageManualPage();
        }

        if (manualTopicSet[1].activeSelf)
        {
            imagePage[1].PreviousImage();
            imagePage[1].UpdateImageManualPage();
        }

        if (manualTopicSet[2].activeSelf)
        {
            //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
            /*
            textPage.PreviousPage();
            textPage.UpdateManualPage();
            _textIndex = textPage.TextIndex;
            */
            imagePage[2].PreviousImage();
            imagePage[2].UpdateImageManualPage();
        }

    }
    
    private void Topic1()
    {
        topicIndex = 0;
        UpdateTopicPage();
        imagePage[0].UpdateImageManualPage();
    }
    
    private void Topic2()
    {
        topicIndex = 1;
        UpdateTopicPage();
        imagePage[1].UpdateImageManualPage();
    }
    
    private void Topic3()
    {
        topicIndex = 2;
        UpdateTopicPage();
        //เปลี่ยนด้วย ว่าจะให้เป็น text หรือ image!!!
        //textPage.UpdateManualPage();
        imagePage[2].UpdateImageManualPage();
    }
    
    private void UpdateTopicPage()
    {
        topicImage[0].color = topicShaderColor;
        topicImage[1].color = topicShaderColor;
        topicImage[2].color = topicShaderColor;
        manualTopicSet[0].SetActive(false);
        manualTopicSet[1].SetActive(false);
        manualTopicSet[2].SetActive(false);
        switch (topicIndex)
        {
            case 0 :
                topicImage[0].color = Color.white;
                manualTopicSet[0].SetActive(true);
                break;
            
            case 1 :
                topicImage[1].color = Color.white;
                manualTopicSet[1].SetActive(true);
                break;
            
            case 2 :
                topicImage[2].color = Color.white;
                manualTopicSet[2].SetActive(true);
                break;
        }
    }
}
