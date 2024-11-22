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
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private RecipeSO currentRecipe;
    
    public delegate void RecipeChanged(RecipeSO current, bool active);
    public static event RecipeChanged OnRecipeChanged;
    public delegate void RecipeComplete(RecipeSO current, bool success);
    public static event RecipeComplete OnRecipeComplete;
    private LevelSO level;

    private void OnEnable()
    {
        LevelManager.OnLevelStart += Initialize;
    }
    private void OnDisable()
    {
        LevelManager.OnLevelStart -= Initialize;
    }
    private void Initialize()
    {
        level = LevelManager.Instance.Level;
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
            if (level.RecipePool.Count != 0)
                return Instantiate(level.RecipePool[Random.Range(0, level.RecipePool.Count)]);
            return null;
        }
        if (!level.Infinite) return GetRandomFiniteRecipe();
        var newRandom = Instantiate(level.RandomPrototype);
        newRandom.Initialize();
        return newRandom;
    }
    
    private RecipeSO GetRandomFiniteRecipe()
    {
        if (level.FiniteRecipes.Count == 0)
        {
            return null;
        }
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
        if (!currentRecipe) return;
        bool allCorrect = true;
        var totalIngredients = ingredients.Count;
        var totalRecipeIngredients = currentRecipe.IngredientData.Sum(data => data.Value);
        //Check total ingredients
        if (totalIngredients != totalRecipeIngredients)
        {
            allCorrect = false;
        }
        var correctDict = new Dictionary<IngredientTypes, int>();
        var clone = new Dictionary<IngredientTypes, int>(currentRecipe.IngredientData);
        foreach (var ingredient in ingredients)
        {
            correctDict.TryAdd(ingredient.IngredientType, 0);
            bool hasType = currentRecipe.IngredientData[ingredient.IngredientType] > 0;
            bool sameCookState = dreamData[currentRecipe.DreamType].IngredientData[ingredient.IngredientType] == ingredient.CookState;
            bool stillRemain = clone[ingredient.IngredientType] > 0;
            if (hasType && sameCookState && stillRemain)
            {
                correctDict[ingredient.IngredientType]++;
                clone[ingredient.IngredientType]--;
                continue;
            }
            allCorrect = false;
        }
        if (clone.Any(x => x.Value > 0))
        {
            allCorrect = false;
        }
        int finalPrice = CalculatePrice(correctDict, allCorrect);
        InventoryManager.Instance.ChangeCurrency(finalPrice);
        OnRecipeComplete?.Invoke(currentRecipe, allCorrect);
    }
    
    private int CalculatePrice(Dictionary<IngredientTypes, int> correctDict, bool allCorrect)
    {
        int basePrice = 0;
        foreach (var pair in correctDict)
        {
            basePrice += ingredientData[pair.Key].CurrentPrice * pair.Value;
        }
        if (!allCorrect) return basePrice;
        float profitPercent = currentRecipe.CurrentProfit;
        int profit = (int) (basePrice * profitPercent);
        int finalPrice = basePrice + profit;
        return finalPrice;
    }
}
