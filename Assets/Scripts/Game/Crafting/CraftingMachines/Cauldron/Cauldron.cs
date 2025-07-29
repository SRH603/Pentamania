using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Game.Utilities;

[Serializable]
public class Cauldron
{
    private readonly ItemStorage itemStorage;
    private readonly FluidStorage fluidStorage;
    private readonly List<CauldronRecipe> recipes;

    
    private readonly FluidDef byproductDef;
    private readonly float byproductVol;
    
    public  FluidDef ByproductDef  => byproductDef;
    public  float ByproductVol => byproductVol;

    public event Action OnInventoryChanged;
    public event Action<float> OnExplode;

    public Cauldron(IEnumerable<CauldronRecipe> recipes,
                    int itemSlots = 16, int fluidTanks = 1, float tankCap = 9999f,
                    FluidDef byproductDef = null, float byproductVol = 0f)
    {
        this.recipes = new List<CauldronRecipe>(recipes);
        itemStorage = new ItemStorage(itemSlots);
        fluidStorage = new FluidStorage(fluidTanks, tankCap);
        this.byproductDef = byproductDef;
        this.byproductVol = byproductVol;
    }
    
    public bool IsEmpty()
    {
        foreach (var it in itemStorage.View()) if (!it.IsEmpty) return false;
        foreach (var fl in fluidStorage.View()) if (!fl.IsEmpty) return false;
        return true;
    }
    
    public float GetTotalAmount(Func<ItemStack, float> itemMeasure = null, Func<FluidStack, float> fluidMeasure = null)
    {
        if (itemMeasure == null)
            itemMeasure = it => it.amount;

        if (fluidMeasure == null)
            fluidMeasure = fl => fl.volume;
        
        float sum = 0f;
        foreach (var it in itemStorage.View()) 
            if (!it.IsEmpty) 
                sum += itemMeasure(it);
        foreach (var fl in fluidStorage.View()) 
            if (!fl.IsEmpty) 
                sum += fluidMeasure(fl);
        return sum;
    }

    public bool TryGetOnlyFluid(out FluidStack found)
    {
        found = new FluidStack();
        foreach (var tank in fluidStorage.View())
        {
            if (tank.IsEmpty) continue;
            
            if (found.IsEmpty)
            {
                //found = new FluidStack(tank.Def, tank.volume);
                found = tank.Copy();
            }
            else if (!found.CanMerge(tank))
            {
                //Debug.Log("Can not merge");
                return false;
            }
            else
            {
                found.volume += tank.volume;
            }
                
        }

        if (found.IsEmpty)
        {
            Debug.Log("[Cauldron] No Fluid inside");
            return false;
        }
            
        return true;
    }

    public Color CalculateFluidColor()
    {
        Color totalColor = Color.black;
        float totalWeight = 0f;
        
        foreach (var st in itemStorage.View())
        {
            if (st.IsEmpty || st.tags == null) continue;

            foreach (var t in st.tags)
            {
                if (t.ingredientTagDef == null)
                    continue;
                float weight = t.value * st.amount * t.ingredientTagDef.GetColorWeight();
                totalColor += t.ingredientTagDef.GetColor() * weight;
                totalWeight += weight;
            }
        }
        
        foreach (var fl in fluidStorage.View())
        {
            if (fl.IsEmpty || fl.tags == null) continue;

            foreach (var t in fl.tags)
            {
                if (t.ingredientTagDef == null)
                    continue;
                float weight = t.value * fl.volume * t.ingredientTagDef.GetColorWeight();
                totalColor += t.ingredientTagDef.GetColor() * weight;
                totalWeight += weight;
            }
        }
        
        Color finalColor = totalWeight > 0 ? totalColor / totalWeight : Color.grey;
        
        Color.RGBToHSV(finalColor, out float h, out float s, out float v);
        
        float oldV = v * 100f;
        
        float mappedV = oldV / 100f * 60f;
        
        float finalV = mappedV / 100f;

        Color newColor = Color.HSVToRGB(h, s, finalV);

        return newColor;
    }
    
    /*
    public bool TryGetOnlyFluid(out FluidDef def, out float vol)
    {
        def = null;
        vol = 0f;
        FluidDef found = null;
        foreach (var tank in fluidStorage.View())
        {
            if (tank.IsEmpty)
                continue;
            if (found == null)
            {
                found = tank.Def;
                vol = tank.volume;
            }
            else if (found != tank.Def) 
                return false;
            else 
                vol += tank.volume;
        }
        if (found == null)
            return false;
        def = found;
        return true;
    }
    */
    
