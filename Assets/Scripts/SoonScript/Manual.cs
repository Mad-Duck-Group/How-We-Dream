using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Manual : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private ClickableArea manualButton;
    [SerializeField] private ClickableArea closeManualButton;
    [SerializeField] private ClickableArea nextPageButton;
    [SerializeField] private ClickableArea previousPageButton;
    [SerializeField] private ClickableArea topic1Button;
    [SerializeField] private ClickableArea topic2Button;
    [SerializeField] private ClickableArea topic3Button;
    
    [FormerlySerializedAs("manualImage")] [SerializeField] private Image manualPanel;
    private int manualIndex;
    [SerializeField] private GameObject manualTopicSet1;
    [SerializeField] private GameObject manualTopicSet2;
    [SerializeField] private GameObject manualTopicSet3;
    
    [Header("Images")]
    [SerializeField] private Image topicImage1;
    [SerializeField] private Image topicImage2;
    [SerializeField] private Image topicImage3;
    private int topicIndex;
    
    [SerializeField] private ImagePageSet imagePage;
    [SerializeField] private ImagePageSet imagePage2;
    private int imageManualIndex;
    private int imageManualIndex2;

    private void Awake() 
    {
        imageManualIndex = imagePage.ImageManualIndex;
        imageManualIndex2 = imagePage2.ImageManualIndex;
    }
    
    
    
    [Header("Texts")]
    [SerializeField] private TMP_Text nameTypeTextPage1;
    [SerializeField] private TMP_Text nameTypeTextPage2;
    [SerializeField] private TMP_Text descriptionText;
    
    private DreamTypes currentDream1;
    private DreamTypes currentDream2;
    private DreamTypes[] dreams = (DreamTypes[])System.Enum.GetValues(typeof(DreamTypes)); 
    private int dreamManualIndex = System.Enum.GetValues(typeof(DreamTypes)).Length;
    [SerializeField] private DreamSO[] dreamSO;
    [SerializeField] private CookStates requiredCookState;
    
    [SerializeField] private TMP_Text[] ingredientTextsPage1;
    [SerializeField] private TMP_Text[] ingredientTextsPage2;

    private void Update()
    {
        imagePage.UpdateImageManualPage();
        imagePage2.UpdateImageManualPage();
        
        Debug.Log("ImageIndex2 " + imagePage2.ImageIndex);
        Debug.Log("ImageIndex " + imagePage.ImageIndex);
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
        manualIndex = 0;
        
        
        //Update
        UpdateManualPage();
        imagePage.UpdateImageManualPage();
        imagePage2.UpdateImageManualPage();
    }
    
    private void OnCloseManualButtonClick()
    {
        Debug.Log("Close manual button clicked!");
        manualPanel.gameObject.SetActive(false);
    }
    
    private void OnNextPageButtonClick()
    {
        if (!manualTopicSet1.activeSelf)
            return;
        imagePage.ImageIndex += 2;
        if (imagePage.ImageIndex >= imageManualIndex)
        {
            imagePage.ImageIndex = 0;
        }
        
        if (!manualTopicSet2.activeSelf)
            return;
        imagePage2.ImageIndex += 2;
        if (imagePage2.ImageIndex >= imageManualIndex2)
        {
            imagePage2.ImageIndex = 0;
        }
        
        
        if (!manualTopicSet3.activeSelf)
            return;
        manualIndex += 2;
        if (manualIndex >= dreamManualIndex)
        {
            manualIndex = 0;
        }
        
        //Update
        UpdateManualPage();
        imagePage.UpdateImageManualPage();
        imagePage2.UpdateImageManualPage();
        
        //Debug
        Debug.Log(dreams[manualIndex]);
        Debug.Log(manualIndex);
        Debug.Log("ImageIndex2 " + imagePage2.ImageIndex);
        Debug.Log("ImageIndex " + imagePage.ImageIndex);
    }
    
    private void OnPreviousPageButtonClick()
    {
        if (!manualTopicSet1.activeSelf)
            return;
        imagePage.ImageIndex -= 2;
        if (imagePage.ImageIndex < 0)
        {
            imagePage.ImageIndex = imageManualIndex - 1;
        }
        
        if (!manualTopicSet2.activeSelf)
            return;
        imagePage2.ImageIndex -= 2;
        if (imagePage2.ImageIndex < 0)
        {
            imagePage2.ImageIndex = imageManualIndex2 - 1;
        }
        
        if (!manualTopicSet3.activeSelf)
            return;
        manualIndex -= 2;
        if (manualIndex < 0)
        {
            manualIndex = dreamManualIndex - 1;
        }
        
        //Update
        UpdateManualPage();
        imagePage.UpdateImageManualPage();
        imagePage2.UpdateImageManualPage();
        
        //Debug
        Debug.Log(dreams[manualIndex]);
        Debug.Log(manualIndex);
    }
    
    private void Topic1()
    {
        topicIndex = 1;
        UpdateTopicPage();
    }
    
    private void Topic2()
    {
        topicIndex = 2;
        UpdateTopicPage();
    }
    
    private void Topic3()
    {
        topicIndex = 3;
        UpdateTopicPage();
    }
    
    // Set Topic image color here!!!
    private void UpdateTopicPage()
    {
        switch (topicIndex)
        {
            case 1 :
                manualTopicSet1.SetActive(true);
                manualTopicSet2.SetActive(false);
                manualTopicSet3.SetActive(false);
                break;
            case 2 :
                manualTopicSet1.SetActive(false);
                manualTopicSet2.SetActive(true);
                manualTopicSet3.SetActive(false);
                break;
            case 3 :
                manualTopicSet1.SetActive(false);
                manualTopicSet2.SetActive(false);
                manualTopicSet3.SetActive(true);
                break;
        }
    }
    
    private void UpdateManualPage()
    {
        currentDream1 = dreams[manualIndex];
        currentDream2 = dreams[manualIndex + 1];
        nameTypeTextPage1.text = currentDream1.ToString();
        nameTypeTextPage2.text = currentDream2.ToString();
        descriptionText.text = requiredCookState.ToString();
        //manualImage.sprite = currentDream.GetSprite();
    }
    
    private void MafiaMethod()
    {
        DreamSO test = RecipeManager.Instance.dreamData[DreamTypes.Lucid]; //ส่ง type เข้าไป
        CookStates test2 = test.IngredientData[IngredientTypes.Truth]; //ส่ง type เข้าไป
        
    }
}
