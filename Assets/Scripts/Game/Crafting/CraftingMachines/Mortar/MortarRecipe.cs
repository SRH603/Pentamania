using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe/Mortar", fileName = "MortarRecipe")]
public class MortarRecipe : ScriptableObject
{
    [Serializable]
    public struct ReactantSpec { public ItemDef ingredient; public int amount; }

    [Header("Reactants (Solid)")]
    public ReactantSpec[] reactants;

    [Header("Product (FluidStack)")]
    public FluidStack product;     // 直接设为 FluidStack，可在 Inspector 中配置 tag 列表
}