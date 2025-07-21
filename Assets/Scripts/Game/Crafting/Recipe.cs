
using System;

/// <summary>
/// A recipe defines an interaction between ingredients (reactants) which result in another ingredient for a specific machine.
/// They are primarily defined by JSON files.
/// </summary>
[Serializable]
public class Recipe
{
    // stores which ingredients must be present to cause the reaction.
    public IngredientRequirement[] Reactants { get; private set; }
    // stores which ingredients are to be created after the reaction occurs.
    //private Ingredient[] products;
    // for json conversion, this is a string array of the paths to IngredientObjects used to get the Ingredients this Recipe produces.
    public string[] ProductPaths { get; private set; }

    public static Recipe DecodeJSON(string json)
    {
        throw new NotImplementedException();
    }

    public Recipe(IngredientRequirement[] reactants, string[] productPaths)
    {
        Reactants = reactants;
        ProductPaths = productPaths;
    }
}
