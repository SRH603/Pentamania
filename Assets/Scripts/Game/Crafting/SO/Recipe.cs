using System;
using System.IO;
using UnityEngine;

/// <summary>
/// A recipe defines an interaction between ingredients (reactants) which result in another ingredient for a specific machine.
/// They are primarily defined by JSON files.
/// </summary>
[Serializable]
[CreateAssetMenu(menuName="Game/Recipe")]
public class Recipe: ScriptableObject
{
    public string machineId; // "Cauldron"
    // stores which ingredients must be present to cause the reaction.
    //public IngredientRequirement[] requiredReactants;
    // stores which ingredients are to be created after the reaction occurs.
    // for json conversion, this is a string array of the paths to IngredientObjects used to get the Ingredients this Recipe produces.
    
    public RequirementRange[] reactants;  // 支持范围/比例
    public ProductSpec[] products;

    // 偏差评价参数
    public ExplosionCurve explosionCurve; // 根据偏差计算爆炸强度
}

[Serializable]
public struct RequirementRange {
    public Ingredient ingredient;
    public float minAmount;
    public float maxAmount;
    public bool proportional; // 若 true 使用比例系数（归一化）
}
[Serializable]
public struct ProductSpec {
    public Ingredient ingredient;
    public float amount;
}
[Serializable]
public struct ExplosionCurve {
    public AnimationCurve deviationToPower; // 0=完美, >0 = 偏差
}
