using System;
using System.Collections;
using System.Collections.Generic;
using Microlight.MicroAudio;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VolumeControlUI : MonoBehaviour
{
    [SerializeField] private Button volumeButton;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unmutedSprite;
    [SerializeField] private Slider volumeSlider;
    private bool isMuted;
    private Image volumeButtonImage;

    private void Awake()
    {
        volumeButtonImage = volumeButton.GetComponent<Image>();
    }

    void Start()
    {
        volumeSlider.value = MicroAudio.MasterVolume;
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        volumeButton.onClick.AddListener(ToggleMute);
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        volumeButtonImage.sprite = isMuted ? mutedSprite : unmutedSprite;
        volumeSlider.gameObject.SetActive(!isMuted);
        MicroAudio.MasterVolume = isMuted ? 0 : volumeSlider.value;
    }

    private void ChangeVolume(float volume)
    {
        MicroAudio.MasterVolume = volume;
    }
    
}
