
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe/CrystalBall")]
public class CrystalBallRecipe : ScriptableObject
{
    [SerializeField] private IngredientDef reactant;
    [SerializeField] protected IngredientDef product;

    void Awake()
    {
        if (reactant.GetType() != product.GetType())
        {
            throw new ArgumentException("A crystal ball cannot convert between liquids and solids!");
        }
    }

    public IngredientStack GetReactant()
    {
        if (reactant.GetType() == typeof(ItemDef))
        {
            return new ItemStack((ItemDef)reactant, 1);
        }
        else if (reactant.GetType() == typeof(FluidDef))
        {
            return new FluidStack((FluidDef)reactant, 1);
        }
        throw new Exception("A crystal ball recipe was found to contain a reactant that was neither a fluid nor an item.");
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

    public virtual bool TryConversion(IngredientStack ingredient, out IngredientStack product)
    {
        ingredient = null;
        product = null;
        return false;
    } 
}