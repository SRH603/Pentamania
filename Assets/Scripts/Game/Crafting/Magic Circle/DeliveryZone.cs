using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IngredientObject ingObj = other.GetComponent<IngredientObject>();
        if (ingObj == null) return;
        
        Ingredient ing = ingObj.GetIngredient();
        TaskManager.Instance.OnItemDelivered(ing.IngredientId, ing.quantity);
        Destroy(other.gameObject);
        
        Debug.Log($"{ingObj.name} is submitted");
    }
}