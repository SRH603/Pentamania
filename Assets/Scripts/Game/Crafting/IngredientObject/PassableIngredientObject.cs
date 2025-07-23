using UnityEngine;

public abstract class PassableIngredientObject : MonoBehaviour
{
    public abstract IngredientStack GetIngredient();
    public abstract void SetIngredient(IngredientStack ingredient);
}
 