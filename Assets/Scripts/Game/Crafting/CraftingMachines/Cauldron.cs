using UnityEngine;

/// <summary>
/// 场景中的坩埚对象：
/// 1) 监听 Trigger，把 IngredientObject 转成数据插入 CraftingMachine；
/// 2) 超过容量即爆炸；
/// 3) 每次库存变化立即尝试配方；
/// 4) 负责把产物实例化到场景。
/// </summary>
[RequireComponent(typeof(Collider))]
public class Cauldron : CraftingMachineObject
{
    [Header("参数")]
    [SerializeField] private float maxCapacity = 20f; // 超过总量直接爆炸
    [SerializeField] private Transform outputPoint;
    [SerializeField] private Recipe[] recipes;        // Inspector 绑定
    
    [Header("触发器")]
    [SerializeField] private Collider liquidReceiver;   // 在 Inspector 拖入“液体碰撞体”

    private Collider _trigger;

    #region Mono
    private void Reset()
    {
        // 方便一键设置 Trigger
        _trigger = GetComponent<Collider>();
        if (_trigger) _trigger.isTrigger = true;
    }

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
        _trigger.isTrigger = true;
        InitCraftingMachine();
        
        base.Awake(); // 会调用 InitCraftingMachine()
        if (!liquidReceiver)
        {
            liquidReceiver = GetComponentInChildren<CauldronLiquidReceiver>()?.GetComponent<Collider>();
        }
        if (liquidReceiver) liquidReceiver.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        IngredientObject ingObj = other.GetComponent<IngredientObject>();
        if (!ingObj) return;

        Ingredient data = ingObj.GetIngredient();
        float amount   = data.quantity <= 0 ? 1f : data.quantity; // 若 prefab 没填数量视作 1

        Debug.Log($"[Cauldron] 触发器接收 {data.ingredientId} ×{amount}");
        // 物体消失（可替换为隐藏/特效）
        Destroy(other.gameObject);

        craftingMachine.InsertIngredient(data, amount);

        // 爆炸检查
        float total = craftingMachine.GetTotalAmount();
        Debug.Log($"[Cauldron] 当前总量 {total}/{maxCapacity}");
        if (total > maxCapacity)
        {
            Explode(total);
            return;
        }

        // 尝试配方
        craftingMachine.TryProcessRecipes();
    }
    #endregion

    #region CraftingMachineObject 必须实现
    protected override void InitCraftingMachine()
    {
        craftingMachine = new CraftingMachine(this, "cauldron", recipes);
    }

    /// <summary>
    /// 产物实例化
    /// </summary>
    public override void SpawnProduct(Ingredient ing, float amount)
    {
        if (!ing || !ing.prefab)
        {
            Debug.LogWarning($"[Cauldron] 产物 {ing?.ingredientId} prefab 为空!");
            return;
        }
        int count = Mathf.Max(1, Mathf.RoundToInt(amount)); // 简化：按整数次实例化
        for (int i = 0; i < count; i++)
        {
            Instantiate(ing.prefab.gameObject,
                        outputPoint ? outputPoint.position : transform.position + Vector3.up * .5f,
                        Quaternion.identity);
        }
        Debug.Log($"[Cauldron] 生成产物 {ing.ingredientId} ×{count}");
    }

    public override void DestroyIngredient(Ingredient ing)
    {
        // 坩埚内部数据删除已在 CraftingMachine 完成，这里一般不用实现
    }
    #endregion

    #region 爆炸
    private void Explode(float actualAmount)
    {
        Debug.LogError($"[Cauldron] !!! 超量 {actualAmount}，坩埚爆炸 !!!");
        // TODO: 爆炸特效、伤害、删除自身等
        craftingMachine = null; // 推倒重建
    }
    #endregion
    
    #region 从液体接收器接收数据
    /// <summary>供 CauldronLiquidReceiver 调用。</summary>
    public void ReceiveLiquid(Ingredient ing, float amount)
    {
        Debug.Log($"[Cauldron] ReceiveLiquid {ing.ingredientId} +{amount}");
        craftingMachine.InsertIngredient(ing, amount);

        float total = craftingMachine.GetTotalAmount();
        if (total > maxCapacity)
        {
            Explode(total);
            return;
        }
        craftingMachine.TryProcessRecipes();
    }
    #endregion
}