using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CauldronLiquidReceiver : MonoBehaviour
{
    private CauldronObject cauldron;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        cauldron = GetComponentInParent<CauldronObject>();
    }
    public void ReceiveLiquid(FluidDef def, float amount)
    {
        cauldron?.ReceiveLiquid(def, amount);
    }
}