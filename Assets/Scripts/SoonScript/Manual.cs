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
    
    [SerializeField] private Image manualImage;
    
    private int manualIndex;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text nameTypeText;
    [SerializeField] private TMP_Text descriptionText;
    
    private DreamTypes currentDream;
    private DreamTypes[] dreams = (DreamTypes[])System.Enum.GetValues(typeof(DreamTypes)); 
    private int dreamManualIndex = System.Enum.GetValues(typeof(DreamTypes)).Length;
    [SerializeField] private DreamSO[] dreamSO;
    [SerializeField] private CookStates requiredCookState;
    
    [SerializeField] private TMP_Text[] ingredientTexts;


    private void OnEnable()
    {
        manualButton.OnClickEvent += OnManualButtonClick;
        closeManualButton.OnClickEvent += OnCloseManualButtonClick;
        nextPageButton.OnClickEvent += OnNextPageButtonClick;
        previousPageButton.OnClickEvent += OnPreviousPageButtonClick;
    }
    
    private void OnDisable()
    {
        manualButton.OnClickEvent -= OnManualButtonClick;
        closeManualButton.OnClickEvent -= OnCloseManualButtonClick;
        nextPageButton.OnClickEvent -= OnNextPageButtonClick;
        previousPageButton.OnClickEvent -= OnPreviousPageButtonClick;
    }
    
    private void OnManualButtonClick()
    {
        Debug.Log("Manual button clicked!");
        //closeManualButton.gameObject.SetActive(true);
        manualImage.gameObject.SetActive(true);
        manualIndex = 0;
        UpdateManualPage();
    }
    
    private void OnCloseManualButtonClick()
    {
        Debug.Log("Close manual button clicked!");
        //closeManualButton.gameObject.SetActive(false);
        manualImage.gameObject.SetActive(false);
    }
    
    private void OnNextPageButtonClick()
    {
        manualIndex++;
        if (manualIndex >= dreamManualIndex)
        {
            manualIndex = 0;
        }
        Debug.Log(dreams[manualIndex]);
        UpdateManualPage();
        Debug.Log(manualIndex);
        //UpdateManualPage();
    }
    
    private void OnPreviousPageButtonClick()
    {
        manualIndex--;
        if (manualIndex < 0)
        {
            manualIndex = dreamManualIndex - 1;
        }
        Debug.Log(dreams[manualIndex]);
        UpdateManualPage();
        Debug.Log(manualIndex);
        //UpdateManualPage();
    }
    
    private void UpdateManualPage()
    {
        currentDream = dreams[manualIndex];
        nameTypeText.text = currentDream.ToString();
        descriptionText.text = requiredCookState.ToString();
        //manualImage.sprite = currentDream.GetSprite();
    }

    private void MafiaMethod()
    {
        DreamSO test = RecipeManager.Instance.dreamData[DreamTypes.Lucid]; //ส่ง type เข้าไป
        CookStates test2 = test.IngredientData[IngredientTypes.Truth]; //ส่ง type เข้าไป
        
    }
    
}
