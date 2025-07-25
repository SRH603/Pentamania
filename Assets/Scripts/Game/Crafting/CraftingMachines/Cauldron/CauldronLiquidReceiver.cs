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
    public float ReceiveLiquid(FluidDef def, float amount)
    {
        return cauldron.ReceiveLiquid(def, amount);
    }

}