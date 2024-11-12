using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoSingleton<RecipeManager>
{
    [SerializedDictionary("Dream Type", "Dream SO")] 
    public SerializedDictionary<DreamTypes, DreamSO> dreamData;
    public SerializedDictionary<DreamTypes, DreamSO> DreamData => dreamData;
    [SerializeField] private List<RecipeSO> recipes;
    public List<RecipeSO> Recipes => recipes;
    [SerializeField] private bool randomRecipe;
    [SerializeField, ShowIf(nameof(randomRecipe))] RecipeSO randomPrototype;
    [SerializeField, ShowIf(nameof(randomRecipe))] private Vector2 randomAmountRange;
    [SerializeField] private TMP_Text orderText;

    [SerializeField, ReadOnly] private RecipeSO currentRecipe;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        if (randomRecipe)
        {
            var loop = Random.Range(randomAmountRange.x, randomAmountRange.y);
            for (int i = 0; i < loop; i++)
            {
                var recipe = Instantiate(randomPrototype);
                recipe.Initialize();
                recipes.Add(recipe);
            }
        }
        currentRecipe = recipes[Random.Range(0, recipes.Count)];
        SetOrderText();
    }
    
    private void SetOrderText()
    {
        orderText.text = $"Dream Type: {currentRecipe.DreamType}\n" +
                         $"Ingredients: \n" +
                         $"{string.Join("\n", currentRecipe.IngredientData.Select(data => $"{data.Key} x{data.Value}"))}";
    }
    
    public bool CheckRecipe(List<IngredientSO> ingredients)
    {
        var totalIngredients = ingredients.Count;
        var totalRecipeIngredients = currentRecipe.IngredientData.Sum(data => data.Value);
        //Check total ingredients
        if (totalIngredients != totalRecipeIngredients) return false;
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
            if (countDict[data.Key] != data.Value) return false;
        }
        //Check ingredient cook state
        return ingredients.All(so => so.CookState == dreamData[currentRecipe.DreamType].IngredientData[so.IngredientType]);
    }
}
