using UnityEngine;

public abstract class IngredientDef : ScriptableObject
{
    [SerializeField] protected string ingredientId;

    public string GetId()
    {
        return ingredientId;
    }
}

[CreateAssetMenu(menuName = "Game/Defs/Item", fileName = "ItemDef")]
public class ItemDef : IngredientDef
{
    [SerializeField] private int maxStack = int.MaxValue;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private Vector3 scale = new Vector3(1,1,1);

    public int GetMaxStack()
    {
        return maxStack;
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    public Material GetMaterial()
    {
        return material;
    }
    
    public Vector3 GetScale()
    {
        return scale;
    }
}

[CreateAssetMenu(menuName = "Game/Defs/Fluid", fileName = "FluidDef")]
public class FluidDef : IngredientDef
{
    [SerializeField] private Material material;

    public Material GetMaterial()
    {
        return material;
    }
}