    public void InsertSolid(ItemStack stack)
    {
        if (stack.IsEmpty)
            return;
        if (stack.Def.GetId() == "cleansing_elixir")
        {
            itemStorage.Clear();
            fluidStorage.Clear();
            return;
        }
        int remain = itemStorage.Insert(stack);
        if (remain > 0)
            Debug.Log("[Cauldron] Exceeds (Solid)");
        OnInventoryChanged?.Invoke();
    }

    public float InsertLiquid(FluidStack stack)
    {
        if (stack.IsEmpty)
            return 0;
        float remain = fluidStorage.Fill(stack);
        if (remain > 0)
            Debug.Log("[Cauldron] Exceeds (Liquid)");
        OnInventoryChanged?.Invoke();
        return remain;
    }

    public FluidStack TakeFluid(FluidStack stack)
    {
        if (stack.IsEmpty)
            return new FluidStack(null, 0);
        
        FluidStack drained = fluidStorage.Drain(f => f.Def == stack.Def, stack.volume);
        float got = Mathf.Min(stack.volume, drained.volume);
        if (got < drained.volume)
        {
            InsertLiquid(new FluidStack(stack.Def, drained.volume - got));
        }
        OnInventoryChanged?.Invoke();

        stack.volume = got;
        return stack;
    }
    
    /*
    public float TakeFluid(FluidDef def, float request)
    {
        if (request <= 0f)
            return 0f;
        FluidStack drained = fluidStorage.Drain(f => f.Def == def, Mathf.CeilToInt(request));
        float got = Mathf.Min(request, drained.volume);
        if (got < drained.volume)
        {
            InsertLiquid(new FluidStack(def, drained.volume - got));
        }
        OnInventoryChanged?.Invoke();
        return got;
    }
    */
    
    public bool TryProcessOnce(out List<ItemStack> solidsToSpawn,
        out List<FluidStack> liquidsToStay)
    {
        Debug.Log("[Cauldron] Matching Recipe");
        solidsToSpawn  = new List<ItemStack>();
        liquidsToStay  = new List<FluidStack>();
        
        var tagMap = BuildTagMap();
        
        CauldronRecipe best = null;
        float bestScore = 0f;

        foreach (var r in recipes)
        {
            if (r.requirement == null || r.requirement.Length == 0) continue;
            float s = GetSimilarity(r, tagMap, extraPenalty: 2.0);
            if (s > bestScore)
            {
                bestScore = s;
                best = r;
            }
        }

        if (best == null || bestScore <= 0.0001)
        {
            Debug.Log("[Cauldron] No recipe matched");
            return false;
        }
        
        Debug.Log($"[Cauldron] Recipe matched: {best} with a similarity of {bestScore}");
        
        float t = GetTotalAmount();
        float c = 0f;
        
        itemStorage.Clear();
        fluidStorage.Clear();

        foreach (var i in best.requirement)
        {
            c += i.weight;
        }

        float m = t / c;
        
        Debug.Log($"[Cauldron] Recipe multiplier is: {m}");
        
        foreach (var p in best.products)
        {
            float amt = p.amount;
            if (!p.itemProduct.IsEmpty)
            {
                var ni = p.itemProduct.CopyWithAmount((int)(p.itemProduct.amount * amt * m));
                solidsToSpawn.Add(ni);
            }

            if (!p.itemProduct.IsEmpty)
            {
                var nf = p.fluidProduct.CopyWithVolume(p.fluidProduct.volume * amt * m);
                liquidsToStay.Add(nf);
                InsertLiquid(nf);
            }
            //Debug.Log(p.fluidProduct);
            /*
            if (p.ingredient is ItemDef itemDef)
                solidsToSpawn.Add(new ItemStack(itemDef, Mathf.RoundToInt(amt)));
            else if (p.ingredient is FluidDef fluidDef)
                liquidsToStay.Add(new FluidStack(fluidDef, amt));
                */
        }
        
        //Debug.Log(liquidsToStay.Count);
        /*
        foreach (var fl in liquidsToStay)
            InsertLiquid(fl);
            */
            

        OnInventoryChanged?.Invoke();
        return true;
    }

    private Dictionary<IngredientDef, float> GetAmountsDict()
    {
        Dictionary<IngredientDef, float> map = new();
        foreach (var it in itemStorage.View())
        {
            if (it.IsEmpty)
                continue;
            if (!map.TryAdd(it.Def, it.amount)) 
                map[it.Def] += it.amount;
        }
        foreach (var fl in fluidStorage.View())
        {
            if (fl.IsEmpty) 
                continue;
            if (!map.TryAdd(fl.Def, fl.volume))
                 map[fl.Def] += fl.volume;
        }
        return map;
    }
    
