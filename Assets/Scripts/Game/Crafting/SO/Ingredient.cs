using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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

[CreateAssetMenu(menuName="Game/Ingredient")]
public class Ingredient : ScriptableObject 
{
    // stores which ingredient this is
    [SerializeField] public string ingredientId;
    // Determines whether this ingredient is a solid, liquid, or dust.
    [SerializeField] public IngredientBehaviorType ingredientBehavior;
    // how much of this ingredient is involved.
    [SerializeField] public float quantity;
    // stores which game object prefab this ingredient is associated with.
    // this is necessary to convert Ingredients to IngredientObjects.
    [SerializeField] public List<Tag> tags;

    [SerializeField] public IngredientObject prefab;
    // Recipes are checked on a first come first serve basis and only one recipe can occur from a single ingredient.
    private IngredientObject ingredientObject;
    // defines a path to the game object that represents this ingredient.
    // Nessecary to convert Ingredients to IngredientObjects if the IngredientObject has already been detached.
    // private string gameObjectPath;
    
    

    public Ingredient(string ingredientId, float quantity, IngredientBehaviorType ingredientBehavior)
    {
        this.ingredientId = ingredientId;
        this.quantity = quantity;
        //this.gameObjectPath = gameObjectPath;
        this.ingredientBehavior = ingredientBehavior;
    }

    public Ingredient(IngredientObject ingredientObject, float quantity)
    {
        this.ingredientObject = ingredientObject;
        this.ingredientId = ingredientObject.GetIngredientId();
        this.quantity = quantity;
        //this.gameObjectPath = gameObjectPath;
        this.ingredientBehavior = ingredientObject.GetIngredientBehavior();
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
        return prefab.gameObject;
    }
}
