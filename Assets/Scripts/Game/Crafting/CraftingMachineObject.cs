using System.Diagnostics;
using UnityEngine;

/// <summary>
/// This is the base script attached to a CraftingMachine (anything that can turn ingredients into another ingredient).
/// The CraftingMachineObject script is responsible for registering a collision with an ingredient, 
/// and passing that ingredient to the logical script (CraftingMachine).
/// It also then creates an ingredient or deletes an ingredient, and also is responsible for informing the crafting machine 
/// when an ingredient is no longer present.
/// </summary>
public abstract class CraftingMachineObject : MonoBehaviour
{
    // Stores what crafting machine this object will pass its collisions to.
    protected CraftingMachine craftingMachine;

    /// <summary>
    /// Creates a copy of the prefab of the ingredient at whatever location this machine outputs resources at. 
    /// </summary>
    /// <param name="ingredient">The ingredient to be created.</param>
    public abstract void CreateIngredient(Ingredient ingredient); // this should call CreateIngredientGameObject with an overwritten position

    /// <summary>
    /// Destroys a given ingredient. Not the same as removing the object component of the ingredient.
    /// </summary>
    /// <param name="ingredient">The ingredient to be destroyed.</param>
    public abstract void DestroyIngredient(Ingredient ingredient);

    /// <summary>
    /// Initializes this CraftingMachineObject's CraftingMachine. This is mandatory to be implemented by all children.
    /// </summary>
    protected abstract void InitCraftingMachine();

    void Awake()
    {
        InitCraftingMachine();
    }

    public void InsertIngredient(GameObject ingredientGameObject)
    {
        // Get the ingredient's ingredient script
        Ingredient ingredient = ingredientGameObject.GetComponent<IngredientObject>().GetIngredient();

        // Add the ingredient to the crafting machine
        craftingMachine.InsertIngredient(ingredient);
    }

    /// <summary>
    /// Creates a new game object for the ingredient at a given position.
    /// </summary>
    /// <param name="position">The position to create the object.</param>
    /// <param name="ingredient">The ingredient to be created.</param>
    protected void CreateIngredientGameObject(Transform position, Ingredient ingredient)
    {
        GameObject gameObject = ingredient.GetPrefab();
        Instantiate(gameObject, transform);
    }

    public CraftingMachine GetCraftingMachine()
    {
        return craftingMachine;
    }
}