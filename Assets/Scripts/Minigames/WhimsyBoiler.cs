using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using NaughtyAttributes;
using Redcode.Extensions;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class WhimsyBoiler : MonoBehaviour, IMinigame
{

    [Serializable]
    public struct FireSpriteData
    {
        [SerializeField, ShowAssetPreview(32, 32)] private Sprite low;
        [SerializeField, ShowAssetPreview(32, 32)] private Sprite medium;
        [SerializeField, ShowAssetPreview(32, 32)] private Sprite high;
        public Sprite Low => low;
        public Sprite Medium => medium;
        public Sprite High => high;
    }
    
    [SerializeField] private CanvasGroup minigameCanvasGroup;
    
    [Header("Pot Water")]
    [SerializeField] private CanvasGroup potWaterCanvasGroup;
    [SerializeField] private Slider potWaterSlider;
    [SerializeField] private RectTransform hitZone;
    [SerializeField] private ClickableArea pourButton;
    [SerializeField] private bool randomHitZoneSize;
    [SerializeField, HideIf(nameof(randomHitZoneSize))] private float hitZoneSize;
    [SerializeField, ShowIf(nameof(randomHitZoneSize))] private Vector2 hitZoneSizeRandomRange;
    [SerializeField] private bool randomHitZonePosition;
    [SerializeField, HideIf(nameof(randomHitZonePosition))] private float hitZonePosition;
    [SerializeField] private Vector2 padding;
    [SerializeField] private float pourSpeed;
    
    [Header("Fire Sliders")]
    [SerializeField] private CanvasGroup fireSlidersCanvasGroup;
    [SerializedDictionary("Slider", "Fire Image")]
    public SerializedDictionary<Slider, Image> fireSliderDict = new();
    [SerializedDictionary("Image", "Fire Sprites")]
    public SerializedDictionary<Image, FireSpriteData> fireSpriteDict = new();
    [SerializeField] private bool randomSweetSpotSize;
    [SerializeField, ShowIf(nameof(randomSweetSpotSize))] private Vector2 sweetSpotSizeRange;
    [SerializeField, HideIf(nameof(randomSweetSpotSize))] private float sweetSpotSize;
    [SerializeField] private bool randomSweetSpotPosition;
    [SerializeField, HideIf(nameof(randomSweetSpotPosition))] private float sweetSpotPosition;
    [SerializeField] private float mediumHeatPadding;
    [SerializeField] private float timeLimit;
    [SerializeField] private TMP_Text timerText;

    [Header("Visuals")] 
    [SerializeField] private Transform boiler;
    [SerializeField] private Vector3 potWaterPosition;
    [SerializeField] private Vector3 fireSliderPosition;
    [SerializeField] private GameObject fireImageRack;
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private float size = 0.3f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector2 spawnPointRandomRange;
    [SerializeField] private float forceInterval;
    [SerializeField] private Vector2 ingredientForce;
    
    [Button("Set Pot Water Position")]
    private void SetPotWaterPosition()
    {
        potWaterPosition = boiler.position;
    }
    
    [Button("Set Fire Sliders Position")]
    private void SetFireSlidersPosition()
    {
        fireSliderPosition = boiler.position;
    }

    public event IMinigame.MinigameStart OnMinigameStart;
    public event IMinigame.MinigameEnd OnMinigameEnd;

    private class SweetSpotData
    {
        private Vector2 sweetSpot;
        public Vector2 SweetSpot => sweetSpot;
        private Vector2 mediumHeat;
        public Vector2 MediumHeat => mediumHeat;
        public bool IsHit { get; set; }
        
        public SweetSpotData(Vector2 sweetSpot, Vector2 mediumHeat)
        {
            this.sweetSpot = sweetSpot;
            this.mediumHeat = mediumHeat;
        }
    }
    private bool mouseDown;
    private Dictionary<Slider, SweetSpotData> sweetSpotDict = new();
    private bool isPotWaterPhase;
    private Moroutine potWaterCoroutine;
    private Moroutine fireSliderCoroutine;
    private Moroutine ingredientBopCoroutine;
    private Vector2 hitZoneRange;
    private int correctCount;
    private List<IngredientSO> ingredients = new List<IngredientSO>();
    private List<IngredientUI> ingredientUIs = new List<IngredientUI>();
    private void OnEnable()
    {
        pourButton.OnDownEvent += OnPourButtonDown;
        pourButton.OnUpEvent += OnPourButtonUp;
    }

    private void OnDisable()
    {
        pourButton.OnDownEvent -= OnPourButtonDown;
        pourButton.OnUpEvent -= OnPourButtonUp;
    }
    void Start()
    {
        minigameCanvasGroup.gameObject.SetActive(false);
        potWaterCanvasGroup.gameObject.SetActive(false);
        fireSlidersCanvasGroup.gameObject.SetActive(false);
    }
    
    public void Halt()
    {
        potWaterCoroutine?.Stop();
        fireSliderCoroutine?.Stop();
        minigameCanvasGroup.gameObject.SetActive(false);
        potWaterCoroutine?.Destroy();
        fireSliderCoroutine?.Destroy();
    }
    
    public void StartMinigame(List<IngredientSO> ingredients)
    {
        this.ingredients = ingredients;
        SpawnIngredient();
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        StartPotWater();
    }
    
    private void SpawnIngredient()
    {
        foreach (var ingredient in ingredients)
        {
            var ingredientUI = Instantiate(ingredientUIPrefab,
                spawnPoint.position + new Vector3(Random.Range(-spawnPointRandomRange.x, spawnPointRandomRange.x),
                    Random.Range(-spawnPointRandomRange.y, spawnPointRandomRange.y)), Quaternion.identity, spawnPoint);
            ingredientUI.Initialize(ingredient, gameObject);
            ingredientUI.SetPhysics(true);
            ingredientUI.SetSize(size: size);
            ingredientUI.SetSortingOrder(11);
            ingredientUI.transform.SetParent(boiler);
            ingredientUIs.Add(ingredientUI);
        }
    }

    private void StartPotWater()
    {
        potWaterCanvasGroup.gameObject.SetActive(true);
        fireImageRack.SetActive(false);
        timerText.gameObject.SetActive(false);
        hitZone.gameObject.SetActive(true);
        pourButton.gameObject.SetActive(true);
        boiler.position = potWaterPosition;
        GenerateHitZone();
        foreach (var pair in fireSliderDict)
        {
            pair.Key.value = pair.Key.minValue;
            pair.Value.sprite = fireSpriteDict[pair.Value].Low;
        }
        potWaterSlider.value = potWaterSlider.minValue;
        mouseDown = false;
        isPotWaterPhase = true;
    }

    private void OnPourButtonDown()
    {
        if (!isPotWaterPhase) return;
        mouseDown = true;
        GlobalSoundManager.Instance.PlayUISFX("MinigameButton");
        GlobalSoundManager.Instance.PlayUISFX("Pour", true, "Pour");
        potWaterCoroutine = Moroutine.Run(gameObject, PourWater());
        potWaterCoroutine.OnCompleted(_ => CheckPotWaterCondition());
    }

    private void OnPourButtonUp()
    {
        if (!isPotWaterPhase) return;
        GlobalSoundManager.Instance.StopSound("Pour");
        mouseDown = false;
    }

    private IEnumerable PourWater()
    {
        while (mouseDown)
        {
            potWaterSlider.value += pourSpeed * Time.deltaTime;
            yield return null;
        }
    }
    
    private void CheckPotWaterCondition()
    {
        if (potWaterSlider.value < hitZoneRange.x || potWaterSlider.value > hitZoneRange.y)
        {
            Fail();
        }
        else
        {
            pourButton.gameObject.SetActive(false);
            boiler.DOMove(fireSliderPosition, 1f).OnComplete(() =>
            {
                StartFireSliders();
            });
        }
    }

    private void StartFireSliders()
    {
        GlobalSoundManager.Instance.PlayUISFX("Stove", true, "Stove");
        correctCount = 0;
        fireImageRack.SetActive(true);
        timerText.gameObject.SetActive(true);
        hitZone.gameObject.SetActive(false);
        fireSliderDict.Keys.ForEach(x => x.interactable = true);
        isPotWaterPhase = false;
        potWaterCanvasGroup.gameObject.SetActive(false);
        fireSlidersCanvasGroup.gameObject.SetActive(true);
        sweetSpotDict.Clear();
        foreach (var pair in fireSliderDict)
        {
            var sweetSpot = GenerateSweetSpot(pair.Key);
            var mediumHeat = new Vector2(sweetSpot.x - mediumHeatPadding, sweetSpot.y + mediumHeatPadding);
            sweetSpotDict.Add(pair.Key, new SweetSpotData(sweetSpot, mediumHeat));
            pair.Key.onValueChanged.RemoveAllListeners();
            pair.Key.onValueChanged.AddListener(_ => OnValueChanged(pair.Key));
        }
        fireSliderCoroutine = Moroutine.Run(gameObject, FireSliderTimer());
        fireSliderCoroutine.OnCompleted(_ => Fail());
        ingredientBopCoroutine = Moroutine.Run(gameObject, IngredientBop());
        potWaterCoroutine?.Destroy();
    }

    private IEnumerable FireSliderTimer()
    {
        float timer = timeLimit;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
            yield return null;
        }
    }

    private IEnumerable IngredientBop()
    {
        float interval = 0;
        while (!isPotWaterPhase)
        {
            interval += Time.deltaTime;
            if (interval >= forceInterval)
            {
                foreach (var ingredientUI in ingredientUIs)
                {
                    var force = ingredientForce * (correctCount / (float)sweetSpotDict.Count);
                    ingredientUI.AddForce(force, fullRange: false);
                }
                interval = 0;
            }
            yield return null;
        }
        yield return null;
    }

    private void OnValueChanged(Slider slider)
    {
        if (isPotWaterPhase) return;
        var sweetSpot = sweetSpotDict[slider].SweetSpot;
        var mediumHeat = sweetSpotDict[slider].MediumHeat;
        if (slider.value >= sweetSpot.x && slider.value <= sweetSpot.y)
        {
            fireSliderDict[slider].sprite = fireSpriteDict[fireSliderDict[slider]].High;
            sweetSpotDict[slider].IsHit = true;
        }
        else if (slider.value >= mediumHeat.x && slider.value <= mediumHeat.y)
        {
            fireSliderDict[slider].sprite = fireSpriteDict[fireSliderDict[slider]].Medium;
            sweetSpotDict[slider].IsHit = false;
        }
        else
        {
            fireSliderDict[slider].sprite = fireSpriteDict[fireSliderDict[slider]].Low;
            sweetSpotDict[slider].IsHit = false;
        }
        CheckFireSlidersCondition();
    }

    private void CheckFireSlidersCondition()
    {
        correctCount = sweetSpotDict.Count(x => x.Value.IsHit);
        if (correctCount == sweetSpotDict.Count)
        {
            fireSliderDict.Keys.ForEach(x => x.interactable = false);
            fireSliderCoroutine?.Stop();
            DOVirtual.DelayedCall(2f, Succeed);
        }
    }

    private void Fail()
    {
        ingredientUIs.ForEach(x => Destroy(x.gameObject));
        ingredientUIs.Clear();
        fireSliderCoroutine?.Stop();
        OnMinigameEnd?.Invoke(false);
        potWaterCanvasGroup.gameObject.SetActive(false);
        fireSlidersCanvasGroup.gameObject.SetActive(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        ingredientBopCoroutine?.Stop();
        ingredientBopCoroutine?.Destroy();
        fireSliderCoroutine?.Destroy();
        potWaterCoroutine?.Destroy();
        GlobalSoundManager.Instance.StopSound("Stove");
    }
    
    private void Succeed()
    {
        ingredientUIs.ForEach(x => Destroy(x.gameObject));
        ingredientUIs.Clear();
        OnMinigameEnd?.Invoke(true);
        potWaterCanvasGroup.gameObject.SetActive(false);
        fireSlidersCanvasGroup.gameObject.SetActive(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        ingredientBopCoroutine?.Stop();
        ingredientBopCoroutine?.Destroy();
        fireSliderCoroutine?.Destroy();
        potWaterCoroutine?.Destroy();
        GlobalSoundManager.Instance.StopSound("Stove");
    }
    
    private void GenerateHitZone()
    {
        if (randomHitZoneSize) hitZoneSize = Random.Range(hitZoneSizeRandomRange.x, hitZoneSizeRandomRange.y);
        if (randomHitZonePosition) hitZonePosition = Random.Range(potWaterSlider.minValue + hitZoneSize / 2 + padding.x, potWaterSlider.maxValue - hitZoneSize / 2 - padding.y);
        hitZoneRange = new Vector2(hitZonePosition - hitZoneSize / 2, hitZonePosition + hitZoneSize / 2);
        var sliderRectHeight = ((RectTransform)potWaterSlider.transform).rect.height;
        float bottom = sliderRectHeight * (hitZoneRange.x / potWaterSlider.maxValue);
        float top = sliderRectHeight - (bottom + sliderRectHeight * (hitZoneSize / potWaterSlider.maxValue));
        hitZone.SetBottom(bottom);
        hitZone.SetTop(-top);
    }

    private Vector2 GenerateSweetSpot(Slider slider)
    {
        if (randomSweetSpotSize) sweetSpotSize = Random.Range(sweetSpotSizeRange.x, sweetSpotSizeRange.y);
        if (randomSweetSpotPosition) sweetSpotPosition = Random.Range(slider.minValue + sweetSpotSize / 2, slider.maxValue - sweetSpotSize / 2);
        return new Vector2(sweetSpotPosition - sweetSpotSize / 2, sweetSpotPosition + sweetSpotSize / 2);
    }
}
