using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePageSet : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    public Sprite[] Images { get => images; set => images = value; }
    [SerializeField] private Image imagePage1;
    [SerializeField] private Image imagePage2;
    private Sprite currentImage1;
    private Sprite currentImage2;
    
    private int imageIndex;
    public int ImageIndex { get => imageIndex; set => imageIndex = value; }
    public int ImageManualIndex => images.Length;
    
    public void UpdateImageManualPage()
    {
        currentImage1 = images[imageIndex];
        currentImage2 = images[imageIndex + 1];
        
        imagePage1.sprite = currentImage1;
        imagePage2.sprite = currentImage2;
    }
}
