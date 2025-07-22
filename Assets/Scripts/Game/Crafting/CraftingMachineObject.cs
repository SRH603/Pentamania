using UnityEngine;

/// <summary>
/// 所有可“炼制 / 合成”对象的抽象基类。  
/// 负责：  
///     • 保存 <see cref="CraftingMachine"/> 实例；  
///     • 将场景中的 <see cref="IngredientObject"/> 吸收为纯数据；  
///     • 提供 <see cref="SpawnProduct"/> 给逻辑层回调以实例化产物；  
///     • 如需删除原料，可重写 <see cref="DestroyIngredient"/>。  
/// </summary>
public abstract class CraftingMachineObject : MonoBehaviour
{
    /// <summary>逻辑层实例。</summary>
    protected CraftingMachine craftingMachine;

    #region 生命周期
    protected virtual void Awake()
    {
        InitCraftingMachine();
    }
    #endregion

    #region 子类必须实现
    /// <summary>子类负责在此 new <see cref="CraftingMachine"/> 并把自身传入。</summary>
    protected abstract void InitCraftingMachine();

    /// <summary>
    /// 当 <see cref="CraftingMachine"/> 判定有产物时会调用此函数。  
    /// 子类应在此实例化 prefab、做动画或掉落等。
    /// </summary>
    public abstract void SpawnProduct(Ingredient ingredient, float amount);
    #endregion

    #region 可选重写
    /// <summary>
    /// 若需要物理方式销毁 / 特效，可在子类覆写。  
    /// 默认仅做 Debug。
    /// </summary>
    public virtual void DestroyIngredient(Ingredient ingredient)
    {
        Debug.Log($"[{name}] DestroyIngredient 默认实现 —— {ingredient?.ingredientId}");
    }
    #endregion

    #region 工具函数
    /// <summary>
    /// 将场景里的 <see cref="IngredientObject"/> 转化为数据并放入逻辑层。  
    /// 由具体触发器（如坩埚）在检测到碰撞时调用。
    /// </summary>
    protected void InsertIngredientObject(IngredientObject ingObj)
    {
        if (!ingObj) return;

        Ingredient data  = ingObj.GetIngredient();
        float      count = data.quantity <= 0 ? 1f : data.quantity;

        Debug.Log($"[{name}] InsertIngredientObject -> {data.ingredientId} ×{count}");
        craftingMachine.InsertIngredient(data, count);
    }

    /// <summary>便于其他脚本获取逻辑对象。</summary>
    public CraftingMachine GetCraftingMachine() => craftingMachine;
    #endregion
}