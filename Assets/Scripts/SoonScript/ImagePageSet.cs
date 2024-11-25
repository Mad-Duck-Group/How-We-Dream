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
    
    private Image nextPageImage;
    private Image previousPageImage;
    
    private Color topicShaderColor;

    private void Awake()
    {
        nextPageImage = Manual.Instance.NextPageImage;
        previousPageImage = Manual.Instance.PreviousPageImage;
        topicShaderColor = Manual.Instance.TopicShaderColor;
    }

    public void UpdateImageManualPage()
    {
        imagePage1.sprite = images[_imageIndex];
        imagePage2.sprite = images[_imageIndex + 1];
        
        if (_imageIndex >= images.Length - 2)
        {
            nextPageImage.color = topicShaderColor;
            previousPageImage.color = Color.white;
        }
        else if (_imageIndex <= 0)
        {
            nextPageImage.color = Color.white;
            previousPageImage.color = topicShaderColor;
        }
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
