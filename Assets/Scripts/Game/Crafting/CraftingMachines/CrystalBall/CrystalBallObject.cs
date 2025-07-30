

using UnityEngine;

public class CrystalBallObject : MonoBehaviour
{
    [SerializeField] private CrystalBallRecipe[] recipeList;
    private bool canConvert = true;

    private CrystalBall ballData;

    private void Awake()
    {
        ballData = new CrystalBall(recipeList);

    }

    public void IngredientObjectEntered(PassableIngredientObject ingredientObject)
    {
        if (!canConvert)
        {
            return;
        }
        if (ballData.TryConversion(ingredientObject.GetIngredient(), out IngredientStack ingredient))
            {
                canConvert = false;
                ingredientObject.SetIngredient(ingredient);

                // CREATE PARTICLES HERE


            }
    }

    public void IngredientObjectExit() {
        canConvert = true;
    }
}