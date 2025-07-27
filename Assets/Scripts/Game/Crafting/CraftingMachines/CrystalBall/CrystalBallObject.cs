

using UnityEngine;

public class CrystalBallObject : MonoBehaviour
{
    [SerializeField] private CrystalBallRecipe[] recipeList;

    private CrystalBall ballData;

    private void Awake()
    {
        ballData = new CrystalBall(recipeList);
    }

    public void IngredientObjectEntered(PassableIngredientObject ingredientObject)
    {
        if (ballData.TryConversion(ingredientObject.GetIngredient(), out IngredientStack ingredient))
        {
            ingredientObject.SetIngredient(ingredient);

            // CREATE PARTICLES HERE
        }
    }
}