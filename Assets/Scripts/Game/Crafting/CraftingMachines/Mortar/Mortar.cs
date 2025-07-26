using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mortar
{
    private readonly List<MortarRecipe> recipeList;

    private readonly List<ItemStack> solidStackList = new List<ItemStack>();
    private readonly FluidStorage fluidStorage;
    private float grindProgress;
    private float progressDecayFactor = 0.7f;

    public event Action OnInventoryChanged;
    public event Action<float> OnProgressChanged;

    public Mortar(IEnumerable<MortarRecipe> recipeSource, int fluidTankCount = 1, float tankCapacity = 9999f)
    {
        recipeList = new List<MortarRecipe>(recipeSource);
        fluidStorage = new FluidStorage(fluidTankCount, tankCapacity);
        grindProgress = 0f;
    }

    public float Progress
    {
        get
        {
            return grindProgress;
        }
    }
    public IReadOnlyList<ItemStack> SolidsView
    {
        get
        {
            return solidStackList;
        }
    }
    public IReadOnlyList<FluidStack> FluidsView
    {
        get
        {
            return fluidStorage.View();
        }
    }

    public void RaiseInventoryChanged()
    {
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
    }
    
    public void InsertSolid(ItemStack incomingStack)
    {
        if (incomingStack.IsEmpty)
        {
            return;
        }

        bool alreadyMerged = false;
        for (int i = 0; i < solidStackList.Count; i++)
        {
            ItemStack storedStack = solidStackList[i];
            if (!storedStack.IsEmpty && storedStack.Def == incomingStack.Def)
            {
                storedStack.amount += incomingStack.amount;
                solidStackList[i] = storedStack;
                alreadyMerged = true;
                break;
            }
        }
        if (!alreadyMerged)
        {
            solidStackList.Add(incomingStack);
        }

        DecayProgress();
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
    }

    public void InsertLiquid(FluidStack incomingFluid)
    {
        if (incomingFluid.IsEmpty)
        {
            return;
        }

        float remain = fluidStorage.Fill(incomingFluid);
        DecayProgress();
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
    }

    private void DecayProgress()
    {
        grindProgress = grindProgress * progressDecayFactor;
        if (grindProgress < 0f)
        {
            grindProgress = 0f;
        }
        if (OnProgressChanged != null)
        {
            OnProgressChanged(grindProgress);
        }
    }
    
    public void PestleHit(float baseIncrement)
    {
        float totalMass = CalculateTotalMass();
        float increment = baseIncrement / Mathf.Max(1f, totalMass);
        grindProgress = Mathf.Clamp01(grindProgress + increment);

        if (OnProgressChanged != null)
        {
            OnProgressChanged(grindProgress);
        }
    }

    private float CalculateTotalMass()
    {
        float sum = 0f;
        foreach (ItemStack eachStack in solidStackList)
        {
            if (!eachStack.IsEmpty)
            {
                sum += eachStack.amount;
            }
        }
        foreach (FluidStack eachFluid in fluidStorage.View())
        {
            if (!eachFluid.IsEmpty)
            {
                sum += eachFluid.volume;
            }
        }
        return sum;
    }
    
    public bool TryCraft(out MortarRecipe matchedRecipe,
                         out Dictionary<IngredientDef, float> consumeMap,
                         out List<ItemStack> productList)
    {
        matchedRecipe = null;
        consumeMap = null;
        productList = null;

        if (grindProgress < 0.999f)
        {
            return false;
        }

        Dictionary<IngredientDef, float> amountDict = BuildAmountDictionary();

        foreach (MortarRecipe recipeCandidate in recipeList)
        {

            Dictionary<IngredientDef, float> consumeTemp;
            bool recipeOk = MatchRecipeStrict(recipeCandidate, amountDict, out consumeTemp);
            if (recipeOk)
            {
                matchedRecipe = recipeCandidate;
                consumeMap = consumeTemp;
                productList = new List<ItemStack>();

                foreach (SolidProductSpec spec in recipeCandidate.products)
                {
                    ItemStack produced = new ItemStack(spec.ingredient, spec.amount);
                    productList.Add(produced);
                }
                return true;
            }
        }
        return false;
    }

    private bool MatchRecipeStrict(MortarRecipe recipeCandidate,
                                   Dictionary<IngredientDef, float> currentAmounts,
                                   out Dictionary<IngredientDef, float> consumeDict)
    {
        consumeDict = new Dictionary<IngredientDef, float>();

        foreach (Requirement requirement in recipeCandidate.reactants)
        {
            float ownedAmount = 0f;
            currentAmounts.TryGetValue(requirement.ingredient, out ownedAmount);
            if (ownedAmount < requirement.minAmount)
                return false;
            if (ownedAmount > requirement.maxAmount)
                return false;
            consumeDict[requirement.ingredient] = requirement.minAmount;
        }

        foreach (KeyValuePair<IngredientDef, float> pair in currentAmounts)
        {
            bool declared = false;
            foreach (Requirement requirement in recipeCandidate.reactants)
            {
                if (requirement.ingredient == pair.Key)
                {
                    declared = true;
                    break;
                }
            }
            if (!declared && pair.Value > 0f)
                return false;
        }
        return true;
    }

    private Dictionary<IngredientDef, float> BuildAmountDictionary()
    {
        Dictionary<IngredientDef, float> map = new Dictionary<IngredientDef, float>();

        foreach (ItemStack stackItem in solidStackList)
        {
            if (stackItem.IsEmpty)
                continue;
            if (!map.ContainsKey(stackItem.Def))
                map[stackItem.Def] = stackItem.amount;
            else
                map[stackItem.Def] += stackItem.amount;
        }

        foreach (FluidStack fluidItem in fluidStorage.View())
        {
            if (fluidItem.IsEmpty)
            {
                continue;
            }
            if (!map.ContainsKey(fluidItem.Def))
                map[fluidItem.Def] = fluidItem.volume;
            else
                map[fluidItem.Def] += fluidItem.volume;
        }

        return map;
    }
    
    public void DoCraft(Dictionary<IngredientDef, float> consumeDict)
    {
        foreach (KeyValuePair<IngredientDef, float> pair in consumeDict)
        {
            if (pair.Key is ItemDef)
            {
                ItemDef targetDef = (ItemDef)pair.Key;
                float remain = pair.Value;

                for (int i = 0; i < solidStackList.Count && remain > 0f; i++)
                {
                    ItemStack stackItem = solidStackList[i];
                    if (stackItem.IsEmpty || stackItem.Def != targetDef)
                    {
                        continue;
                    }

                    int taken = Mathf.Min(Mathf.RoundToInt(remain), stackItem.amount);
                    stackItem.amount -= taken;
                    remain -= taken;
                    
                    
                    if (stackItem.amount <= 0)
                        stackItem = new ItemStack(null, 0);
                    solidStackList[i] = stackItem;
                }
            }
            else if (pair.Key is FluidDef)
            {
                FluidDef fluidDefTarget = (FluidDef)pair.Key;
                TakeFluid(fluidDefTarget, pair.Value);
            }
        }

        solidStackList.RemoveAll(stackElement => stackElement.IsEmpty);
        grindProgress = 0f;

        if (OnProgressChanged != null)
            OnProgressChanged(grindProgress);
        
        
        if (OnInventoryChanged != null)
            OnInventoryChanged();
    }
    
    public bool TryGetOnlyFluid(out FluidDef def, out float volume)
    {
        def = null;
        volume = 0f;

        FluidDef foundDef = null;
        float foundVol = 0f;

        foreach (FluidStack tank in fluidStorage.View())
        {
            if (tank.IsEmpty)
            {
                continue;
            }

            if (foundDef == null)
            {
                foundDef = tank.Def;
                foundVol = tank.volume;
            }
            else if (foundDef != tank.Def)
            {
                return false;
            }
            else
            {
                foundVol += tank.volume;
            }
        }

        if (foundDef == null)
            return false;

        def = foundDef;
        volume = foundVol;
        return true;
    }

    public float TakeFluid(FluidDef def, float requestAmount)
    {
        FluidStack drained = fluidStorage.Drain(x => x.Def == def, Mathf.CeilToInt(requestAmount));
        float realTake = Mathf.Min(requestAmount, drained.volume);

        if (realTake < drained.volume)
        {
            FluidStack back = new FluidStack(def, drained.volume - realTake);
            fluidStorage.Fill(back);
        }

        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
        return realTake;
    }
}