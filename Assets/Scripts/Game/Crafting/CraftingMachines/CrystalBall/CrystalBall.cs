using System.Collections.Generic;

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

            if (MatchReverseRecipe(recipe, ingredient))
            {
                product = recipe.GetReactant();
                return true;
            }
        }

        product = null;
        return false;
    }

    private bool MatchRecipe(CrystalBallRecipe recipe, IngredientStack ingredient)
    {
        return ingredient.GetAbstractDef().GetId().Equals(recipe.GetReactant().GetAbstractDef().GetId());
    }

    private bool MatchReverseRecipe(CrystalBallRecipe recipe, IngredientStack ingredient)
    {
        return ingredient.GetAbstractDef().GetId().Equals(recipe.GetProduct().GetAbstractDef().GetId());
    }
}
