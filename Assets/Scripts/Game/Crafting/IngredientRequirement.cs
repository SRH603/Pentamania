using System;

[Serializable]
public class IngredientRequirement
{
    public string ingredientId;
    public int quantity;
    public bool isStrict;

    public IngredientRequirement(string ingredientId, int quantity, bool isStrict)
    {
        this.ingredientId = ingredientId;
        this.quantity = quantity;
        this.isStrict = isStrict;
    }

    public bool CompareIngredient(Ingredient ingredient)
    {
        if (!ingredientId.Equals(ingredient.IngredientId))
        {
            return false;
        }

        if (isStrict && quantity != ingredient.quantity)
        {
            return false;
        }

        if (quantity <= ingredient.quantity)
        {
            return true;
        }

        return false;
    }
}