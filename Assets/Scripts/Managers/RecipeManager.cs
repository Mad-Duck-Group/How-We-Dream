using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RecipeManager : MonoSingleton<RecipeManager>
{
    [Header("Dictionary Data")]
    [SerializedDictionary("Dream Type", "Dream SO")] 
    public SerializedDictionary<DreamTypes, DreamSO> dreamData;
    public SerializedDictionary<DreamTypes, DreamSO> DreamData => dreamData;
    [SerializedDictionary("Ingredient Type", "Ingredient SO")] 
    public SerializedDictionary<IngredientTypes, IngredientSO> ingredientData;
    public SerializedDictionary<IngredientTypes, IngredientSO> IngredientData => ingredientData;

    [Header("Level Data")]
    [SerializeField, Expandable] private LevelSO level;

    [Header("Debug")]
    [SerializeField, ReadOnly] private RecipeSO currentRecipe;
    
    public delegate void RecipeChanged(RecipeSO current, bool active);
    public static event RecipeChanged OnRecipeChanged;
    public delegate void RecipeComplete(RecipeSO current, bool success);
    public static event RecipeComplete OnRecipeComplete;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        level = Instantiate(level);
        RandomRecipe();
        InitializeRecipe();
    }

    private void RandomRecipe()
    {
        if (!level.RandomRecipe) return;
        if (level.Infinite) return;
        var loop = Random.Range(level.RandomAmountRange.x, level.RandomAmountRange.y);
        for (int i = 0; i < loop; i++)
        {
            var recipe = Instantiate(level.RandomPrototype);
            if (level.UsePool && level.RandomToPool)
                level.RecipePool.Add(recipe);
            else 
                level.FiniteRecipes.Add(recipe);
        }
    }
    private void InitializeRecipe()
    {
        level.RecipePool.ForEach(recipe => recipe.Initialize());
        level.FiniteRecipes.ForEach(recipe => recipe.Initialize());
    }
    
    public RecipeSO GetRandomRecipe()
    {
        if (level.UsePool && !level.Infinite)
        {
            if (level.FiniteRecipes.Count > 0) return GetRandomFiniteRecipe();
            if (level.RecipePool.Count == 0) return null;
            return Instantiate(level.RecipePool[Random.Range(0, level.RecipePool.Count)]);
        }
        if (level.Infinite)
        {
            var newRandom = Instantiate(level.RandomPrototype);
            newRandom.Initialize();
            return newRandom;
        }
        return GetRandomFiniteRecipe();
    }
    
    private RecipeSO GetRandomFiniteRecipe()
    {
        if (level.FiniteRecipes.Count == 0) return null;
        var recipe = level.FiniteRecipes[Random.Range(0, level.FiniteRecipes.Count)];
        level.FiniteRecipes.Remove(recipe);
        return Instantiate(recipe);
    }

    public void SetActiveRecipe(RecipeSO recipeSo)
    {
        currentRecipe = recipeSo;
        OnRecipeChanged?.Invoke(currentRecipe, true);
    }

    public void UnsetActiveRecipe(RecipeSO recipeSo)
    {
        if (recipeSo != currentRecipe) return;
        OnRecipeChanged?.Invoke(currentRecipe, false);
        currentRecipe = null;
    }
    
    public void CheckRecipe(List<IngredientSO> ingredients)
    {
        var totalIngredients = ingredients.Count;
        var totalRecipeIngredients = currentRecipe.IngredientData.Sum(data => data.Value);
        //Check total ingredients
        if (totalIngredients != totalRecipeIngredients)
        {
            Fail();
            return;
        }
        var countDict = new Dictionary<IngredientTypes, int>();
        foreach (var ingredient in ingredients)
        {
            if (!countDict.ContainsKey(ingredient.IngredientType))
            {
                countDict.Add(ingredient.IngredientType, 1);
            }
            else
            {
                countDict[ingredient.IngredientType]++;
            }
        }
        //Check ingredient count
        foreach (var data in 
                 currentRecipe.IngredientData.Where(data 
                     => countDict.ContainsKey(data.Key)))
        {
            if (countDict[data.Key] == data.Value) continue;
            Fail();
            return;
        }
        //Check ingredient cook state
        bool cookStateCheck = ingredients.All(so => so.CookState == dreamData[currentRecipe.DreamType].IngredientData[so.IngredientType]);
        if (!cookStateCheck)
        {
            Fail();
            return;
        }
        Succeed();
    }

    private void Fail()
    {
        OnRecipeComplete?.Invoke(currentRecipe, false);
    }

    private void Succeed()
    {
        OnRecipeComplete?.Invoke(currentRecipe, true);
    }
}
