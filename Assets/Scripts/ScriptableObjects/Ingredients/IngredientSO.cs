using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public enum IngredientTypes
{
    Truth,
    Imagination,
    Emotions,
    Memories,
    Realization,
    Subconscious,
    Time,
    Symbol,
    Mystery
}

public enum CookStates
{
    Raw,
    Baked,
    Boiled,
    Blended,
    Fried,
}
[Serializable]
public struct SpriteData
{
    [SerializeField, ShowAssetPreview, AllowNesting] private Sprite sprite;
    public Sprite Sprite => sprite;
}
[CreateAssetMenu(fileName = "New Ingredient", menuName = "ScriptableObjects/Ingredient")]
public class IngredientSO : ScriptableObject
{
    [SerializeField] private string ingredientName;
    public string IngredientName => ingredientName;
    [SerializedDictionary("Cook State", "Sprite Data")] 
    public SerializedDictionary<CookStates, SpriteData> spriteData;
    public SerializedDictionary<CookStates, SpriteData> SpriteData => spriteData;
    [SerializeField, ShowAssetPreview] private Sprite flippedSprite;
    [SerializeField] private IngredientTypes ingredientType;
    public IngredientTypes IngredientType => ingredientType;
    [SerializeField] private int basePrice;
    [SerializeField] private int maxPrice;
    [SerializeField] private AnimationCurve priceCurve;
    public int CurrentPrice
    {
        get
        {
            float progress = ProgressionManager.Instance.Progress;
            float eval = priceCurve.Evaluate(progress);
            return Mathf.RoundToInt(Mathf.Lerp(basePrice, maxPrice, eval));
        }
    }

    private bool used;
    public delegate void UseIngredient(IngredientTypes ingredientType);
    public static event UseIngredient OnIngredientUsed;
    [SerializeField, ReadOnly] private CookStates cookState = CookStates.Raw;
    public CookStates CookState
    {
        get => cookState;
        set
        {
            if (value is not CookStates.Raw) Use();
            cookState = value;
        }
    }

    public void Use()
    {
        if (used) return;
        OnIngredientUsed?.Invoke(ingredientType);
        used = true;
    }

    public Sprite GetSprite(CookStates cookState)
    {
        if (!spriteData.ContainsKey(cookState)) return null;
        var sprite = spriteData[cookState];
        return sprite.Sprite;
    }
    
    public Sprite GetFlippedSprite()
    {
        return flippedSprite;
    }
}
