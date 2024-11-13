using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using Redcode.Extensions;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WhimsyBoiler : MonoBehaviour, IMinigame
{
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
    [SerializeField] private float pourSpeed;
    
    [Header("Fire Sliders")]
    [SerializeField] private CanvasGroup fireSlidersCanvasGroup;
    [SerializedDictionary("Slider", "Fire Image")]
    public SerializedDictionary<Slider, Image> fireSliderDict = new();
    [SerializeField] private bool randomSweetSpotSize;
    [SerializeField, ShowIf(nameof(randomSweetSpotSize))] private Vector2 sweetSpotSizeRange;
    [SerializeField, HideIf(nameof(randomSweetSpotSize))] private float sweetSpotSize;
    [SerializeField] private bool randomSweetSpotPosition;
    [SerializeField, HideIf(nameof(randomSweetSpotPosition))] private float sweetSpotPosition;
    [SerializeField] private float mediumHeatPadding;
    [SerializeField] private float timeLimit;
    [SerializeField] private TMP_Text timerText;

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
    private Vector2 hitZoneRange;
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
    
    public void StartMinigame()
    {
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        StartPotWater();
    }
    
    private void StartPotWater()
    {
        potWaterCanvasGroup.gameObject.SetActive(true);
        GenerateHitZone();
        foreach (var pair in fireSliderDict)
        {
            pair.Key.value = pair.Key.minValue;
            pair.Value.color = Color.red;
        }
        potWaterSlider.value = potWaterSlider.minValue;
        mouseDown = false;
        isPotWaterPhase = true;
    }

    private void OnPourButtonDown()
    {
        if (!isPotWaterPhase) return;
        mouseDown = true;
        potWaterCoroutine = Moroutine.Run(gameObject, PourWater());
        potWaterCoroutine.OnCompleted(_ => CheckPotWaterCondition());
    }

    private void OnPourButtonUp()
    {
        if (!isPotWaterPhase) return;
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
            StartFireSliders();
        }
    }

    private void StartFireSliders()
    {
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

    private void OnValueChanged(Slider slider)
    {
        if (isPotWaterPhase) return;
        var sweetSpot = sweetSpotDict[slider].SweetSpot;
        var mediumHeat = sweetSpotDict[slider].MediumHeat;
        if (slider.value >= sweetSpot.x && slider.value <= sweetSpot.y)
        {
            fireSliderDict[slider].color = Color.green;
            sweetSpotDict[slider].IsHit = true;
        }
        else if (slider.value >= mediumHeat.x && slider.value <= mediumHeat.y)
        {
            fireSliderDict[slider].color = Color.yellow;
            sweetSpotDict[slider].IsHit = false;
        }
        else
        {
            fireSliderDict[slider].color = Color.red;
            sweetSpotDict[slider].IsHit = false;
        }
        CheckFireSlidersCondition();
    }

    private void CheckFireSlidersCondition()
    {
        var allHit = sweetSpotDict.All(x => x.Value.IsHit);
        if (allHit)
        {
            Succeed();
        }
    }

    private void Fail()
    {
        fireSliderCoroutine?.Stop();
        OnMinigameEnd?.Invoke(false);
        potWaterCanvasGroup.gameObject.SetActive(false);
        fireSlidersCanvasGroup.gameObject.SetActive(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        fireSliderCoroutine?.Destroy();
        potWaterCoroutine?.Destroy();
    }
    
    private void Succeed()
    {
        fireSliderCoroutine?.Stop();
        OnMinigameEnd?.Invoke(true);
        potWaterCanvasGroup.gameObject.SetActive(false);
        fireSlidersCanvasGroup.gameObject.SetActive(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        fireSliderCoroutine?.Destroy();
        potWaterCoroutine?.Destroy();
    }
    
    private void GenerateHitZone()
    {
        if (randomHitZoneSize) hitZoneSize = Random.Range(hitZoneSizeRandomRange.x, hitZoneSizeRandomRange.y);
        if (randomHitZonePosition) hitZonePosition = Random.Range(potWaterSlider.minValue + hitZoneSize / 2, potWaterSlider.maxValue - hitZoneSize / 2);
        hitZoneRange = new Vector2(hitZonePosition - hitZoneSize / 2, hitZonePosition + hitZoneSize / 2);
        var sliderRectHeight = ((RectTransform)potWaterSlider.transform).rect.height;
        float buttom = sliderRectHeight * (hitZoneRange.x / potWaterSlider.maxValue);
        float top = sliderRectHeight - (buttom + sliderRectHeight * (hitZoneSize / potWaterSlider.maxValue));
        hitZone.SetBottom(buttom);
        hitZone.SetTop(-top);
    }

    private Vector2 GenerateSweetSpot(Slider slider)
    {
        if (randomSweetSpotSize) sweetSpotSize = Random.Range(sweetSpotSizeRange.x, sweetSpotSizeRange.y);
        if (randomSweetSpotPosition) sweetSpotPosition = Random.Range(slider.minValue + sweetSpotSize / 2, slider.maxValue - sweetSpotSize / 2);
        return new Vector2(sweetSpotPosition - sweetSpotSize / 2, sweetSpotPosition + sweetSpotSize / 2);
    }
}
