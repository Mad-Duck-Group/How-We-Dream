using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Manual : MonoBehaviour
{
    public static Manual Instance;
    
    [Header("ClickableArea Buttons")]
    [SerializeField] private ClickableArea manualButton;
    [SerializeField] private ClickableArea closeManualButton;
    [SerializeField] private ClickableArea nextPageButton;
    [SerializeField] private ClickableArea previousPageButton;
    [SerializeField] private ClickableArea topic1Button;
    [SerializeField] private ClickableArea topic2Button;
    [SerializeField] private ClickableArea topic3Button;
    
    [SerializeField] private Image manualPanel;
    
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
    private int _imageIndex;
    private int _imageIndex2;
    private int maxImageIndex;
    private int maxImageIndex2;
    
    // Texts on manual
    [Header("Texts Manual")]
    [SerializeField] private TextPageSet textPage;
    private int _textIndex;
    private int maxTextIndex;
    
    [Header("Colors")]
    [SerializeField] private Color topicShaderColor;
    public Color TopicShaderColor => topicShaderColor;

    private void Awake()
    {
        Instance = this;
        
        maxImageIndex = imagePage.Images.Length;
        maxImageIndex2 = imagePage2.Images.Length;
        
        maxTextIndex = textPage.Dreams.Length;
    }

    private void OnEnable()
    {
        manualButton.OnClickEvent += OnManualButtonClick;
        closeManualButton.OnClickEvent += OnCloseManualButtonClick;
        nextPageButton.OnClickEvent += OnNextPageButtonClick;
        previousPageButton.OnClickEvent += OnPreviousPageButtonClick;
        topic1Button.OnClickEvent += Topic1;
        topic2Button.OnClickEvent += Topic2;
        topic3Button.OnClickEvent += Topic3;
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
    }
    
    private void OnManualButtonClick()
    {
        Debug.Log("Manual button clicked!");
        manualPanel.gameObject.SetActive(true);
        
        //Update
        UpdateTopicPage();
        imagePage.UpdateImageManualPage();
        imagePage2.UpdateImageManualPage();
        textPage.UpdateManualPage();
    }
    
    private void OnCloseManualButtonClick()
    {
        Debug.Log("Close manual button clicked!");
        manualPanel.gameObject.SetActive(false);
    }
    
    // Set the next page button
    private void OnNextPageButtonClick()
    {
        if (manualTopicSet1.activeSelf)
        {
            imagePage.NextImage();
            imagePage.UpdateImageManualPage();
            _imageIndex = imagePage.ImageIndex;
        }

        if (manualTopicSet2.activeSelf)
        {
            imagePage2.NextImage();
            imagePage2.UpdateImageManualPage();
            _imageIndex2 = imagePage2.ImageIndex;
        }

        if (manualTopicSet3.activeSelf)
        {
            textPage.NextPage();
            textPage.UpdateManualPage();
            _textIndex = textPage.TextIndex;
        }
    }
    
    // Set the previous page button
    private void OnPreviousPageButtonClick()
    {
        if (manualTopicSet1.activeSelf)
        {
            imagePage.PreviousImage();
            imagePage.UpdateImageManualPage();
            _imageIndex = imagePage.ImageIndex;
        }

        if (manualTopicSet2.activeSelf)
        {
            imagePage2.PreviousImage();
            imagePage2.UpdateImageManualPage();
            _imageIndex2 = imagePage2.ImageIndex;
        }

        if (manualTopicSet3.activeSelf)
        {
            textPage.PreviousPage();
            textPage.UpdateManualPage();
            _textIndex = textPage.TextIndex;
        }

    }
    
    private void Topic1()
    {
        topicIndex = 1;
        UpdateTopicPage();
        imagePage.UpdateImageManualPage();
    }
    
    private void Topic2()
    {
        topicIndex = 2;
        UpdateTopicPage();
        imagePage2.UpdateImageManualPage();
    }
    
    private void Topic3()
    {
        topicIndex = 3;
        UpdateTopicPage();
        textPage.UpdateManualPage();
    }
    
    private void UpdateTopicPage()
    {
        switch (topicIndex)
        {
            case 1 :
                topicImage1.color = Color.white;
                manualTopicSet1.SetActive(true);
                
                topicImage2.color = topicShaderColor;
                manualTopicSet2.SetActive(false);
                
                topicImage3.color = topicShaderColor;
                manualTopicSet3.SetActive(false);
                break;
            
            case 2 :
                topicImage1.color = topicShaderColor;
                manualTopicSet1.SetActive(false);
                
                topicImage2.color = Color.white;
                manualTopicSet2.SetActive(true);
                
                topicImage3.color = topicShaderColor;
                manualTopicSet3.SetActive(false);
                break;
            
            case 3 :
                topicImage1.color = topicShaderColor;
                manualTopicSet1.SetActive(false);
                
                topicImage2.color = topicShaderColor;
                manualTopicSet2.SetActive(false);
                
                topicImage3.color = Color.white;
                manualTopicSet3.SetActive(true);
                break;
        }
    }
    
    void Update()
    {
        Debug.Log("Image Index: " + _imageIndex);
        Debug.Log("Image Index 2: " + _imageIndex2);
        Debug.Log("Text Index: " + _textIndex);
    }
}
