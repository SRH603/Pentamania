using System;
using UnityEngine;

public class FluidContainer : PassableIngredientObject, IFluidContainer
{
    [SerializeField] private FluidStack ingredient;
    [SerializeField] private float capacity = 10f;
    [SerializeField] private float pourRateLps = 1f;
    [SerializeField] private float angleThreshold = 45f;
    [SerializeField] private ParticleSystem pourEffect;

    private bool isPouring;
    private CauldronLiquidReceiver receiverCache;

    public float Capacity => capacity;

    public FluidStack CurrentFluid => ingredient;

    public FluidContainer(FluidStack ingredient)
    {
        this.ingredient = ingredient;
    }

    public override IngredientStack GetIngredient()
    {
        return ingredient;
    }

    public override void SetIngredient(IngredientStack ingredient)
    {
        if (ingredient is FluidStack fluidIngredient)
        {
            // handle logic for the change in ingredient
            this.ingredient = fluidIngredient;
            // TODO:
            // change liquid color to fluidIngredient.Def.GetMaterial();

        }
        else
        {
            // throw some error
        }
    }

    # region Triggers
    private void OnTriggerEnter(Collider other)
    {
        receiverCache = other.GetComponent<CauldronLiquidReceiver>();
        if (receiverCache)
            Debug.Log($"[{name}] entered CauldronLiquidReceiver");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CauldronLiquidReceiver>())
        {
            StopPour();
            receiverCache = null;
            Debug.Log($"[{name}] left CauldronLiquidReceiver");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //throw new System.Exception("Hey, implement the rest of Fluid Container!");
        if (!receiverCache) return;
        if (ingredient.volume <= 0f) return;

        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle < angleThreshold)
        {
            StopPour();
            return;
        }

        float delta = Mathf.Min(pourRateLps * Time.deltaTime, ingredient.volume);
        ingredient.volume -= delta;
        // TODO: Convert the crafting machine to use better stacks
        receiverCache.ReceiveLiquid(ingredient.Def, delta);

        if (!isPouring) StartPour();

        if (Mathf.Approximately(ingredient.volume, 0f))
        {
            Debug.Log($"[{name}] is empty");
            StopPour();
        }
    }
    #endregion

    #region Effects (visual / sounds)
    private void StartPour()
    {
        isPouring = true;
        if (pourEffect) pourEffect.Play();
        Debug.Log($"[{name}] Started Pouring");
    }

    private void StopPour()
    {
        if (!isPouring) return;
        isPouring = false;
        if (pourEffect) pourEffect.Stop();
        Debug.Log($"[{name}] Stopped Pouring");
    }
    #endregion

    /*
    public float Fill(FluidStack stack)
    {
        
        if (CanAccept(stack.Def))
        {
            ingredient.Merge(ref stack);
            return ingredient.volume;
        }
        else
        {
            throw new ArgumentException("The fluid stack must pass the CanMerge function to Merge with this fluid container.");
        }
    }
    */
    
    public float Fill(FluidStack stack)
    {
        if (stack.IsEmpty) 
            return 0;
        if (!CanAccept(stack.Def)) 
            return stack.volume;

        float room = capacity - ingredient.volume;
        float toFill = Mathf.Min(room, stack.volume);

        if (ingredient.IsEmpty)
            ingredient = new FluidStack(stack.Def, toFill);
        else
            ingredient.volume += toFill;

        return stack.volume - toFill;
    }

    /*
    public FluidStack Drain(int amount)
    {
        ingredient.volume -= amount;
        return ingredient;
    }
    */
    
    public FluidStack Drain(int amount)
    {
        if (ingredient.IsEmpty || amount <= 0) 
            return new FluidStack(null, 0);

        float toDrain = Mathf.Min(ingredient.volume, amount);
        FluidDef def = ingredient.Def;
        ingredient.volume -= toDrain;

        if (ingredient.volume == 0) 
            ingredient = new FluidStack(null, 0);

        return new FluidStack(def, toDrain);
    }
    
    public bool CanAccept(FluidDef def)
    {
        return ingredient.IsEmpty || ingredient.Def.Equals(def);
    }
}
