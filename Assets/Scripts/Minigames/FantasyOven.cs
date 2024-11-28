using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Redcode.Extensions;
using Redcode.Moroutines;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FantasyOven : MonoBehaviour, IMinigame
{
    [SerializeField] private CanvasGroup minigameCanvasGroup; 
    [SerializeField] private Image[] lights;
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderHandle;
    [SerializeField] private RectTransform hitZone;
    [SerializeField] private ClickableArea knob;
    [SerializeField] private bool randomHitZoneSize;
    [SerializeField, HideIf(nameof(randomHitZoneSize))] private float hitZoneSize;
    [SerializeField, ShowIf(nameof(randomHitZoneSize))] private Vector2 hitZoneSizeRandomRange;
    [SerializeField] private bool randomHitZonePosition;
    [SerializeField, HideIf(nameof(randomHitZonePosition))] private float hitZonePosition;
    [SerializeField] private Vector2 padding;
    [SerializeField] private float barSpeed;
    [SerializeField] private float preparationTime = 2f;

    private int currentAttempt;
    private int gameEndThreshold;
    private int mistakes;
    private int success;
    private bool ready;
    private Moroutine minigameCoroutine;
    private Vector2 hitZoneRange;
    public event IMinigame.MinigameStart OnMinigameStart;
    public event IMinigame.MinigameEnd OnMinigameEnd;
    private void OnEnable()
    {
        knob.OnClickEvent += OnKnobClick;
    }
    
    private void OnDisable()
    {
        knob.OnClickEvent -= OnKnobClick;
    }

    private void Start()
    {
        minigameCanvasGroup.gameObject.SetActive(false);
    }
    
    public void Halt()
    {
        minigameCoroutine?.Stop();
        minigameCanvasGroup.gameObject.SetActive(false);
        minigameCoroutine?.Destroy();
    }
    
    public void StartMinigame()
    {
        gameEndThreshold = Mathf.CeilToInt(lights.Length / 2f);
        success = 0;
        mistakes = 0;
        currentAttempt = 0;
        lights.ForEach(x => x.color = Color.white);
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        minigameCoroutine = Moroutine.Run(gameObject, UpdateMinigame());
        minigameCoroutine.OnCompleted(_ => Fail());
        GlobalSoundManager.Instance.PlayUISFX("Stove", true, "Stove");
    }

    private void OnKnobClick()
    {
        if (minigameCoroutine == null) return;
        if (!minigameCoroutine.IsRunning) return;
        if (!ready) return;
        GlobalSoundManager.Instance.PlayUISFX("MinigameButton");
        if (slider.value < hitZoneRange.x || slider.value > hitZoneRange.y)
        {
           Fail();
        }
        else
        {
            Succeed();
        }
    }

    private IEnumerable UpdateMinigame()
    {
        GenerateHitZone();
        slider.value = slider.minValue;
        ready = false;
        sliderHandle.DOColor(Color.yellow, preparationTime / 6).SetLoops(6, LoopType.Yoyo);
        yield return new WaitForSeconds(preparationTime);
        ready = true;
        while (slider.value < slider.maxValue)
        {
            slider.value += barSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void Fail()
    {
        mistakes++;
        minigameCoroutine.Stop();
        lights[currentAttempt].DOColor(Color.red, 0.2f);
        if (mistakes >= gameEndThreshold)
        {
            OnMinigameEnd?.Invoke(false);
            minigameCanvasGroup.gameObject.SetActive(false);
            minigameCoroutine.Destroy();
            GlobalSoundManager.Instance.StopSound("Stove");
            return;
        }
        currentAttempt++;
        if (currentAttempt > lights.Length - 1)
        {
            OnMinigameEnd?.Invoke(true);
            minigameCanvasGroup.gameObject.SetActive(false);
            minigameCoroutine.Destroy();
            GlobalSoundManager.Instance.StopSound("Stove");
            return;
        }
        minigameCoroutine.Rerun();
    }

    private void Succeed()
    {
        success++;
        minigameCoroutine.Stop();
        lights[currentAttempt].DOColor(Color.green, 0.2f);
        currentAttempt++;
        if (currentAttempt > lights.Length - 1 || success >= gameEndThreshold)
        {
            OnMinigameEnd?.Invoke(true);
            minigameCanvasGroup.gameObject.SetActive(false);
            minigameCoroutine.Destroy();
            GlobalSoundManager.Instance.StopSound("Stove");
            return;
        }
        minigameCoroutine.Rerun();
    }
    private void GenerateHitZone()
    {
        if (randomHitZoneSize) hitZoneSize = Random.Range(hitZoneSizeRandomRange.x, hitZoneSizeRandomRange.y);
        if (randomHitZonePosition) hitZonePosition = Random.Range(slider.minValue + hitZoneSize / 2 + padding.x, slider.maxValue - hitZoneSize / 2 - padding.y);
        hitZoneRange = new Vector2(hitZonePosition - hitZoneSize / 2, hitZonePosition + hitZoneSize / 2);
        var sliderRectWidth = ((RectTransform)slider.transform).rect.width;
        float left = sliderRectWidth * (hitZoneRange.x / slider.maxValue);
        float right = sliderRectWidth - (left + sliderRectWidth * (hitZoneSize / slider.maxValue));
        hitZone.SetLeft(left);
        hitZone.SetRight(-right);
    }
}
