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
    [Serializable]
    private struct LightSpriteData
    {
        [SerializeField] private Sprite normal;
        [SerializeField] private Sprite success;
        [SerializeField] private Sprite fail;
        public Sprite Normal => normal;
        public Sprite Success => success;
        public Sprite Fail => fail;
    }
    
    [Header("UI")]
    [SerializeField] private CanvasGroup minigameCanvasGroup; 
    [SerializeField] private Image[] lights;
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderHandle;
    [SerializeField] private RectTransform hitZone;
    [SerializeField] private ClickableArea knob;
    [SerializeField] private LightSpriteData lightSpriteData;
    
    [Header("Settings")]
    [SerializeField] private bool randomHitZoneSize;
    [SerializeField, HideIf(nameof(randomHitZoneSize))] private float hitZoneSize;
    [SerializeField, ShowIf(nameof(randomHitZoneSize))] private Vector2 hitZoneSizeRandomRange;
    [SerializeField] private bool randomHitZonePosition;
    [SerializeField, HideIf(nameof(randomHitZonePosition))] private float hitZonePosition;
    [SerializeField] private Vector2 padding;
    [SerializeField] private float barSpeed;
    [SerializeField] private float preparationTime = 2f;
    
    [Header("Visuals")]
    [SerializeField] private Image oven;
    [SerializeField] private Image ingredientImagePrefab;
    [SerializeField] private float bopSize = 0.1f;
    [SerializeField] private float bopTime = 0.1f;
    [SerializeField] private Transform parent;

    private int currentAttempt;
    private int gameEndThreshold;
    private int mistakes;
    private int success;
    private bool ready;
    private Moroutine minigameCoroutine;
    private Vector2 hitZoneRange;
    private List<IngredientSO> ingredients = new List<IngredientSO>();
    private List<Image> ingredientImages = new List<Image>();
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
    
    public void StartMinigame(List<IngredientSO> ingredients)
    {
        this.ingredients = ingredients;
        SpawnIngredient();
        gameEndThreshold = Mathf.CeilToInt(lights.Length / 2f);
        success = 0;
        mistakes = 0;
        currentAttempt = 0;
        lights.ForEach(x => x.sprite = lightSpriteData.Normal);
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        minigameCoroutine = Moroutine.Run(gameObject, UpdateMinigame());
        minigameCoroutine.OnCompleted(_ => HitFail());
        GlobalSoundManager.Instance.PlayUISFX("Stove", true, "Stove");
    }
    
    private void SpawnIngredient()
    {
        foreach (var ingredient in ingredients)
        {
            var ingredientImage = Instantiate(ingredientImagePrefab, parent);
            ingredientImage.sprite = ingredient.GetSprite(ingredient.CookState);
            ingredientImage.transform.DOScale(bopSize, bopTime).SetRelative().SetLoops(-1, LoopType.Yoyo);
            ingredientImages.Add(ingredientImage);
        }
    }

    private void OnKnobClick()
    {
        if (minigameCoroutine == null) return;
        if (!minigameCoroutine.IsRunning) return;
        if (!ready) return;
        GlobalSoundManager.Instance.PlayUISFX("MinigameButton");
        if (slider.value < hitZoneRange.x || slider.value > hitZoneRange.y)
        {
           HitFail();
        }
        else
        {
            HitSuccess();
        }
    }

    private IEnumerable UpdateMinigame()
    {
        GenerateHitZone();
        slider.value = slider.minValue;
        ready = false;
        sliderHandle.DOColor(Color.yellow, preparationTime / 6).SetLoops(6, LoopType.Yoyo);
        knob.transform.rotation = Quaternion.Euler(0, 0, 90);
        yield return new WaitForSeconds(preparationTime);
        ready = true;
        while (slider.value < slider.maxValue)
        {
            float targetAngle = Mathf.Lerp(90f, -90f, slider.value / slider.maxValue);
            knob.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            slider.value += barSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void HitFail()
    {
        mistakes++;
        minigameCoroutine.Stop();
        //lights[currentAttempt].DOColor(Color.red, 0.2f);
        lights[currentAttempt].sprite = lightSpriteData.Fail;
        if (mistakes >= gameEndThreshold)
        {
            DOVirtual.DelayedCall(1f, Fail);
            return;
        }
        currentAttempt++;
        if (currentAttempt > lights.Length - 1)
        {
            DOVirtual.DelayedCall(1f, Success);
            return;
        }
        minigameCoroutine.Rerun();
    }

    private void HitSuccess()
    {
        success++;
        minigameCoroutine.Stop();
        oven.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
        lights[currentAttempt].sprite = lightSpriteData.Success;
        currentAttempt++;
        if (currentAttempt > lights.Length - 1 || success >= gameEndThreshold)
        {
            DOVirtual.DelayedCall(1f, Success);
            return;
        }
        minigameCoroutine.Rerun();
    }

    private void Fail()
    {
        ingredientImages.ForEach(x => Destroy(x.gameObject));
        ingredientImages.Clear();
        OnMinigameEnd?.Invoke(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        minigameCoroutine.Destroy();
        GlobalSoundManager.Instance.StopSound("Stove");
    }
    
    private void Success()
    {
        ingredientImages.ForEach(x => Destroy(x.gameObject));
        ingredientImages.Clear();
        OnMinigameEnd?.Invoke(true);
        minigameCanvasGroup.gameObject.SetActive(false);
        minigameCoroutine.Destroy();
        GlobalSoundManager.Instance.StopSound("Stove");
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
