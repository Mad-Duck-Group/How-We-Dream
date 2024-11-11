using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 1)]
public class RecipeSO : ScriptableObject
{
    [SerializeField] private bool randomDreamType;
    [SerializeField, HideIf(nameof(randomDreamType))] private DreamTypes dreamType;
    public DreamTypes DreamType => dreamType;

    [SerializeField] private bool randomIngredient;
    [SerializeField, ShowIf(nameof(randomIngredient))] private Vector2 ingredientTypeRange;
    [SerializeField, ShowIf(nameof(randomIngredient))] private Vector2 ingredientAmountRange;
    [SerializedDictionary("Ingredient Type", "Require Amount"), HideIf(nameof(randomIngredient))] 
    public SerializedDictionary<IngredientTypes, int> ingredientData;
    public SerializedDictionary<IngredientTypes, int> IngredientData => ingredientData;

    [SerializeField] private float profitModifier;
    public float ProfitModifier => profitModifier;

    public void Initialize()
    {
        if (randomDreamType)
        {
            var length = System.Enum.GetValues(typeof(DreamTypes)).Length;
            dreamType = (DreamTypes) Random.Range(0, length);
        }
        if (randomIngredient)
        {
            var loop = Random.Range(ingredientTypeRange.x, ingredientTypeRange.y);
            var length = System.Enum.GetValues(typeof(IngredientTypes)).Length;
            foreach (var data in ingredientData)
            {
                ingredientData[data.Key] = 0;
            }
            for (int i = 0; i < loop; i++)
            {
                var ingredientType = (IngredientTypes)Random.Range(0, length);
                var amount = Random.Range((int)ingredientAmountRange.x, (int)ingredientAmountRange.y);
                ingredientData[ingredientType] = amount;
            }
        }
    }

}
