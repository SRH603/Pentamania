using UnityEngine;

public class CraftingTester : MonoBehaviour
{
    public CraftingMachineObject craftingMachineObject;
    public GameObject testIngredientObject;

    void Start()
    {
        TestCrafting();
    }

    private void TestCrafting()
    {
        CraftingMachine craftingMachine = craftingMachineObject.GetCraftingMachine();
        LogCraftingMachine(craftingMachine);

        Ingredient salt = Ingredient.GetIngredientFromPath("salt");
        Ingredient sand = Ingredient.GetIngredientFromPath("sand");

        craftingMachine.InsertIngredient(salt);
        craftingMachine.InsertIngredient(salt);
        LogCraftingMachine(craftingMachine);

        craftingMachine.InsertIngredient(sand);
        craftingMachine.CheckReactions(sand);
        LogCraftingMachine(craftingMachine);
    }

    private void LogRecipe(Recipe recipe)
    {
        foreach (IngredientRequirement requirement in recipe.requiredReactants)
        {
            Debug.Log($"ing: {requirement.ingredientId}, qnt: {requirement.quantity}");
        }
        foreach (string path in recipe.productPaths)
        {
            Debug.Log("path: " + path);
        }
        foreach (Ingredient ingredient in recipe.GetProducts())
        {
            Debug.Log("result ing: " + ingredient.IngredientId);
        }
    }

    private void LogIngredient(Ingredient ingredient)
    {
        Debug.Log("IngredientId: " + ingredient.IngredientId);
        Debug.Log("Behavior: " + ingredient.IngredientBehavior);
        Debug.Log("Printing all recipes. . .");
        foreach (Recipe recipe in ingredient.Recipes)
        {
            Debug.Log("Recipe:");
            LogRecipe(recipe);
        }
        Debug.Log("Quantity: " + ingredient.quantity);
    }

    private void LogCraftingMachine(CraftingMachine machine)
    {
        Debug.Log("Machine Id: " + machine.GetId());

        if (machine.GetIngredients().Length > 0)
        {
            Debug.Log("Printing ingredients. . .");
            foreach (Ingredient ingredient in machine.GetIngredients())
            {
                LogIngredient(ingredient);
            }
        }

        Debug.Log("done.");
    }


    private void TestIngredientStateTrans()
    {
        IngredientObject ingobj = testIngredientObject.GetComponent<IngredientObject>();

        Debug.Log(ingobj.GetIngredientId());

        LogIngredient(ingobj.GetIngredient());
    }
}
