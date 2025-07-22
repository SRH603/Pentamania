using UnityEngine;

/// <summary>
/// 继承自 IngredientObject 的“液体容器”版本：
///     • 保存当前液体 <see cref="Ingredient"/> 与容量上限；
///     • 拥有每秒倾倒速率 pourRateLps (Litre Per Second)；
///     • 当与 Cauldron 的液体接收触发器接触且俯仰角 > angleThreshold 时自动倾倒；
///     • 提供事件/虚函数便于播放特效、声音。
/// </summary>
public class LiquidContainer : IngredientObject
{
    [Header("容器参数")]
    [SerializeField] private float capacity = 10f;          // 容量上限
    [SerializeField] private float pourRateLps = 1f;        // L/s
    [SerializeField] private float angleThreshold = 45f;    // 触发倾倒所需倾斜角
    [SerializeField] private ParticleSystem pourEffect;     // 倾倒特效（可空）

    private bool _isPouring;
    private CauldronLiquidReceiver _receiverCache;

    // 当前液体量 = ingredient.quantity (继承字段)

    private void Start()
    {
        Debug.Log($"[{name}] 初始化容量 {capacity}，当前液体 {ingredient.quantity}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 进入坩埚液体触发器
        _receiverCache = other.GetComponent<CauldronLiquidReceiver>();
        if (_receiverCache)
            Debug.Log($"[{name}] 进入 CauldronLiquidReceiver");
    }

    private void OnTriggerExit(Collider other)
    {
        // 离开触发器，停止倾倒
        if (other.GetComponent<CauldronLiquidReceiver>())
        {
            StopPour();
            _receiverCache = null;
            Debug.Log($"[{name}] 离开 CauldronLiquidReceiver");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("111");
        if (!_receiverCache) return;              // 必须是在液体触发器里
        if (ingredient.quantity <= 0f) return;    // 没液体可倒

        float angle = Vector3.Angle(transform.up, Vector3.up);
        Debug.Log(angle);
        if (angle < angleThreshold)
        {
            // 倾角不足
            StopPour();
            return;
        }

        // 满足倾倒条件
        float delta = Mathf.Min(pourRateLps * Time.deltaTime, ingredient.quantity);
        ingredient.quantity -= delta;
        _receiverCache.ReceiveLiquid(ingredient, delta);

        if (!_isPouring) StartPour(); // 开始特效

        if (Mathf.Approximately(ingredient.quantity, 0f))
        {
            Debug.Log($"[{name}] 已倾空");
            StopPour();
        }
    }

    #region 特效接口
    private void StartPour()
    {
        _isPouring = true;
        if (pourEffect) pourEffect.Play();
        Debug.Log($"[{name}] StartPour()");
    }

    private void StopPour()
    {
        if (!_isPouring) return;
        _isPouring = false;
        if (pourEffect) pourEffect.Stop();
        Debug.Log($"[{name}] StopPour()");
    }
    #endregion

    #region API
    public bool TryAddLiquid(float amount)
    {
        float newVol = ingredient.quantity + amount;
        if (newVol > capacity) return false;
        ingredient.quantity = newVol;
        Debug.Log($"[{name}] 手动加液体 {amount}，现 {ingredient.quantity}/{capacity}");
        return true;
    }
    #endregion
}