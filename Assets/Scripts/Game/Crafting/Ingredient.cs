using UnityEngine;

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
/// Despite this, all Ingredient classes also come with an IngredientGameObject field so they can be turned into an object if necessary.
/// </summary>
public class Ingredient
{
    // stores which ingredient this is
    public string IngredientId { get; private set; }
    // defines how this ingredient should behave when moved. 
    public IngredientBehaviorType IngredientBehavior { get; private set; }
    // how much of this ingredient is involved.
    public int Quantity { get; private set; }
    // the actual recipes that are involved
    public Recipe[] Recipes { get; private set; }
    // stores which game object prefab this ingredient is associated with.
    // this is necessary to convert Ingredients to IngredientObjects.
    private IngredientObject ingredientObject;

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

    /// <summary>
    /// Used to instantiate a new Ingredient.
    /// </summary>
    /// <param name="ingredientId"></param>
    /// <param name="ingredientBehavior"></param>
    /// <param name="quantity"></param>
    /// <param name="recipes"></param>
    public Ingredient(string ingredientId, IngredientBehaviorType ingredientBehavior, int quantity, Recipe[] recipes, IngredientObject ingredientObject)
    {
        IngredientId = ingredientId;
        IngredientBehavior = ingredientBehavior;
        Quantity = quantity;
        Recipes = recipes;
        this.ingredientObject = ingredientObject;
    }

    /// <summary>
    /// Converts this ingredient to a gameObject. 
    /// </summary>
    /// <returns>The new game object with an ingredientObject script.</returns>
    public GameObject ConvertToGameObject()
    {
        return ingredientObject.gameObject;
    }
}
