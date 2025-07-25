using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BunsenBurnerSolidReceiver : MonoBehaviour
{
    private BunsenBurnerObject burner;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        burner = GetComponentInParent<BunsenBurnerObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SolidObject solidObject = other.GetComponent<SolidObject>();
        if (!solidObject)
            return;
        if (solidObject.GetType() != typeof(SolidObject))
            return;
        
        burner.SolidEntered(solidObject);
    }

    private void OnTriggerExit(Collider other)
    {
        SolidObject solidObject = other.GetComponent<SolidObject>();
        if (!solidObject)
            return;
        if (solidObject.GetType() != typeof(SolidObject))
            return;
        
        burner.SolidExited(solidObject);
    }
}