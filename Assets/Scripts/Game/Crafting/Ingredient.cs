using System;
using UnityEngine;
using IngredientBehaviorType = IngredientObject.IngredientBehaviorType;

/// <summary>
/// An ingredient is any item that we need to either create from a recipe or use in a recipe.
/// Essentially, all interactable materials.<br/>
/// <br/>
/// Each ingredient has:
/// <br/> - An id so that other classes can check if the ingredient matches the type it needs.
/// <br/> - A behavior that determines how the ingredient moves physically.
/// <br/> - A quantity, either used in recipes to ask for a specific amount of the ingredient or to store how much of this ingredient is bundled in this object.
/// <br/> - An array of Recipes that this ingredient is used in. <br/>
/// The Ingredient class is just for storing the data associated with the ingredient. 
/// The actual script attached to an object is the IngredientObject script, which is responsible for setting all these variables.<br/>
/// <br/>
/// </summary>
public class Ingredient
{
    // stores which ingredient this is
    public string IngredientId { get; private set; }
    // Determines whether this ingredient is a solid, liquid, or dust.
    public IngredientBehaviorType IngredientBehavior { get; private set; }
    // how much of this ingredient is involved.
    public int quantity;
    // the actual recipes that are involved
    public Recipe[] Recipes { get; private set; }
    // stores which game object prefab this ingredient is associated with.
    // this is necessary to convert Ingredients to IngredientObjects.
    // Recipes are checked on a first come first serve basis and only one recipe can occur from a single ingredient.
    private IngredientObject ingredientObject;
    // defines a path to the game object that represents this ingredient.
    // Nessecary to convert Ingredients to IngredientObjects if the IngredientObject has already been detached.
    private string gameObjectPath;

    public Ingredient(string ingredientId, int quantity, Recipe[] recipes, string gameObjectPath, IngredientBehaviorType ingredientBehavior)
    {
        IngredientId = ingredientId;
        this.quantity = quantity;
        Recipes = recipes;
        this.gameObjectPath = gameObjectPath;
        IngredientBehavior = ingredientBehavior;
    }

    public Ingredient(IngredientObject ingredientObject, int quantity, string gameObjectPath)
    {
        this.ingredientObject = ingredientObject;
        IngredientId = ingredientObject.GetIngredientId();
        this.quantity = quantity;
        Recipes = ingredientObject.InitializeRecipes();
        this.gameObjectPath = gameObjectPath;
        IngredientBehavior = ingredientObject.GetIngredientBehavior();
    }

    public Ingredient[] CheckReaction(Ingredient[] ingredients)
    {
        // Loop through the recipes in this ingredient
        foreach (Recipe recipe in Recipes)
        {
            // check the reaction, and if it produces a result, return that.
            // this means that recipes are checked in a first come, first serve basis, and later recipes are ignored.
            Ingredient[] products = recipe.CheckReaction(ingredients);
            if (products != null)
            {
                return products;
            }
        }

        return null;
    }

    /// <summary>
    /// Removes the IngredientObject component of this ingredient, turning it into pure data (for stuff like being in a cauldron.)
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void DecoupleIngredientObject()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Converts this ingredient to a gameObject. </br>
    /// This can return null if the Ingredient has been disconnected from an IngredientObject.
    /// </summary>
    /// <returns>The new game object with an ingredientObject script.</returns>
    public GameObject ConvertToGameObject()
    {
        // if we don't have our ingredient object, we have nothing to return.
        if (ingredientObject == null)
        {
            return null;
        }
        return ingredientObject.gameObject;
    }

    /// <summary>
    /// Gets the prefab this ingredient is associated with, for converting it to a physical game object.
    /// After calling this and instantiating the prefab, you need to transfer this one's quantity to that one.
    /// </summary>
    /// <returns>A prefab to be instantiated.</returns>
    public GameObject GetPrefab()
    {
        return Resources.Load<GameObject>(gameObjectPath);
    }

    /// <summary>
    /// Gets a new objectless ingredient from a provided path.
    /// </summary>
    /// <param name="path">The path to get the ingredient from. The head is Resources/Prefabs/Ingredients</param>
    /// <returns></returns>
    public static Ingredient GetIngredientFromPath(string path)
    {
        GameObject masterIngredient = Resources.Load<GameObject>("Prefabs/Ingredients/" + path);
        IngredientObject ingredientObject = masterIngredient.GetComponent<IngredientObject>();
        return ingredientObject.InitializeObjectlessIngredient();
    }

    public void ReduceQuantityByRequirement(IngredientRequirement requirement)
    {
        if (requirement.ingredientId.Equals(IngredientId))
        {
            quantity -= requirement.quantity;
        }
    }  
}
