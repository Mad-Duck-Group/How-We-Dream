using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum SkillTypes
{
    Currency,
    Ingredient
}

[Serializable]
public abstract class Skill
{
    [SerializeField] private string skillName;
    public string SkillName => skillName;
    [SerializeField] private string description;
    public string Description => description;
    [SerializeField] private SkillTypes skillType;
    public SkillTypes SkillType => skillType;
    [SerializeField] private int skillCost;
    public int SkillCost => skillCost;

    public abstract void ApplySkill();
}

[Serializable]
public class CurrencySkill : Skill
{
    [SerializeField] private int currencyValue;
    public int CurrencyValue => currencyValue;
    
    public override void ApplySkill()
    {
        InventoryManager.Instance.ChangeCurrency(currencyValue);
    }
}

[Serializable]
public class IngredientSkill : Skill
{
    [SerializedDictionary("Ingredient Type", "Amount")]
    public SerializedDictionary<IngredientTypes, int> ingredientData;

    public override void ApplySkill()
    {
        foreach (var ingredient in ingredientData)
        {
            InventoryManager.Instance.ChangeIngredientAmount(ingredient.Key, ingredient.Value);
        }
    }
}
