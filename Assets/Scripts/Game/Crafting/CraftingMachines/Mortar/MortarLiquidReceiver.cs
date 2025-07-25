using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MortarLiquidReceiver : MonoBehaviour
{
    private MortarObject mortar;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        mortar = GetComponentInParent<MortarObject>();
    }
    
    public void ReceiveLiquid(FluidDef def, float amount)
    {
        mortar?.ReceiveLiquid(def, amount);
    }
}