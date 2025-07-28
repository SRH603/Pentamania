using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CauldronObject))]
public class CauldronEffects : MonoBehaviour
{
    [SerializeField] private VisualEffect surfaceBubbleEffect;
    private CauldronObject cauldron;

    void Start()
    {
        cauldron = GetComponent<CauldronObject>();
    }
    
    void Update()
    {
        surfaceBubbleEffect.SetVector4("Base Color", cauldron.GetCurrentLiquidColor());
        surfaceBubbleEffect.SetFloat("Strength", cauldron.GetCurrentStrength());
    }
}