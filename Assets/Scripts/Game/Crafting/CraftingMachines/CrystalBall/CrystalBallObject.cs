

using UnityEngine;
using UnityEngine.VFX;

public class CrystalBallObject : MonoBehaviour
{
    [SerializeField] private CrystalBallRecipe[] recipeList;
    private bool canConvert = true;

    private CrystalBall ballData;

    public VisualEffect craftingEffect;

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
            craftingEffect.Play();

        }
    }

    public void IngredientObjectExit() {
        canConvert = true;
    }
}