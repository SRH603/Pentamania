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