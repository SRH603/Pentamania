
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe/CrystalBall")]
public class CrystalBallRecipe : ScriptableObject
{
    [SerializeField] private IngredientDef reactant;
    [SerializeField] private IngredientDef product;

    void Awake()
    {
        if (reactant.GetType() != product.GetType())
        {
            throw new ArgumentException("A crystal ball cannot convert between liquids and solids!");
        }
    }

    public IngredientDef GetReactant()
    {
        return reactant;
    }

    public IngredientStack GetProduct()
    {
        if (product.GetType() == typeof(ItemDef))
        {
            return new ItemStack((ItemDef)product, 1);
        }
        else if (product.GetType() == typeof(FluidDef))
        {
            return new FluidStack((FluidDef)product, 1);
        }
        throw new Exception("A crystal ball recipe was found to contain a product that was neither a fluid nor an item.");
    }
}