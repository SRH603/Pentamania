using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cauldron
{
    private readonly ItemStorage itemStorage;
    private readonly FluidStorage fluidStorage;
    private readonly string machineId;
    private readonly List<CauldronRecipe> recipes;

    public event Action OnInventoryChanged;
    public event Action<float> OnExplode;
    
    public Cauldron(string machineId, IEnumerable<CauldronRecipe> recipes,
                    int itemSlots = 16, int fluidTanks = 1, float tankCap = 9999f)
    {
        this.machineId = machineId;
        this.recipes   = new List<CauldronRecipe>(recipes);
        itemStorage    = new ItemStorage(itemSlots);
        fluidStorage   = new FluidStorage(fluidTanks, tankCap);
        Debug.Log($"[CauldronData] Created id={machineId}, recipes={this.recipes.Count}");
    }

    #region API
    public float GetTotalAmount(Func<ItemStack, float> itemMeasure = null,
                                Func<FluidStack, float> fluidMeasure = null)
    {
        itemMeasure  ??= (it => it.amount);
        fluidMeasure ??= (fl => fl.volume);

        float sum = 0f;
        foreach (var it in itemStorage.View())  if (!it.IsEmpty) sum += itemMeasure(it);
        foreach (var fl in fluidStorage.View()) if (!fl.IsEmpty) sum += fluidMeasure(fl);
        return sum;
    }

    public bool TryGetOnlyFluid(out FluidDef def, out float vol)
    {
        def = null; vol = 0f;
        FluidDef found = null;
        foreach (var tank in fluidStorage.View())
        {
            if (tank.IsEmpty) continue;
            if (found == null) { found = tank.Def; vol = tank.volume; }
            else if (found != tank.Def) return false;
            else vol += tank.volume;
        }
        if (found == null) return false;
        def = found;
        return true;
    }
    #endregion

    #region I/O
    public void InsertSolid(ItemStack stack)
    {
        if (stack.IsEmpty) return;
        int remain = itemStorage.Insert(stack);
        if (remain > 0) Debug.LogWarning($"[CauldronData] Item overflow: {stack.Def.name} x{remain}");
        OnInventoryChanged?.Invoke();
    }

    public void InsertLiquid(FluidStack stack)
    {
        if (stack.IsEmpty) return;
        float remain = fluidStorage.Fill(stack);
        if (remain > 0) Debug.LogWarning($"[CauldronData] Fluid overflow: {stack.Def.name} ~{remain}");
        OnInventoryChanged?.Invoke();
    }

    public float TakeFluid(FluidDef def, float request)
    {
        if (request <= 0f) return 0f;
        FluidStack drained = fluidStorage.Drain(f => f.Def == def, Mathf.CeilToInt(request));
        float got = Mathf.Min(request, drained.volume);
        if (got < drained.volume)
        {
            InsertLiquid(new FluidStack(def, drained.volume - got));
        }
        OnInventoryChanged?.Invoke();
        return got;
    }
    #endregion

    #region Recipe
    public bool TryProcessOnce(out List<ItemStack> solidsToSpawn, out List<FluidStack> liquidsToStay)
    {
        solidsToSpawn = null;
        liquidsToStay = null;

        foreach (var r in recipes)
        {
            if (r.machineId != machineId) continue;
            if (MatchRecipe(r, out var consumeMap, out float ratio))
            {
                foreach (var kv in consumeMap)
                {
                    Consume(kv.Key, kv.Value);
                }
                
                solidsToSpawn = new List<ItemStack>();
                liquidsToStay = new List<FluidStack>();

                foreach (var p in r.products)
                {
                    float amt = p.amount;
                    if (r.reactants != null && ratio < 1f && HasProportional(r))
                        amt *= ratio;

                    if (p.ingredient is ItemDef itemDef)
                        solidsToSpawn.Add(new ItemStack(itemDef, Mathf.RoundToInt(amt)));
                    else if (p.ingredient is FluidDef fluidDef)
                        liquidsToStay.Add(new FluidStack(fluidDef, amt));
                }
                
                foreach (var fl in liquidsToStay) InsertLiquid(fl);

                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    private bool MatchRecipe(CauldronRecipe r,
                             out Dictionary<IngredientDef, float> consume,
                             out float proportionRatio)
    {
        consume = new Dictionary<IngredientDef, float>();
        proportionRatio = 1f;
        
        Dictionary<IngredientDef, float> amounts = GetAmountsDict();
        
        float limiting = float.MaxValue;
        bool anyProportional = false;

        foreach (var req in r.reactants)
        {
            amounts.TryGetValue(req.ingredient, out float have);
            if (have < req.minAmount) return false;
            if (have > req.maxAmount) {
            }
            consume[req.ingredient] = req.minAmount;

            if (req.proportional)
            {
                anyProportional = true;
                float localRatio = have / req.minAmount;
                if (localRatio < limiting) limiting = localRatio;
            }
        }

        if (anyProportional)
        {
            proportionRatio = Mathf.Clamp(limiting, 0f, float.MaxValue);
        }
        
        float deviation = CalcDeviation(r.reactants, amounts);
        if (deviation > 0 && r.explosionCurve.deviationToPower != null)
        {
            float power = r.explosionCurve.deviationToPower.Evaluate(deviation);
            if (power > 0.0001f) OnExplode?.Invoke(power);
        }

        return true;
    }

    private Dictionary<IngredientDef,float> GetAmountsDict()
    {
        Dictionary<IngredientDef, float> map = new();
        foreach (var it in itemStorage.View())
        {
            if (it.IsEmpty) continue;
            if (!map.TryAdd(it.Def, it.amount)) map[it.Def] += it.amount;
        }
        foreach (var fl in fluidStorage.View())
        {
            if (fl.IsEmpty) continue;
            if (!map.TryAdd(fl.Def, fl.volume)) map[fl.Def] += fl.volume;
        }
        return map;
    }

    private bool HasProportional(CauldronRecipe r)
    {
        foreach (var req in r.reactants) if (req.proportional) return true;
        return false;
    }

    private float CalcDeviation(RequirementRange[] reqs, Dictionary<IngredientDef,float> have)
    {
        float dev = 0f;
        foreach (var req in reqs)
        {
            have.TryGetValue(req.ingredient, out float amt);
            if (amt > req.maxAmount)
                dev += amt - req.maxAmount;
        }
        return dev;
    }

    private void Consume(IngredientDef def, float amt)
    {
        if (def is ItemDef itemDef)
        {
            itemStorage.Extract(st => st.Def == itemDef, Mathf.RoundToInt(amt));
        }
        else if (def is FluidDef fluidDef)
        {
            TakeFluid(fluidDef, amt);
        }
    }
    #endregion
}