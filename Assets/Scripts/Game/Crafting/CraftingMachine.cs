using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 纯逻辑层：管理当前机器中的所有 <see cref="Ingredient"/> 数量，
/// 并根据绑定的 <see cref="Recipe"/> 判定、执行反应。
/// 不负责场景物体的销毁 / 生成，全部交由 <see cref="CraftingMachineObject"/> 派生类实现。
/// </summary>
public class CraftingMachine
{
    #region 内部数据结构
    private class Stack
    {
        public Ingredient Def;   // ScriptableObject 定义
        public float Amount;     // 数量（可为体积/克/件，由配方统一约定）

        public Stack(Ingredient def, float amount)
        {
            Def = def;
            Amount = amount;
        }
    }
    #endregion

    private readonly CraftingMachineObject _host;
    private readonly string _machineId;
    private readonly List<Recipe> _recipes;           // 绑定在 Cauldron 上
    private readonly List<Stack> _inventory = new();  // 现存原料

    public IReadOnlyList<Recipe> Recipes => _recipes;
    public IReadOnlyList<Ingredient> CurrentIngredients
    {
        get
        {
            List<Ingredient> list = new();
            foreach (var s in _inventory) list.Add(s.Def);
            return list;
        }
    }

    public CraftingMachine(CraftingMachineObject host, string machineId, IEnumerable<Recipe> recipes)
    {
        _host      = host;
        _machineId = machineId;
        _recipes   = new List<Recipe>(recipes);
        Debug.Log($"[{_machineId}] CraftingMachine 创建完毕，已载入配方 {_recipes.Count} 条");
    }

    #region 原料操作
    /// <summary>把原料放入机器（同 id 则堆叠）。</summary>
    public void InsertIngredient(Ingredient ing, float amount)
    {
        Debug.Log($"[{_machineId}] 请求插入 {ing.ingredientId} x{amount}");
        foreach (var s in _inventory)
        {
            if (s.Def == ing)      // 同一个 ScriptableObject => 同一原料
            {
                s.Amount += amount;
                Debug.Log($"[{_machineId}] 堆叠 => {ing.ingredientId} 现有 {s.Amount}");
                return;
            }
        }
        _inventory.Add(new Stack(ing, amount));
        Debug.Log($"[{_machineId}] 新增堆栈 => {ing.ingredientId} 现有 {amount}");
    }

    /// <summary>返回当前总量（用于爆炸检测）。</summary>
    public float GetTotalAmount()
    {
        float sum = 0f;
        foreach (var s in _inventory) sum += s.Amount;
        return sum;
    }
    #endregion

    #region 配方判定
    /// <summary>当库存发生变化时调用，尝试匹配并执行配方。</summary>
    public bool TryProcessRecipes()
    {
        Debug.Log(_recipes);
        foreach (var r in _recipes)
        {
            if (r.machineId != _machineId) continue;      // 非本机配方跳过
            Debug.Log("111");
            if (MatchRecipe(r, out Dictionary<Ingredient, float> consumeMap))
            {
                Debug.Log($"[{_machineId}] 配方 {r.name} 满足，开始产出");
                // 1) 消耗原料
                foreach (var kv in consumeMap)
                {
                    Consume(kv.Key, kv.Value);
                }
                // 2) 生成产物
                foreach (var p in r.products)
                {
                    _host.SpawnProduct(p.ingredient, p.amount);
                }
                return true;
            }
        }
        Debug.Log($"[{_machineId}] 当前未满足任何配方");
        return false;
    }

    /// <summary>检查库存是否满足配方，如满足返回需要扣除的数量。</summary>
    private bool MatchRecipe(Recipe r, out Dictionary<Ingredient, float> consume)
    {
        consume = new Dictionary<Ingredient, float>();
        foreach (var req in r.reactants)
        {
            Stack stack = _inventory.Find(s => s.Def == req.ingredient);
            if (stack == null)
            {
                Debug.Log($"[{_machineId}] 缺少原料 {req.ingredient.ingredientId}");
                return false;
            }

            // 判断数量区间
            if (stack.Amount < req.minAmount || stack.Amount > req.maxAmount)
            {
                Debug.Log($"[{_machineId}] 原料 {req.ingredient.ingredientId} 数量 {stack.Amount} 不在 [{req.minAmount},{req.maxAmount}]");
                return false;
            }

            consume[req.ingredient] = req.minAmount; // 基础消耗：先按 min 扣，后续可扩展比例/偏差逻辑
        }
        return true;
    }

    /// <summary>真正扣库存（若归零则移除堆栈）。</summary>
    private void Consume(Ingredient ing, float amount)
    {
        Stack stack = _inventory.Find(s => s.Def == ing);
        if (stack == null) return;
        stack.Amount -= amount;
        Debug.Log($"[{_machineId}] 消耗 {ing.ingredientId} {amount}，剩余 {stack.Amount}");
        if (stack.Amount <= 0.0001f)
        {
            _inventory.Remove(stack);
            Debug.Log($"[{_machineId}] {ing.ingredientId} 堆栈清空并移除");
        }
    }
    #endregion
}