using UnityEngine;

[CreateAssetMenu(menuName = "Game/Defs/Tag", fileName = "TagDef")]
public class IngredientTagDef : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Color color;
    [SerializeField] private float colorWeight;
    
    public string GetId()
    {
        return id;
    }

    public Color GetColor()
    {
        return color;
    }

    public float GetColorWeight()
    {
        return colorWeight;
    }
}