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

public interface IMinigame
{
    public delegate void MinigameStart();
    public delegate void MinigameEnd(bool success);
    public event MinigameStart OnMinigameStart;
    public event MinigameEnd OnMinigameEnd;
    
    public void StartMinigame();
    public void Halt();
}
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
    [SerializeField] private float barSpeed;
    [SerializeField] private float preparationTime = 2f;

    private int currentAttempt;
    private int maxMistakes;
    private int mistakes;
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
        maxMistakes = Mathf.CeilToInt(lights.Length / 2f);
        mistakes = 0;
        currentAttempt = 0;
        lights.ForEach(x => x.color = Color.white);
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        minigameCoroutine = Moroutine.Run(gameObject, UpdateMinigame());
        minigameCoroutine.OnCompleted(_ => Fail());
    }

    private void OnKnobClick()
    {
        if (minigameCoroutine == null) return;
        if (!minigameCoroutine.IsRunning) return;
        if (!ready) return;
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
        if (mistakes >= maxMistakes)
        {
            OnMinigameEnd?.Invoke(false);
            minigameCanvasGroup.gameObject.SetActive(false);
            minigameCoroutine.Destroy();
            return;
        }
        currentAttempt++;
        if (currentAttempt > lights.Length - 1)
        {
            OnMinigameEnd?.Invoke(true);
            minigameCanvasGroup.gameObject.SetActive(false);
            minigameCoroutine.Destroy();
            return;
        }
        minigameCoroutine.Rerun();
    }

    private void Succeed()
    {
        minigameCoroutine.Stop();
        lights[currentAttempt].DOColor(Color.green, 0.2f);
        currentAttempt++;
        if (currentAttempt > lights.Length - 1)
        {
            OnMinigameEnd?.Invoke(true);
            minigameCanvasGroup.gameObject.SetActive(false);
            minigameCoroutine.Destroy();
            return;
        }
        minigameCoroutine.Rerun();
    }
    private void GenerateHitZone()
    {
        if (randomHitZoneSize) hitZoneSize = Random.Range(hitZoneSizeRandomRange.x, hitZoneSizeRandomRange.y);
        if (randomHitZonePosition) hitZonePosition = Random.Range(slider.minValue + hitZoneSize / 2, slider.maxValue - hitZoneSize / 2);
        hitZoneRange = new Vector2(hitZonePosition - hitZoneSize / 2, hitZonePosition + hitZoneSize / 2);
        var sliderRectWidth = ((RectTransform)slider.transform).rect.width;
        float left = sliderRectWidth * (hitZoneRange.x / slider.maxValue);
        float right = sliderRectWidth - (left + sliderRectWidth * (hitZoneSize / slider.maxValue));
        hitZone.SetLeft(left);
        hitZone.SetRight(-right);
    }
}
