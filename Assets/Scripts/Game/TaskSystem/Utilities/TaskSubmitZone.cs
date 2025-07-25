using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TaskSubmitZone : MonoBehaviour
{
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        SolidObject solid = other.GetComponent<SolidObject>();
        if (solid == null) return;

        IngredientStack info = solid.GetIngredient();
        if (!(info is ItemStack)) return;

        ItemStack stack = (ItemStack)info;
        if (stack.IsEmpty) return;

        TaskManager manager = TaskManager.Instance;
        if (manager == null) return;

        bool needed = manager.NeedThisItem(stack.Def);
        if (!needed)
        {
            Debug.Log("[Task System] " + stack.Def.GetId() + " is not needed");
            solid.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0), ForceMode.Impulse);
            return;
        }

        Debug.Log("[Task System] Submited " + stack.Def.GetId() + " x " + stack.amount);
        manager.SubmitItem(stack.Def, stack.amount);
        Destroy(solid.gameObject);
    }
}