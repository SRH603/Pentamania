using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game/Recipe/Mortar", fileName = "MortarRecipe")]
public class MortarRecipe : ScriptableObject
{
    public string machineId = "mortar";
    public RequirementRange[] reactants;
    public SolidProductSpec[] products;
}

[Serializable]
public struct SolidProductSpec {
    public ItemDef ingredient;
    public int amount;
}