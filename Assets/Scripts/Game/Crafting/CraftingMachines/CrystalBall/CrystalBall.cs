using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CrystalBall
{
    private readonly List<CrystalBallRecipe> recipes;

    public CrystalBall(IEnumerable<CrystalBallRecipe> recipes)
    {
        this.recipes = new List<CrystalBallRecipe>(recipes);
    }

    public bool TryConversion(IngredientStack ingredient, out IngredientStack product)
    {
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(recipe, ingredient))
            {
                product = recipe.GetProduct();
                return true;
            }
        }

        product = null;
        return false;
    }

    private bool MatchRecipe(CrystalBallRecipe recipe, IngredientStack ingredient)
    {
        return true;
        //return ingredient.GetId().Equals(recipe.GetReactant().GetId());
    }
}
