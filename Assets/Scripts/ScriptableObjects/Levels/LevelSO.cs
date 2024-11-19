using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class LevelSO : ScriptableObject
{
    [SerializeField] private List<RecipeSO> recipePool;
    public List<RecipeSO> RecipePool => recipePool;
    [SerializeField] private List<RecipeSO> finiteRecipes;
    public List<RecipeSO> FiniteRecipes => finiteRecipes;
    [SerializeField] private bool randomRecipe;
    public bool RandomRecipe => randomRecipe;
    [SerializeField, ShowIf(nameof(randomRecipe))] RecipeSO randomPrototype;
    public RecipeSO RandomPrototype => randomPrototype;
    [SerializeField, ShowIf(nameof(randomRecipe))] private bool infinite;
    [SerializeField, HideIf(nameof(infinite))] private bool usePool;
    public bool UsePool => usePool;
    public bool Infinite => infinite;
    [SerializeField, ShowIf(nameof(ShowToPool))] private bool randomToPool;
    public bool RandomToPool => randomToPool;
    [SerializeField, ShowIf(nameof(ShowRandomRange))] private Vector2 randomAmountRange;
    
    [SerializeField] private float timeLimit;
    public float TimeLimit => timeLimit;
    [SerializeField] private bool hasQuota;
    public bool HasQuota => hasQuota;
    [SerializeField, ShowIf(nameof(hasQuota))] private int quota;
    public int Quota => quota;
    [SerializeField] private float boogeyManQuota;
    public float BoogeyManQuota => boogeyManQuota;
    [SerializeField] private bool endless;
    public bool Endless => endless;
    public Vector2 RandomAmountRange => randomAmountRange;
    private bool ShowToPool => randomRecipe && usePool && !infinite;
    private bool ShowRandomRange => randomRecipe && !infinite;
}
