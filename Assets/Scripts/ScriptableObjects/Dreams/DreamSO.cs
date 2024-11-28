using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
public enum DreamTypes
{
    Lucid,
    Day,
    Prophetic,
    Suppressed
}

[CreateAssetMenu(fileName = "Dream", menuName = "ScriptableObjects/Dream", order = 2)]
public class DreamSO : ScriptableObject
{
    [SerializeField] private DreamTypes dreamType;
    [SerializeField] private string dreamTypeName;
    public string DreamTypeName => dreamTypeName;
    public DreamTypes DreamType => dreamType;
    [SerializedDictionary("Ingredient Type", "Required Cook State")] 
    public SerializedDictionary<IngredientTypes, CookStates> ingredientData;
    public SerializedDictionary<IngredientTypes, CookStates> IngredientData => ingredientData;
}
