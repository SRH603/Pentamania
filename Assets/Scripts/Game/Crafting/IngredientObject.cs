using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor.Search;
using UnityEngine;

/// <summary>
/// This is a script attached to an actual object in the unity scene to mark that object as an ingredient.
/// It is intended for that object to then be turned into a prefab.
/// This script stores the data of the ingredient, and then turns that into an Ingredient class which can then be passed between machines.
/// </summary>
public class IngredientObject : MonoBehaviour
{
    // stores which ingredient this is
    [SerializeField] private string ingredientId;
    [SerializeField] private IngredientBehaviorType ingredientBehavior;
    // how much of this ingredient is involved.
    [SerializeField] private int quantity;
    [SerializeField] private List<Tag> tags;
    // the paths to the json files for the various recipes this ingredient is a part of.
    // include EVERY recipe that can be triggered by putting this item into a machine
    //[SerializeField] private string[] recipePaths;
    // the path to the prefab that this IngredientObject is associated with. Required for instantiating the ingredient.
    //[SerializeField] private string gameObjectPath;
    //[SerializeField] private GameObject prefab;
    // stores what ingredient data this ingredient object has. This is used to pass an ingredient through a collision.
    [SerializeField] protected Ingredient ingredient;

    /// <summary>
    /// The ingredient behavior represents how the ingredient moves when handled.
    /// Solid is a stable singular object, liquid should have a liquid shader and is poured, and dust is also poured but has a dust shader.
    /// </summary>
    public enum IngredientBehaviorType
    {
        Solid,
        Liquid,
        Dust,
        Num
    }
    
    public string GetIngredientId()
    {
        return ingredientId;
    }

    /// <summary>
    /// Gets the behavior of this ingredient. 
    /// </summary>
    /// <returns>The IngredientBehavior (solid, liquid, dust)</returns>
    public IngredientBehaviorType GetIngredientBehavior()
    {
        return ingredientBehavior;
    }

    /// <summary>
    /// Gets the Ingredient of this IngredientObject.
    /// </summary>
    /// <returns>Returns the Ingredient this IngredientObject is associated with.</returns>
    public Ingredient GetIngredient()
    {
        if (ingredient == null)
        {
            return null;
        }
        return ingredient;
    }
}

[Serializable]
public class Tag
{
    public string Id;
    public float Value;
}
