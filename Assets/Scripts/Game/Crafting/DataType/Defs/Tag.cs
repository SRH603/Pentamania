using System;
using UnityEngine;

[System.Serializable]
public class IngredientTag
{
    public string id;
    public float value;

    public IngredientTag(string id, float value)
    {
        this.id = id;
        this.value = value;
    }
    
    public IngredientTag(IngredientTag src)
    {
        id = src.id;
        value = src.value;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is IngredientTag other)
        {
            return id == other.id && Mathf.Approximately(this.value, other.value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode() ^ value.GetHashCode();
    }
}

[System.Serializable]
public class IngredientProperty
{
    public string id;
    public float value;

    public IngredientProperty(string id, float value)
    {
        this.id = id;
        this.value = value;
    }
}