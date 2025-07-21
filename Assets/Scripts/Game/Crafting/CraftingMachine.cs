using System;

/// <summary>
/// Handles logic regarding when and how ingredients interact. 
/// Does not handle the creation or deletion of IngredientObjects, that is the responsibility of CraftingMachineObject.
/// </summary>
public class CraftingMachine
{
    // this is the object which is responsible for handling interactions with actual objects and not logic.
    private CraftingMachineObject craftingMachineObject;
    // stores which crafting machine this is, for reference with recipes.
    private string craftingMachineId;

    /// <summary>
    /// Initializes a new instance of the logic behind this crafting machine. 
    /// Called solely by CraftingMachineObject.
    /// </summary>
    /// <param name="craftingMachineObject">The CraftingMachineObject creating this CraftingMachine.</param>
    /// <param name="craftingMachineId">Which crafting machine this is. Ex: "Cauldron".</param>
    public CraftingMachine(CraftingMachineObject craftingMachineObject, string craftingMachineId)
    {
        this.craftingMachineObject = craftingMachineObject;
        this.craftingMachineId = craftingMachineId;
    }

    /// <summary>
    /// Adds a new ingredient to this crafting machine.
    /// </summary>
    /// <param name="ingredient">The ingredient that will be added to the crafting machine.</param>
    /// <exception cref="NotImplementedException"></exception>
    public void InsertIngredient(Ingredient ingredient)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks the passed in ingredient's reactions. 
    /// For most machines, call this when a new ingredient is added, right after InsterIngredient().
    /// </summary>
    /// <param name="ingredient">The ingredient whose reactions we will check.</param>
    /// <returns>All new ingredients created by the reaction.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Ingredient[] CheckReactions(Ingredient ingredient)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks all reactions with all ingredients in this crafting machine.
    /// Probably will go unused.
    /// Don't use without checking in with Dillon about the ramifications of this method.
    /// </summary>
    /// <returns>All new ingredients created by the reactions.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Ingredient[] CheckAllReactions()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes all ingredients from this crafting machine.
    /// </summary>
    /// <param name="deleteIngredients">Whether the ingredients should be deleted via the CraftingMachineObject.</param>
    /// <exception cref="NotImplementedException"></exception>
    public void ClearAllIngredients(bool deleteIngredients)
    {
        throw new NotImplementedException();
    }

    public void ClearAllIngredients()
    {
        ClearAllIngredients(false);
    }

    /// <summary>
    /// Tries to remove a specific ingredient from this crafting machine.
    /// </summary>
    /// <param name="ingredient">The ingredient to be removed.</param>
    /// <param name="deleteIngredient">Whether the ingredient should be deleted via the CraftingMachineObject.</param>
    /// <returns>Whether the ingredient was successfully removed. (False if the ingredient wasn't in the crafting machine)</returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool RemoveIngredient(Ingredient ingredient, bool deleteIngredient)
    {
        throw new NotImplementedException();
    }

    public bool RemoveIngredient(Ingredient ingredient)
    {
        return RemoveIngredient(ingredient, false);
    }
}
