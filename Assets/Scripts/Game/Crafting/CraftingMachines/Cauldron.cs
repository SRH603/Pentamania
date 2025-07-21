using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Cauldron : CraftingMachineObject
{
    #region 字段
    [SerializeField] private Transform outputPoint;
    #endregion

    #region Mono

    void OnValidate()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        IngredientObject ingObj = other.GetComponent<IngredientObject>();
        
        if (ingObj == null) return;
        InsertIngredient(other.gameObject);

        // TODO: Create the ingredients spawned by this line
        craftingMachine.CheckReactions(other.gameObject.GetComponent<IngredientObject>().GetIngredient());
    }

    #endregion

    #region CraftingMachineObject

    public override void CreateIngredient(Ingredient ingredient)
    {
        GameObject prefab = ingredient.GetPrefab();
        if (prefab == null)
        {
            Debug.LogWarning($"[{name}] ConvertToGameObject Failed {ingredient.IngredientId}");
            return;
        }

        Vector3 spawnPos = outputPoint != null ? outputPoint.position : transform.position + Vector3.up * 0.5f;
        GameObject clone = Instantiate(prefab, spawnPos, Quaternion.identity);
        
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb != null) rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }

    public override void DestroyIngredient(Ingredient ingredient)
    {
        GameObject go = ingredient.ConvertToGameObject();
        if (go == null) return;

        Destroy(go);
    }

    protected override void InitCraftingMachine()
    {
        craftingMachine = new CraftingMachine(this, "Cauldron");
    }

    #endregion
}