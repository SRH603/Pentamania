using UnityEngine;

public enum RequirementType
{
    CraftItem,  
    DeliverItem,
}

[System.Serializable]
public class RequirementDefinition
{
    public RequirementType requirementType;
    public string targetId;
    public int quantity = 1;
}