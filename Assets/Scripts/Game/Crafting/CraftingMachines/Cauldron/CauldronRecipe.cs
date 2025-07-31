using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A recipe defines an interaction between ingredients (reactants) which result in another ingredient for a specific machine.
/// They are primarily defined by JSON files.
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Game/Recipe/Cauldron")]
public class CauldronRecipe : ScriptableObject
{
    // stores which ingredients must be present to cause the reaction.
    //public IngredientRequirement[] requiredReactants;
    // stores which ingredients are to be created after the reaction occurs.
    // for json conversion, this is a string array of the paths to IngredientObjects used to get the Ingredients this Recipe produces.

    //public Requirement[] reactants;
    public IngredientRequirement[] requirements;
    public ProductSpec[] products;

    public bool CheckForRecipeMatch(Dictionary<IngredientTagDef, double> tagMap)
    {
        // Compare tags to myself
        foreach (IngredientTagDef tag in tagMap.Keys)
        {
            foreach (IngredientRequirement requirement in requirements)
            {
                if (requirement.ingredientTag == tag)
                {
                    if (tagMap[tag] < requirement.min || tagMap[tag] > requirement.max)
                    {
                        return false;
                    }
                }
                else
                {
                    if (tagMap[tag] < tag.GetDefaultMin() || tagMap[tag] > tag.GetDefaultMax())
                    {
                        return false;
                    }
                }
            }
        }

        // make sure all requirements were checked
        foreach (IngredientRequirement requirement in requirements)
        {
            if (!tagMap.Keys.Contains(requirement.ingredientTag))
            {
                return false;
            }
        }

        return true;
    }
}

[Serializable]
public struct IngredientRequirement
{
    [FormerlySerializedAs("Tag")] public IngredientTagDef ingredientTag;
    public float min;
    public float max;
}

[Serializable]
public struct ProductSpec
{
    public ItemStack itemProduct;
    public FluidStack fluidProduct;
    //public IngredientDef ingredient;
    public float amount;
}

[Serializable]
public struct Requirement {
    public IngredientDef ingredient;
    public float minAmount;
    public float maxAmount;
    public bool proportional;
}
