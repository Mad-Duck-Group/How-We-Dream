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
    [SerializedDictionary("Dream Type", "Dream SO")] 
    public SerializedDictionary<DreamTypes, DreamSO> dreamData;
    public SerializedDictionary<DreamTypes, DreamSO> DreamData => dreamData;
    [SerializedDictionary("Ingredient Type", "Ingredient SO")] 
    public SerializedDictionary<IngredientTypes, IngredientSO> ingredientData;
    public SerializedDictionary<IngredientTypes, IngredientSO> IngredientData => ingredientData;
    [FormerlySerializedAs("recipes")] [SerializeField] private List<RecipeSO> recipePool;
    public List<RecipeSO> RecipePool => recipePool;
    [SerializeField] private List<RecipeSO> finiteRecipes;
    public List<RecipeSO> FiniteRecipes => finiteRecipes;
    [SerializeField] private bool endless;
    [SerializeField] private bool randomRecipe;
    [SerializeField, ShowIf(nameof(randomRecipe))] private bool toPool;
    [SerializeField, ShowIf(nameof(randomRecipe))] RecipeSO randomPrototype;
    [SerializeField, ShowIf(nameof(randomRecipe))] private Vector2 randomAmountRange;

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
        RandomRecipe();
    }

    private void RandomRecipe()
    {
        if (!randomRecipe) return;
        var loop = Random.Range(randomAmountRange.x, randomAmountRange.y);
        for (int i = 0; i < loop; i++)
        {
            var recipe = Instantiate(randomPrototype);
            recipe.Initialize();
            if (toPool)
                recipePool.Add(recipe);
            else 
                finiteRecipes.Add(recipe);
        }
    }
    
    public RecipeSO GetRandomRecipe()
    {
        if (endless) 
            return Instantiate(recipePool[Random.Range(0, recipePool.Count)]);
        if (finiteRecipes.Count == 0) return null;
        var recipe = finiteRecipes[Random.Range(0, finiteRecipes.Count)];
        finiteRecipes.Remove(recipe);
        return recipe;

    }

    public Sprite GetIngredientSprite(IngredientTypes type, CookStates cookStates)
    {
        if (!ingredientData.ContainsKey(type)) return null;
        return ingredientData[type].GetSprite(cookStates);
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
