using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Redcode.Extensions;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class JoyBlender : MonoBehaviour, IMinigame
{
    [Header("UI")]
    [SerializeField] private CanvasGroup minigameCanvasGroup;
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform hitZone;
    [SerializeField] private ClickableArea blenderButton;
    [SerializeField] private TMP_Text timerText;
    
    [Header("Settings")]
    [SerializeField] private bool randomHitZoneSize;
    [SerializeField, HideIf(nameof(randomHitZoneSize))] private float hitZoneSize;
    [SerializeField, ShowIf(nameof(randomHitZoneSize))] private Vector2 hitZoneSizeRandomRange;
    [SerializeField] private bool randomHitZonePosition;
    [SerializeField, HideIf(nameof(randomHitZonePosition))] private float hitZonePosition;
    [SerializeField] private Vector2 padding;
    [SerializeField] private float timeLimit;
    [SerializeField] private float sliderDropSpeed;
    [SerializeField] private float sliderBumpAmount;
    
    [Header("Visuals")]
    [SerializeField] private Transform blender;
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private float size = 0.3f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector2 spawnPointRandomRange;
    [SerializeField] private Vector2 ingredientForce;
    
    public event IMinigame.MinigameStart OnMinigameStart;
    public event IMinigame.MinigameEnd OnMinigameEnd;
    private Moroutine minigameCoroutine;
    private Vector2 hitZoneRange;
    private List<IngredientSO> ingredients = new List<IngredientSO>();
    private List<IngredientUI> ingredientUIs = new List<IngredientUI>();

    private void OnEnable()
    {
        blenderButton.OnClickEvent += OnBlenderButtonClick;
    }
    
    private void OnDisable()
    {
        blenderButton.OnClickEvent -= OnBlenderButtonClick;
    }
    
    private void Start()
    {
        minigameCanvasGroup.gameObject.SetActive(false);
    }

    private void OnBlenderButtonClick()
    {
        if (minigameCoroutine == null) return;
        if (!minigameCoroutine.IsRunning) return;
        GlobalSoundManager.Instance.PlayUISFX("MinigameButton");
        blender.DOShakePosition(0.1f, 5, 25);
        ingredientUIs.ForEach(x => x.AddForce(ingredientForce));
        slider.value += sliderBumpAmount;
    }
    
    public void Halt()
    {
        minigameCoroutine?.Stop();
        minigameCanvasGroup.gameObject.SetActive(false);
        minigameCoroutine?.Destroy();
    }
    
    public void StartMinigame(List<IngredientSO> ingredients)
    {
        this.ingredients = ingredients;
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        slider.value = slider.minValue;
        SpawnIngredients();
        minigameCoroutine = Moroutine.Run(gameObject, UpdateMinigame());
        minigameCoroutine.OnCompleted(_ => CheckCondition());
        GlobalSoundManager.Instance.PlayUISFX("Blend", true, "Blend");
    }
    
    private void SpawnIngredients()
    {
        foreach (var ingredient in ingredients)
        {
            var sp = new Vector3(
                spawnPoint.position.x + Random.Range(-spawnPointRandomRange.x, spawnPointRandomRange.x),
                spawnPoint.position.y + Random.Range(-spawnPointRandomRange.y, spawnPointRandomRange.y), 0);
            var ingredientUI = Instantiate(ingredientUIPrefab, sp, Quaternion.identity);
            ingredientUI.Initialize(ingredient, gameObject);
            ingredientUI.SetPhysics(true);
            ingredientUI.SetSize(size: size);
            ingredientUI.SetSortingOrder(11);
            ingredientUI.transform.SetParent(blender);
            ingredientUIs.Add(ingredientUI);
        }
    }

    private IEnumerable UpdateMinigame()
    {
        GenerateHitZone();
        float timer = timeLimit;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            slider.value -= sliderDropSpeed * Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
            yield return null;
        }
    }

    private void CheckCondition()
    {
        if (slider.value < hitZoneRange.x || slider.value > hitZoneRange.y)
        {
            Fail();
        }
        else
        {
            Succeed();
        }
    }

    private void Fail()
    {
        minigameCoroutine.Stop();
        ingredientUIs.ForEach(x => Destroy(x.gameObject));
        ingredientUIs.Clear();
        OnMinigameEnd?.Invoke(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        minigameCoroutine.Destroy();
        GlobalSoundManager.Instance.StopSound("Blend");
    }

    private void Succeed()
    {
        minigameCoroutine.Stop();
        ingredientUIs.ForEach(x => Destroy(x.gameObject));
        ingredientUIs.Clear();
        OnMinigameEnd?.Invoke(true);
        minigameCanvasGroup.gameObject.SetActive(false);
        minigameCoroutine.Destroy();
        GlobalSoundManager.Instance.StopSound("Blend");
    }
    
    private void GenerateHitZone()
    {
        if (randomHitZoneSize) hitZoneSize = Random.Range(hitZoneSizeRandomRange.x, hitZoneSizeRandomRange.y);
        if (randomHitZonePosition) hitZonePosition = Random.Range(slider.minValue + hitZoneSize / 2 + padding.x, slider.maxValue - hitZoneSize / 2 - padding.y);
        hitZoneRange = new Vector2(hitZonePosition - hitZoneSize / 2, hitZonePosition + hitZoneSize / 2);
        var sliderRectHeight = ((RectTransform)slider.transform).rect.height;
        float bottom = sliderRectHeight * (hitZoneRange.x / slider.maxValue);
        float top = sliderRectHeight - (bottom + sliderRectHeight * (hitZoneSize / slider.maxValue));
        hitZone.SetBottom(bottom);
        hitZone.SetTop(-top);
    }
}
