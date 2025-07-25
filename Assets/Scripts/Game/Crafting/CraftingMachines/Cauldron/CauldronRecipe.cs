using System;
using System.IO;
using UnityEngine;

/// <summary>
/// A recipe defines an interaction between ingredients (reactants) which result in another ingredient for a specific machine.
/// They are primarily defined by JSON files.
/// </summary>
[Serializable]
[CreateAssetMenu(menuName="Game/Recipe/Cauldron")]
public class CauldronRecipe: ScriptableObject
{
    // stores which ingredients must be present to cause the reaction.
    //public IngredientRequirement[] requiredReactants;
    // stores which ingredients are to be created after the reaction occurs.
    // for json conversion, this is a string array of the paths to IngredientObjects used to get the Ingredients this Recipe produces.
    
    public RequirementRange[] reactants;
    public ProductSpec[] products;
    public ExplosionCurve explosionCurve;
}

[Serializable]
public struct RequirementRange {
    public IngredientDef ingredient;
    public float minAmount;
    public float maxAmount;
    public bool proportional;
}
[Serializable]
public struct ProductSpec {
    public IngredientDef ingredient;
    public float amount;
}

[Serializable]
public struct ExplosionCurve
{
    public AnimationCurve deviationToPower;
}
