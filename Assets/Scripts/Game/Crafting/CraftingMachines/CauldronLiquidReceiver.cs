using UnityEngine;

/// <summary>
/// 挂在坩埚“液体接收触发器”Collider 上，
/// 把来自 Container 的液体传给坩埚内部 <see cref="CraftingMachine"/>。
/// </summary>
[RequireComponent(typeof(Collider))]
public class CauldronLiquidReceiver : MonoBehaviour
{
    private Cauldron _cauldron;

    private void Awake()
    {
        _cauldron = GetComponentInParent<Cauldron>();
        if (!_cauldron)
            Debug.LogError("[CauldronLiquidReceiver] 未找到父级 Cauldron！");
    }

    /// <summary>被 <see cref="LiquidContainer"/> 调用。</summary>
    public void ReceiveLiquid(Ingredient ing, float amount)
    {
        if (!_cauldron) return;
        _cauldron.ReceiveLiquid(ing, amount);
    }
}