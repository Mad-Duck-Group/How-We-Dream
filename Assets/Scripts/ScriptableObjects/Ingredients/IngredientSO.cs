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
    [SerializeField] private IngredientTypes ingredientType;
    public IngredientTypes IngredientType => ingredientType;
    [SerializeField] private float basePrice;
    public float BasePrice => basePrice;
    //[SerializeField, CurveRange(0, 0, 1, 1)] private AnimationCurve priceCurve;

    [SerializeField, ReadOnly] private CookStates cookState = CookStates.Raw;
    public CookStates CookState { get => cookState; set => cookState = value; }

    public Sprite GetSprite(CookStates cookState)
    {
        if (!spriteData.ContainsKey(cookState)) return null;
        var sprite = spriteData[cookState];
        return sprite.Sprite;
    }
    
    [Button("Reset Cook State")]
    private void ResetCookState()
    {
        cookState = CookStates.Raw;
    }
}
