using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game/Recipe/BunsenBurner", fileName = "BunsenBurnerRecipe")]
public class BunsenBurnerRecipe : ScriptableObject
{
    public ItemDef input;
    public ItemDef output;
    public float cookTime = 3f;
}