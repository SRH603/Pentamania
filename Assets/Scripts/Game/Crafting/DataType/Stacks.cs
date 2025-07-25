using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public interface IngredientStack {
    public IngredientDef GetAbstractDef();
}

[System.Serializable]
public struct ItemStack : IStackable<ItemStack>, IngredientStack
{
    [SerializeField]
    private ItemDef def;

    public ItemDef Def
    {
        get => def;
        private set => def = value;
    }

    public int amount;
    public List<IngredientTag> tags;

    public ItemStack(ItemDef def, int amount, List<IngredientTag> tags)
    {
        this.def = def;
        this.amount = amount;
        this.tags = tags;
    }
    
    public ItemStack(ItemDef def, int amount)
    {
        this.def = def;
        this.amount = amount;
        this.tags = new List<IngredientTag>();
    }

    public bool IsEmpty => Def == null || amount <= 0;

    public bool CanMerge(ItemStack other) =>
        Def == other.Def && !IsEmpty && !other.IsEmpty;

    public void Merge(ref ItemStack other)
    {
        if (!CanMerge(other)) return;
        int room = Def.GetMaxStack() - amount;
        int toMove = Mathf.Min(room, other.amount);
        amount += toMove;
        other.amount -= toMove;
    }

    public IngredientDef GetAbstractDef()
    {
        return Def;
    }
}

[System.Serializable]
public struct FluidStack : IngredientStack, IStackable<FluidStack>
{
    [SerializeField]
    private FluidDef def;

    public FluidDef Def
    {
        get => def;
        private set => def = value;
    }

    public float volume;
    public List<IngredientTag> tags;
    
    public FluidStack(FluidDef def, float volume, List<IngredientTag> tags)
    {
        this.def = def;
        this.volume = volume;
        this.tags = tags;
    }

    public FluidStack(FluidDef def, float volume)
    {
        this.def = def;
        this.volume = volume;
        this.tags = new List<IngredientTag>();
    }

    public bool IsEmpty => Def == null || volume <= 0;

    public IngredientDef GetAbstractDef()
    {
        return Def;
    }

    public bool CanMerge(FluidStack other)
    {
        return other.Def.GetId().Equals(this.Def.GetId());
    }

    public void Merge(ref FluidStack other)
    {
        if (CanMerge(other))
        {
            volume += other.volume;
        }
        else
        {
            throw new ArgumentException("The other fluid stack must pass the CanMerge function to Merge with this fluid stack.");
        }
    }
}