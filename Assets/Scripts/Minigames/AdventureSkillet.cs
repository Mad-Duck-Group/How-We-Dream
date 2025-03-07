using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Redcode.Extensions;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AdventureSkillet : MonoBehaviour, IMinigame
{
    [Serializable]
    private struct MouseSpriteData
    {
        [SerializeField] private Sprite normal;
        [SerializeField] private Sprite success;
        [SerializeField] private Sprite fail;
        public Sprite Normal => normal;
        public Sprite Success => success;
        public Sprite Fail => fail;
    }
    
    [SerializeField] private CanvasGroup minigameCanvasGroup;

    [Header("Rotate")] 
    [SerializeField] private CanvasGroup rotateCanvasGroup;
    [SerializeField] private Transform center;
    [FormerlySerializedAs("rotateArea")] [SerializeField] private ClickableArea rotate;
    [SerializeField] private Slider rotateSlider;
    [SerializeField] private float skilletMoveModifier;
    [SerializeField] private float sliderGainRate;
    [SerializeField] private float changeDirectionThreshold;
    [SerializeField] private float timeLimit;
    [SerializeField] private TMP_Text timerText;
    
    [Header("Flip")]
    [SerializeField] private CanvasGroup flipCanvasGroup;
    [SerializeField] private Slider flipSlider;
    [SerializeField] private Image sliderHandle;
    [SerializeField] private Image mousePrefab;
    [SerializeField] private MouseSpriteData leftMouseSpriteData;
    [SerializeField] private MouseSpriteData rightMouseSpriteData;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector2 padding;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private int hitZoneCount;
    [SerializeField] private float hitZoneSize;
    [SerializeField] private float preparationTime = 2f;
    [SerializeField] private int maxMistakes = 3;

    [Header("Visuals")] 
    [SerializeField] private Transform skillet;
    [SerializeField] private IngredientUI ingredientUIPrefab;
    [SerializeField] private float size = 0.35f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector2 spawnPointRandomRange;
    [SerializeField] private float ingredientForce;

    public event IMinigame.MinigameStart OnMinigameStart;
    public event IMinigame.MinigameEnd OnMinigameEnd;
    private Vector2 MousePosition => Input.mousePosition.WithZ(0);
    private Vector2 MouseWorldPosition => Camera.main.ScreenToWorldPoint(MousePosition) * skilletMoveModifier;
    private Vector2 mousePosDifference;
    private Vector2 skilletOriginalPosition;
    private bool mouseDown;
    private bool isRotatePhase;
    private Moroutine rotateMinigameCoroutine;
    private Moroutine flipMinigameCoroutine;
    private List<IngredientSO> ingredients = new List<IngredientSO>();
    private List<IngredientUI> ingredientUIs = new List<IngredientUI>();

    private record FlipData(Vector2 HitZoneRange, bool IsLeft)
    {
        public Vector2 HitZoneRange { get; } = HitZoneRange;
        public bool IsLeft { get; } = IsLeft;
    }
    private Dictionary<Image, FlipData> flipPositionDict = new();

    private void OnEnable()
    {
        rotate.OnDownEvent += OnRotateDown;
        rotate.OnUpEvent += OnRotateUp;
    }

    private void OnDisable()
    {
        rotate.OnDownEvent -= OnRotateDown;
        rotate.OnUpEvent -= OnRotateUp;
    }

    private void OnRotateDown()
    {
        if (!isRotatePhase) return;
        GlobalSoundManager.Instance.PlayUISFX("MinigameButton");
        mousePosDifference = MouseWorldPosition - (Vector2)skillet.position;
        mouseDown = true;
    }

    private void OnRotateUp()
    {
        if (!isRotatePhase) return;
        mouseDown = false;
    }

    void Start()
    {
        skilletOriginalPosition = skillet.position;
        minigameCanvasGroup.gameObject.SetActive(false);
        rotateCanvasGroup.gameObject.SetActive(false);
        flipCanvasGroup.gameObject.SetActive(false);
    }
    
    public void Halt()
    {
        rotateMinigameCoroutine?.Stop();
        flipMinigameCoroutine?.Stop();
        minigameCanvasGroup.gameObject.SetActive(false);
        rotateMinigameCoroutine?.Destroy();
        flipMinigameCoroutine?.Destroy();
    }
    
    public void StartMinigame(List<IngredientSO> ingredients)
    {
        this.ingredients = ingredients;
        SpawnIngredients();
        OnMinigameStart?.Invoke();
        minigameCanvasGroup.gameObject.SetActive(true);
        StartRotateMinigame();
        GlobalSoundManager.Instance.PlayUISFX("Fry", true, "Fry");
    }
    
    private void SpawnIngredients()
    {
        foreach (var ingredient in ingredients)
        {
            var sp = new Vector3(
                spawnPoint.position.x + Random.Range(-spawnPointRandomRange.x, spawnPointRandomRange.x),
                spawnPoint.position.y + Random.Range(-spawnPointRandomRange.y, spawnPointRandomRange.y),
                spawnPoint.position.z);
            var ingredientUI = Instantiate(ingredientUIPrefab, sp, Quaternion.identity);
            ingredientUI.Initialize(ingredient, gameObject);
            ingredientUI.SetSortingOrder(11);
            ingredientUI.SetPhysics(true, false);
            ingredientUI.SetSize(size: size);
            //ingredientUI.transform.SetParent(skillet);
            ingredientUIs.Add(ingredientUI);
        }
    }

    private void StartRotateMinigame()
    {
        mouseDown = false;
        rotateSlider.value = 0f;
        rotateCanvasGroup.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        skillet.position = skilletOriginalPosition;
        rotateMinigameCoroutine = Moroutine.Run(gameObject, UpdateRotateMinigame());
        rotateMinigameCoroutine.OnCompleted(_ => Fail());
        isRotatePhase = true;
    }

    private IEnumerable UpdateRotateMinigame()
    {
        float timer = timeLimit;
        float previousAngle = 0;
        int previousSign = 0;
        int currentSign = 0;
        bool firstClick = true;
        float accumulatedAngle = 0f;
        bool changeDirection = false;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
            if (mouseDown)
            {
                skillet.position = (MouseWorldPosition - mousePosDifference);
                // convert mouse position to world space
                Vector2 centerScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, center.position);
                Vector2 direction = (MousePosition - centerScreenPoint).normalized;
                Vector2 forceVector = direction * ingredientForce;
                ingredientUIs.ForEach(x => x.AddForce(forceVector));
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (firstClick)
                {
                    previousAngle = angle;
                    firstClick = false;
                }
                float deltaAngle = Mathf.DeltaAngle(previousAngle, angle);
                currentSign = deltaAngle == 0 ? currentSign : (int)Mathf.Sign(deltaAngle);
                if (previousSign == 0)
                {
                    previousSign = currentSign;
                }
                float absDeltaAngle = Mathf.Abs(deltaAngle);
                bool passedThreshold = false;
                if (!changeDirection)
                {
                    changeDirection = currentSign != previousSign;
                    passedThreshold = true;
                }
                else
                {
                    accumulatedAngle += absDeltaAngle;
                    accumulatedAngle = currentSign != previousSign ? 0 : accumulatedAngle;
                    if (accumulatedAngle >= changeDirectionThreshold)
                    {
                        passedThreshold = true;
                        accumulatedAngle = 0f;
                        changeDirection = false;
                    }
                }
                if (passedThreshold)
                {
                    rotateSlider.value += absDeltaAngle * sliderGainRate * Time.deltaTime;
                }
                previousAngle = angle;
                previousSign = currentSign;
                CheckRotateCondition();
            }
            else
            {
                firstClick = true;
            }
            yield return null;
        }
    }

    private void CheckRotateCondition()
    {
        if (rotateSlider.value < rotateSlider.maxValue) return;
        isRotatePhase = false;
        rotateMinigameCoroutine.Stop();
        rotateCanvasGroup.gameObject.SetActive(false);
        skillet.DOMove(skilletOriginalPosition, 0.5f).OnComplete(() => StartFlipMinigame());
    }

    private void StartFlipMinigame()
    {
        flipSlider.value = 0f;
        timerText.gameObject.SetActive(false);
        flipCanvasGroup.gameObject.SetActive(true);
        flipPositionDict.Clear();
        for (int i = 0; i < hitZoneCount; i++)
        {
            var hitZone = Instantiate(mousePrefab, parent);
            var isLeft = Random.Range(0, 2) == 0;
            var sprite = isLeft ? leftMouseSpriteData.Normal : rightMouseSpriteData.Normal;
            hitZone.sprite = sprite;
            var hitZonePositions = GenerateHitZone(i);
            ((RectTransform)hitZone.transform).SetAnchoredPositionX(hitZonePositions.Item2.x);
            flipPositionDict.Add(hitZone, new FlipData(hitZonePositions.Item1, isLeft));
        }
        flipMinigameCoroutine = Moroutine.Run(gameObject, UpdateFlipMinigame());
        flipMinigameCoroutine.OnCompleted(_ => Succeed());
        rotateMinigameCoroutine?.Destroy();
    }


    private IEnumerable UpdateFlipMinigame()
    {
        int currentHitZoneIndex = 0;
        int mistakes = 0;
        sliderHandle.DOColor(Color.yellow, preparationTime / 6).SetLoops(6, LoopType.Yoyo);
        yield return new WaitForSeconds(preparationTime);
        while (flipSlider.value < flipSlider.maxValue)
        {
            flipSlider.value += sliderSpeed * Time.deltaTime;
            if (currentHitZoneIndex < flipPositionDict.Count)
            {
                var currentHitZone = flipPositionDict.ElementAt(currentHitZoneIndex);
                if (Input.GetMouseButtonDown(0))
                {
                    GlobalSoundManager.Instance.PlayUISFX("LeftSpatula");
                    CheckMouseButton(true, currentHitZone, ref mistakes, ref currentHitZoneIndex);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    GlobalSoundManager.Instance.PlayUISFX("RightSpatula");
                    CheckMouseButton(false, currentHitZone, ref mistakes, ref currentHitZoneIndex);
                }
                if (flipSlider.value > currentHitZone.Value.HitZoneRange.y && currentHitZoneIndex <= flipPositionDict.Count - 1)
                {
                    bool canStillFail = flipPositionDict.Count - currentHitZoneIndex >= maxMistakes - mistakes;
                    if (canStillFail) mistakes++;
                    var sprite = currentHitZone.Value.IsLeft ? leftMouseSpriteData.Fail : rightMouseSpriteData.Fail;
                    currentHitZone.Key.sprite = sprite;
                    currentHitZoneIndex++;
                }
            }
            if (mistakes >= maxMistakes)
            {
                Fail();
            }
            yield return null;
        }
    }

    private void CheckMouseButton(bool isLeft, KeyValuePair<Image, FlipData> currentHitZone, ref int mistakes, ref int currentHitZoneIndex)
    {
        if (isLeft != currentHitZone.Value.IsLeft || flipSlider.value < currentHitZone.Value.HitZoneRange.x ||
            flipSlider.value > currentHitZone.Value.HitZoneRange.y)
        {
            mistakes++;
            var sprite = currentHitZone.Value.IsLeft ? leftMouseSpriteData.Fail : rightMouseSpriteData.Fail;
            currentHitZone.Key.sprite = sprite;
        }
        else
        {
            var sprite = currentHitZone.Value.IsLeft ? leftMouseSpriteData.Success : rightMouseSpriteData.Success;
            currentHitZone.Key.sprite = sprite;
            ingredientUIs.ForEach(x => x.FlipSprite());
        }
        currentHitZoneIndex++;
    }

    private void Fail()
    {
        flipPositionDict.Keys.ForEach(Destroy);
        ingredientUIs.ForEach(x => Destroy(x.gameObject));
        ingredientUIs.Clear();
        rotateMinigameCoroutine?.Stop();
        flipMinigameCoroutine?.Stop();
        OnMinigameEnd?.Invoke(false);
        rotateCanvasGroup.gameObject.SetActive(false);
        flipCanvasGroup.gameObject.SetActive(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        rotateMinigameCoroutine?.Destroy();
        flipMinigameCoroutine?.Destroy();
        GlobalSoundManager.Instance.StopSound("Fry");
    }

    private void Succeed()
    {
        flipPositionDict.Keys.ForEach(Destroy);
        ingredientUIs.ForEach(x => Destroy(x.gameObject));
        ingredientUIs.Clear();
        rotateMinigameCoroutine?.Stop();
        flipMinigameCoroutine?.Stop();
        OnMinigameEnd?.Invoke(true);
        rotateCanvasGroup.gameObject.SetActive(false);
        flipCanvasGroup.gameObject.SetActive(false);
        minigameCanvasGroup.gameObject.SetActive(false);
        rotateMinigameCoroutine?.Destroy();
        flipMinigameCoroutine?.Destroy();
        GlobalSoundManager.Instance.StopSound("Fry");
    }

    private (Vector2, Vector2) GenerateHitZone(int index)
    {
        var leftWithPadding = flipSlider.minValue + padding.x;
        var rightWithPadding = flipSlider.maxValue - padding.y;
        var hitZoneGap = (rightWithPadding - leftWithPadding) / (hitZoneCount - 1);
        var hitZonePosition = leftWithPadding + hitZoneGap * index;
        var hitZoneRange = new Vector2(hitZonePosition - hitZoneSize / 2, hitZonePosition + hitZoneSize / 2);
        var sliderWidth = ((RectTransform)flipSlider.transform).rect.width;
        var percentage = hitZonePosition / flipSlider.maxValue;
        Vector2 screenHitZone = new Vector2(sliderWidth * percentage, 0);
        return (hitZoneRange, screenHitZone);
    }
}
