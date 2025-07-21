using System;
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
    // the paths to the json files for the various recipes this ingredient is a part of.
    // include EVERY recipe that can be triggered by putting this item into a machine
    [SerializeField] private string[] recipePaths;
    // the path to the prefab that this IngredientObject is associated with. Required for instantiating the ingredient.
    [SerializeField] private string gameObjectPath;
    // stores what ingredient data this ingredient object has. This is used to pass an ingredient through a collision.
    private Ingredient ingredient;

    /// <summary>
    /// The ingredient behavior represents how the ingredient moves when handled.
    /// Solid is a stable singular object, liquid should have a liquid shader and is poured, and dust is also poured but has a dust shader.
    /// </summary>
    public enum IngredientBehaviorType
    {
        Solid,
        Liquid,
        Dust
    }

    void Awake()
    {
        ingredient = InitializeIngredient();
    }

    /// <summary>
    /// Converts this IngredientObject into its associated Ingredient.<br/>
    /// This is used by IngredientObject to setup its own Ingredient.
    /// </summary>
    /// <returns>The Ingredient this IngredientObject is associated with.</returns>
    private Ingredient InitializeIngredient()
    {
        return new Ingredient(this, quantity, gameObjectPath);
    }

    public Ingredient InitializeObjectlessIngredient()
    {
        return new Ingredient(ingredientId, quantity, InitializeRecipes(), gameObjectPath, ingredientBehavior);
    }

    /// <summary>
    /// Gets the recipes from the inputted paths in recipePaths. 
    /// This is run on awake and when we call ReloadRecipes.
    /// This is nessecary so that we have the actual Recipe array initialized, since that has the actual values and isn't a path.
    /// </summary>
    public Recipe[] InitializeRecipes()
    {
        Recipe[] recipes = new Recipe[recipePaths.Length];

        for (int i = 0; i < recipePaths.Length; i++)
        {
            recipes[i] = Recipe.DecodePath(recipePaths[i]);
        }

        return recipes;
    }

    // functions to get the various fields of this ingredient.
    // mostly used to copy ingredients from the prefab to an Ingredient.

    /// <summary>
    /// Gets the ingredientId of this ingredient. This is unique for types of ingredients but not for each copy of that ingredient. 
    /// </summary>
    /// <returns>The string ingredientId.</returns>
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
    /// Returns the paths to each of the recipes. Should only be used if you're copying from a prefab to a new objectless instance.
    /// </summary>
    /// <returns>The string paths for all of the recipes this ingredient can trigger.</returns>
    public string[] GetRecipePaths()
    {
        return recipePaths;
    }

    /// <summary>
    /// Gets the Ingredient of this IngredientObject.
    /// </summary>
    /// <returns>Returns the Ingredient this IngredientObject is associated with.</returns>
    public Ingredient GetIngredient()
    {
        if (ingredient == null)
        {
            return InitializeIngredient();
        }
        return ingredient;
    }
}
