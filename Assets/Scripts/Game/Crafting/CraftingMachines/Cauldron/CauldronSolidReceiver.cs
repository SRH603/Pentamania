using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CauldronSolidReceiver : MonoBehaviour
{
    private CauldronObject cauldron;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        cauldron = GetComponentInParent<CauldronObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        SolidObject solid = other.GetComponent<SolidObject>();
        if (!solid) return;
        cauldron.ReceiveSolid(solid);
    }
}