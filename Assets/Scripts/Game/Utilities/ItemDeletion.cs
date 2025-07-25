using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ItemDeletion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("111rytu");
        if (other.TryGetComponent<XRGrabInteractable>(out var interactable))
        {
            if (interactable.gameObject != gameObject)
            {
                Destroy(interactable.gameObject);
            }
        }
    }
}