
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// A recipe defines an interaction between ingredients (reactants) which result in another ingredient for a specific machine.
/// They are primarily defined by JSON files.
/// </summary>
[Serializable]
public class Recipe
{
    // stores which ingredients must be present to cause the reaction.
    public IngredientRequirement[] requiredReactants;
    // stores which ingredients are to be created after the reaction occurs.
    private Ingredient[] products;
    // for json conversion, this is a string array of the paths to IngredientObjects used to get the Ingredients this Recipe produces.
    public string[] productPaths;

    public static Recipe DecodePath(string path)
    {
        path = "Recipes/" + path;
        string json = Resources.Load<TextAsset>(path).text;
        Recipe recipe = JsonUtility.FromJson<Recipe>(json);
        recipe.InitProducts();
        return recipe;
    }

    public void InitProducts()
    {
        products = new Ingredient[productPaths.Length];

        for (int i = 0; i < productPaths.Length; i++)
        {
            products[i] = Ingredient.GetIngredientFromPath(productPaths[i]);   
        }
    }

    public Recipe(IngredientRequirement[] reactants, string[] productPaths)
    {
        requiredReactants = reactants;
        this.productPaths = productPaths;
        InitProducts();
    }

    /// <summary>
    /// Checks if there should be a reaction, and if so, returns the result of the reaction.
    /// </summary>
    /// <param name="reactants">The ingredients which are in the crafting machine to be checked.</param>
    /// <returns>The products the reaction creates, if any.</returns>
    public Ingredient[] CheckReaction(Ingredient[] reactants)
    {
        // if the given reactants and the required reactants are of differing lengths then we know they can't align.
        if (reactants.Length != requiredReactants.Length)
        {
            Debug.Log("Length mismatch, reaction impossible.");
            return null;
        }

        // iterate over the requirements and check each one.
        // we know from the first check that should all checks succeed, then we have a valid reaction. 
        foreach (IngredientRequirement requirement in requiredReactants)
        {
            bool hasMatched = false;

            foreach (Ingredient reactant in reactants)
            {
                if (requirement.CompareIngredient(reactant))
                {
                    hasMatched = true;
                    Debug.Log("Match, ing: " + reactant.IngredientId);
                    break;
                }
            }

            if (!hasMatched)
            {
                return null;
            }
        }

        // reduce the ingredients by the amount they were consumed by the recipe
        foreach (Ingredient reactant in reactants)
        {
            foreach (IngredientRequirement requirement in requiredReactants)
            {
                reactant.ReduceQuantityByRequirement(requirement);
            }
        }

        return products;
    }

    public Ingredient[] GetProducts()
    {
        return products;
    }
}