    private float CalcDeviation(Requirement[] reqs, Dictionary<IngredientDef, float> have)
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

    public void HandleExplosion(float power)
    {
        itemStorage.Clear();
        fluidStorage.Clear();
    }
    
private Dictionary<string, double> BuildTagMap()
{
    var map = new Dictionary<string, double>();
    
    foreach (var st in itemStorage.View())
    {
        if (st.IsEmpty || st.tags == null) continue;
        foreach (var t in st.tags)
        {
            double add = t.value * st.amount;
            if (map.TryGetValue(t.ingredientTagDef.GetId(), out var cur)) map[t.ingredientTagDef.GetId()] = cur + add;
            else map[t.ingredientTagDef.GetId()] = add;
        }
    }
    
    foreach (var fl in fluidStorage.View())
    {
        if (fl.IsEmpty || fl.tags == null) continue;
        foreach (var t in fl.tags)
        {
            double add = t.value * fl.volume;
            if (map.TryGetValue(t.ingredientTagDef.GetId(), out var cur)) map[t.ingredientTagDef.GetId()] = cur + add;
            else map[t.ingredientTagDef.GetId()] = add;
        }
    }
    return map;
}


public float GetSimilarity(
    CauldronRecipe recipe,
    IDictionary<string, double> actual,
    double extraPenalty = 2.0)
{
    var target = new Dictionary<string, (double r, int w)>();
    foreach (var req in recipe.requirement)
    {
        string id = req.ingredientTag.GetId();
        double value = req.weight;
        int weight = Mathf.Max(1, req.weight);

        if (target.TryGetValue(id, out var old))
        {
            target[id] = (old.r + value, old.w + weight);
        }
        else
        {
            target[id] = (value, weight);
        }
    }
    
    double totalTarget = target.Sum(kv => kv.Value.r * kv.Value.w);
    if (totalTarget <= 0)
    {
        return 0f;
    }

    var targetDist = target.ToDictionary(
        kv => kv.Key,
        kv =>
        {
            double normalized = kv.Value.r * kv.Value.w / totalTarget;
            return (r: normalized, w: kv.Value.w);
        }
    );

    double totalActual = actual.Values.Sum();
    if (totalActual <= 0)
    {
        return 0f;
    }
    
    var actualDist = actual.ToDictionary(
        kv => kv.Key,
        kv =>
        {
            double pk = kv.Value / totalActual;
            return pk;
        }
    );
    
    double diff = 0.0;
    foreach (var kv in targetDist)
    {
        string tagId = kv.Key;
        double r = kv.Value.r;
        int w = kv.Value.w;

        if (actualDist.TryGetValue(tagId, out double pk))
        {
            double term = w * Math.Abs(r - pk);
            diff += term;
            actualDist.Remove(tagId);
        }
        else
        {
            double term = w * r;
            diff += term;
        }
    }
    
    double extraMass = 0.0;
    foreach (var kv in actualDist)
    {
        double pk = kv.Value;
        extraMass += pk;
    }
    double extraTerm = extraPenalty * extraMass;
    diff += extraTerm;

    double targetSum = targetDist.Sum(kvp => kvp.Value.w * kvp.Value.r);
    double maxDiff = targetSum + extraPenalty * 1.0;

    double score = Math.Max(0.0, 1.0 - diff / maxDiff) * 100.0;
    return (float)score;
}


    // DEBUG STUFF
    public void ShowIngredients()
    {
        string debugOutput = "";
        debugOutput += "Solid Ingredients: \n";
        if (itemStorage == null)
        {
            Debug.Log("Uh oh");
        }
        foreach (ItemStack itemStack in itemStorage.View())
        {
            if (itemStack.amount == 0)
            {
                continue;
            }
            debugOutput += $"ItemStack {itemStack.Def.GetId()} with quant {itemStack.amount}\n";
        }

        debugOutput += "Fluid Ingredients: \n";
        foreach (FluidStack fluidStack in fluidStorage.View())
        {
            if (fluidStack.volume == 0)
            {
                continue;
            }
            debugOutput += $"ItemStack {fluidStack.Def.GetId()} with quant {fluidStack.volume}\n";
        }

        Debug.Log(debugOutput);
    }

    public ItemStorage GetItemStorage()
    {
        return itemStorage;
    }
    
    public FluidStorage GetFluidStorage()
    {
        return fluidStorage;
    }

}