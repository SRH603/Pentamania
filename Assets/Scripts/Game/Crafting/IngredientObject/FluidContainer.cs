using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FluidContainer : SolidObject, IFluidContainer
{
    [SerializeField] private FluidStack fluidIngredient;
    [SerializeField] private float capacity = 10f;
    [SerializeField] private float pourRateLps = 1f;
    [SerializeField] private float angleThreshold = 45f;
    [SerializeField] private ParticleSystem pourEffect;
    [SerializeField] private GameObject fluidDisplay;

    private bool isPouring;
    private CauldronLiquidReceiver receiverCache;

    public float Capacity => capacity;

    public FluidStack CurrentFluid => fluidIngredient;

    public FluidContainer(ItemStack ingredient,FluidStack fluidIngredient)
    {
        base.ingredient = ingredient;
        this.fluidIngredient = fluidIngredient;
    }

    private new void Start()
    {
        base.Start();
        Init(fluidIngredient);
    }
    public void Init(FluidStack ingredient)
    {
        base.Init(base.ingredient);
        if (ingredient.Def == null) return;
        this.fluidIngredient = ingredient;
        fluidDisplay.GetComponent<MeshRenderer>().material = ingredient.Def.GetMaterial();
    }

    public override IngredientStack GetIngredient()
    {
        return base.ingredient;
    }
    
    public IngredientStack GetFluidIngredient()
    {
        return fluidIngredient;
    }

    public void SetFluidIngredient(IngredientStack ingredient)
    {
        if (ingredient is FluidStack fluidIngredient)
        {
            if (fluidIngredient.Def == null) return;
            // handle logic for the change in ingredient
            this.fluidIngredient = fluidIngredient;
            // TODO:
            // change liquid color to fluidIngredient.Def.GetMaterial();
            fluidDisplay.GetComponent<MeshRenderer>().material = fluidIngredient.Def.GetMaterial();

        }
        else
        {
            // throw some error
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        receiverCache = other.GetComponent<CauldronLiquidReceiver>();
        /*
        if (receiverCache)
            Debug.Log("111");
            */
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CauldronLiquidReceiver>())
        {
            StopPour();
            receiverCache = null;
            //Debug.Log("222");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //throw new System.Exception("Hey, implement the rest of Fluid Container!");
        if (!receiverCache) return;
        if (fluidIngredient.volume <= 0f) return;

        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle < angleThreshold)
        {
            StopPour();
            return;
        }

        float delta = Mathf.Min(pourRateLps * Time.deltaTime, fluidIngredient.volume);
        fluidIngredient.volume -= delta;
        // TODO: Convert the crafting machine to use better stacks
        fluidIngredient.volume += receiverCache.ReceiveLiquid(fluidIngredient.Def, delta);

        if (!isPouring) StartPour();

        if (Mathf.Approximately(fluidIngredient.volume, 0f))
        {
            //Debug.Log($"[{name}] is empty");
            StopPour();
        }
    }
    
    private void StartPour()
    {
        isPouring = true;
        if (pourEffect) pourEffect.Play();
        //Debug.Log("111");
    }

    private void StopPour()
    {
        if (!isPouring) return;
        isPouring = false;
        if (pourEffect) pourEffect.Stop();
        //Debug.Log("111");
    }

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
        {
            return 0;
        }
            
        if (!CanAccept(stack.Def)) 
            return stack.volume;

        float room = capacity - fluidIngredient.volume;
        float toFill = Mathf.Min(room, stack.volume);

        if (fluidIngredient.IsEmpty)
            SetFluidIngredient(new FluidStack(stack.Def, toFill));
        else
            fluidIngredient.volume += toFill;

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
        if (fluidIngredient.IsEmpty || amount <= 0) 
            return new FluidStack(null, 0);

        float toDrain = Mathf.Min(fluidIngredient.volume, amount);
        FluidDef def = fluidIngredient.Def;
        fluidIngredient.volume -= toDrain;

        if (fluidIngredient.volume == 0) 
            fluidIngredient = new FluidStack(null, 0);

        return new FluidStack(def, toDrain);
    }
    
    public bool CanAccept(FluidDef def)
    {
        return fluidIngredient.IsEmpty || fluidIngredient.Def.Equals(def);
    }
}
