using System;

[Serializable]
public class IngredientRequirement
{
    public string IngredientId { get; private set; }
    public int Quantity { get; private set; }

    public IngredientRequirement(string ingredientId, int quantity)
    {
        IngredientId = ingredientId;
        Quantity = quantity;
    }
}