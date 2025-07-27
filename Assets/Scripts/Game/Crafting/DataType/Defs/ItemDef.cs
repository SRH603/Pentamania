using UnityEngine;

[CreateAssetMenu(menuName = "Game/Defs/Item", fileName = "ItemDef")]
public class ItemDef : IngredientDef
{
    [SerializeField] private int maxStack = int.MaxValue;
    [SerializeField] private GameObject baseIngredient; // set this to a prefab to get all the stats automatically

    public int GetMaxStack()
    {
        return maxStack;
    }

    public Mesh GetMesh()
    {
        return baseIngredient.GetComponent<MeshFilter>().sharedMesh;
    }

    public Material GetMaterial()
    {
        return baseIngredient.GetComponent<MeshRenderer>().sharedMaterial;
    }
    
    public Vector3 GetScale()
    {
        //Debug.Log(baseIngredient.transform.localScale.ToString());
        return baseIngredient.transform.localScale;
    }
    
    public IngredientProperty[] GetProperties()
    {
        return properties;
    }
}