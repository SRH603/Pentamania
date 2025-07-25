using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class ItemStorage : IItemStorage
{
    [SerializeField] private List<ItemStack> slots;
    public int SlotCount => slots.Count;

    public ItemStorage(int slotCount)
    {
        slots = new List<ItemStack>(slotCount);
        for (int i = 0; i < slotCount; i++) slots.Add(new ItemStack(null, 0));
    }

    public IReadOnlyList<ItemStack> View() => slots;

    public int Insert(ItemStack stack)
    {
        if (stack.IsEmpty) return 0;
        
        for (int i = 0; i < slots.Count && stack.amount > 0; i++)
        {
            if (!slots[i].IsEmpty && slots[i].Def == stack.Def)
            {
                slots[i].Merge(ref stack);
                slots[i] = slots[i];
            }
        }
        for (int i = 0; i < slots.Count && stack.amount > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int toMove = Mathf.Min(stack.amount, stack.Def.GetMaxStack());
                slots[i] = new ItemStack(stack.Def, toMove);
                stack.amount -= toMove;
            }
        }
        return stack.amount;
    }

    public ItemStack Extract(Predicate<ItemStack> filter, int amount)
    {
        ItemStack result = new ItemStack(null, 0);
        for (int i = 0; i < slots.Count && amount > 0; i++)
        {
            if (slots[i].IsEmpty) continue;
            if (!filter(slots[i])) continue;

            int toTake = Mathf.Min(amount, slots[i].amount);
            if (result.IsEmpty)
                result = new ItemStack(slots[i].Def, toTake);
            else
                result.amount += toTake;

            ItemStack temp = new ItemStack();
            temp = slots[i];
            temp.amount -= toTake;
            slots[i] = temp;
            //slots[i].amount -= toTake;
            amount -= toTake;

            if (slots[i].amount <= 0)
                slots[i] = new ItemStack(null, 0);
        }
        return result;
    }
}

[System.Serializable]
public class FluidStorage : IFluidStorage
{
    [SerializeField] private List<FluidStack> tanks;
    [SerializeField] private List<float> capacities;

    public FluidStorage(int tankCount, float eachCapacity)
    {
        tanks = new List<FluidStack>(tankCount);
        capacities = new List<float>(tankCount);
        for (int i = 0; i < tankCount; i++)
        {
            tanks.Add(new FluidStack(null, 0));
            capacities.Add(eachCapacity);
        }
    }

    public IReadOnlyList<FluidStack> View() => tanks;

    public float Fill(FluidStack stack)
    {
        if (stack.IsEmpty) return 0;
        float remain = stack.volume;
        
        for (int i = 0; i < tanks.Count && remain > 0; i++)
        {
            if (!tanks[i].IsEmpty && tanks[i].Def == stack.Def)
            {
                float room = capacities[i] - tanks[i].volume; 
                float toFill = Mathf.Min(room, remain);
                
                FluidStack temp = new FluidStack();
                temp = tanks[i];
                temp.volume += toFill;
                tanks[i] = temp;
                //tanks[i].volume += toFill;
                remain -= toFill;
            }
        }
        for (int i = 0; i < tanks.Count && remain > 0; i++)
        {
            if (tanks[i].IsEmpty)
            {
                float toFill = Mathf.Min(capacities[i], remain);
                tanks[i] = new FluidStack(stack.Def, toFill);
                remain -= toFill;
            }
        }
        return remain;
    }

    public FluidStack Drain(Predicate<FluidStack> filter, float amount)
    {
        FluidStack result = new FluidStack(null, 0);
        for (int i = 0; i < tanks.Count && amount > 0; i++)
        {
            if (tanks[i].IsEmpty) continue;
            if (!filter(tanks[i])) continue;

            float toTake = Mathf.Min(amount, tanks[i].volume);

            if (result.IsEmpty)
                result = new FluidStack(tanks[i].Def, toTake);
            else
                result.volume += toTake;

            FluidStack temp = new FluidStack();
            temp = tanks[i];
            temp.volume -= toTake;
            tanks[i] = temp;
            //anks[i].volume -= toTake;
            amount -= toTake;

            if (tanks[i].volume <= 0)
                tanks[i] = new FluidStack(null, 0);
        }
        return result;
    }
}
