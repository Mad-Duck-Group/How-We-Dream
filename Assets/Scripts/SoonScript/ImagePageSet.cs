using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePageSet : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    //public Sprite[] Images { get => images; set => images = value; }
    [SerializeField] private Image imagePage1;
    [SerializeField] private Image imagePage2;

    private int _imageIndex;
    //public int ImageIndex { get => _imageIndex; set => _imageIndex = value; }

    public void UpdateImageManualPage()
    {
        imagePage1.sprite = images[_imageIndex];
        imagePage2.sprite = images[_imageIndex + 1];

        if (images.Length <= 2)
        {
            Manual.Instance.UpdateChangePageButton(false, false);
            return;
        }
        if (_imageIndex >= images.Length - 2)
        {
            Manual.Instance.UpdateChangePageButton(true, false);
            return;
        }
        if (_imageIndex <= 0)
        {
            Manual.Instance.UpdateChangePageButton(false, true);
            return;
        }
        Manual.Instance.UpdateChangePageButton(true, true);
    }
    
    public void NextImage()
    {
        if (_imageIndex < images.Length - 2)
        {
            _imageIndex += 2;
            UpdateImageManualPage();
        }
    }
    
    public void PreviousImage()
    {
        if (_imageIndex > 0)
        {
            _imageIndex -= 2;
            UpdateImageManualPage();
        }
    }
}
