using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manual : MonoBehaviour
{
    [SerializeField] private ClickableArea manualButton;
    [SerializeField] private ClickableArea closeManualButton;
    [SerializeField] private Image manualImage;

    
    private void OnEnable()
    {
        manualButton.OnClickEvent += OnManualButtonClick;
        closeManualButton.OnClickEvent += OnCloseManualButtonClick;
    }
    
    private void OnDisable()
    {
        manualButton.OnClickEvent -= OnManualButtonClick;
        closeManualButton.OnClickEvent -= OnCloseManualButtonClick;
    }
    
    private void OnManualButtonClick()
    {
        Debug.Log("Manual button clicked!");
        //closeManualButton.gameObject.SetActive(true);
        manualImage.gameObject.SetActive(true);
    }
    
    private void OnCloseManualButtonClick()
    {
        Debug.Log("Close manual button clicked!");
        //closeManualButton.gameObject.SetActive(false);
        manualImage.gameObject.SetActive(false);
    }
}
