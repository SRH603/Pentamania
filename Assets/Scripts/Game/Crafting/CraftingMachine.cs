using System;
using System.Collections.Generic;
using System.Linq;

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
    private List<Ingredient> ingredients;

    /// <summary>
    /// Toggles whether this CraftingMachine will repeat a reaction if possible.
    /// </summary>
    public bool batchProduction = true;
    /// <summary>
    /// Toggles whether this CraftingMachine will try a reaction with all products after a reaction as well.
    /// Might be buggy / bad performance.
    /// </summary>
    public bool recursiveProduction = false;

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
        ingredients = new List<Ingredient>();
    }

    /// <summary>
    /// Adds a new ingredient to this crafting machine.
    /// </summary>
    /// <param name="ingredient">The ingredient that will be added to the crafting machine.</param>
    public void InsertIngredient(Ingredient ingredient)
    {
        // if an existing ingredient has the same id, combine them into one stack
        foreach (Ingredient existingIngredient in ingredients)
        {
            if (existingIngredient.IngredientId.Equals(ingredient.IngredientId))
            {
                existingIngredient.quantity += ingredient.quantity;
                return;
            }
        }

        // otherwise, add the ingredient to the list of ingredients we have
        ingredients.Add(ingredient);
    }

    /// <summary>
    /// Checks the passed in ingredient's reactions. 
    /// For most machines, call this when a new ingredient is added, right after InsterIngredient().
    /// </summary>
    /// <param name="ingredient">The ingredient whose reactions we will check.</param>
    /// <returns>All new ingredients created by the reaction.</returns>
    public Ingredient[] CheckReactions(Ingredient ingredient)
    {
        Ingredient[] productArray = ingredient.CheckReaction(ingredients.ToArray());
        if (productArray == null) {
            return null;
        }
        List<Ingredient> products = productArray.ToList();
        
        foreach (Ingredient product in products)
        {
            InsertIngredient(product);
        }

        // check for products with zero quantity
        ClearDepletedIngredients();

        // if we have batch production turned on, will attempt this reaction a second time
        if (batchProduction)
        {
            Ingredient[] extraProducts = CheckReactions(ingredient);
            if (extraProducts != null)
            {
                foreach (Ingredient product in extraProducts)
                {
                    products.Add(product);
                }
            }
        }

        // if we have recursive production, try production on each product
        // this is probably bad code
        if (recursiveProduction)
        {
            List<Ingredient> recursiveExtraProducts = new List<Ingredient>();
            foreach (Ingredient product in products)
            {
                Ingredient[] extraProducts = CheckReactions(product);
                if (extraProducts != null)
                {
                    foreach (Ingredient extraProduct in extraProducts)
                    {
                        recursiveExtraProducts.Add(extraProduct);
                    }
                }
            }
            foreach (Ingredient product in recursiveExtraProducts)
            {
                products.Add(product);
            }
        }

        return products.ToArray();
    }

    // gets rid of all ingredients that have zero quantity left
    private void ClearDepletedIngredients()
    {
        List<Ingredient> newIngredients = new List<Ingredient>();

        foreach (Ingredient i in ingredients)
        {
            if (i.quantity > 0)
            {
                newIngredients.Add(i);
            }
        }

        ingredients = newIngredients;
    }

    /// <summary>
    /// Checks all reactions with all ingredients in this crafting machine.
    /// Probably will go unused.
    /// Don't use without checking in with Dillon about the ramifications of this method.
    /// </summary>
    /// <returns>All new ingredients created by the reactions.</returns>
    public Ingredient[] CheckAllReactions()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes all ingredients from this crafting machine.
    /// </summary>
    /// <param name="deleteIngredients">Whether the ingredients should be deleted via the CraftingMachineObject.</param>
    public void ClearAllIngredients(bool deleteIngredients)
    {
        if (deleteIngredients)
        {
            // safely delete the ingredients without iteration
            while (ingredients.Count > 0)
            {
                craftingMachineObject.DestroyIngredient(ingredients[0]);
            }
        }

        ingredients.Clear();
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
    public bool RemoveIngredient(Ingredient ingredient, bool deleteIngredient)
    {
        // if the ingredient is not contained in this crafting machine, return false.
        if (!ingredients.Contains(ingredient))
        {
            return false;
        }

        ingredients.Remove(ingredient);

        if (deleteIngredient)
        {
            craftingMachineObject.DestroyIngredient(ingredient);
        }

        return true;
    }

    public bool RemoveIngredient(Ingredient ingredient)
    {
        return RemoveIngredient(ingredient, false);
    }

    public string GetId()
    {
        return craftingMachineId;
    }

    public Ingredient[] GetIngredients()
    {
        return ingredients.ToArray();
    }
}
