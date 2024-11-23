using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPageSet : MonoBehaviour
{
    private int _textIndex;
    public int TextIndex { get => _textIndex; set => _textIndex = value; }
    
    [Header("Texts")]
    [SerializeField] private TMP_Text nameTypeTextPage1;
    [SerializeField] private TMP_Text nameTypeTextPage2;
    [SerializeField] private TMP_Text descriptionText;
    
    private DreamTypes currentDream1;
    private DreamTypes currentDream2;
    private DreamTypes[] dreams = (DreamTypes[])System.Enum.GetValues(typeof(DreamTypes)); 
    private int dreamManualIndex = System.Enum.GetValues(typeof(DreamTypes)).Length;
    private DreamSO[] dreamSO;
    [SerializeField] private CookStates requiredCookState;
    
    [Header("Ingredients")] 
    [SerializeField] private TMP_Text[] ingredientTextsPage1;
    [SerializeField] private TMP_Text[] ingredientTextsPage2;
    
    public void UpdateManualPage()
    {
        currentDream1 = dreams[_textIndex];
        currentDream2 = dreams[_textIndex + 1];
        nameTypeTextPage1.text = currentDream1.ToString();
        nameTypeTextPage2.text = currentDream2.ToString();
        descriptionText.text = requiredCookState.ToString();
        //manualImage.sprite = currentDream.GetSprite();
    }
    
    public void NextPage()
    {
        if (_textIndex < dreamManualIndex - 2)
        {
            _textIndex += 2;
            UpdateManualPage();
        }
    }
    
    public void PreviousPage()
    {
        if (_textIndex > 0)
        {
            _textIndex -= 2;
            UpdateManualPage();
        }
    }
    
    private void MafiaMethod()
    {
        DreamSO test = RecipeManager.Instance.dreamData[DreamTypes.Lucid]; //ส่ง type เข้าไป
        CookStates test2 = test.IngredientData[IngredientTypes.Truth]; //ส่ง type เข้าไป
    }
}
