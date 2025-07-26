using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CauldronObject : MonoBehaviour
{
    [SerializeField] private float maxCapacity = 100f;
    [SerializeField] private int itemSlots = 16;
    [SerializeField] private int fluidTanks = 1;
    [SerializeField] private float tankCapacity = 9999f;
    [SerializeField] private Transform outputPoint;
    [SerializeField] private CauldronRecipe[] recipes;
    [SerializeField] private GameObject solidObjectPrefab;
    [SerializeField] private GameObject liquidPlane;

    [SerializeField] private float progressMax = 100f;
    [SerializeField] private float progressPerTurn = 20f;
    private float progress = 0f;
    [SerializeField] private HandleController handle;
    
    private float prevAngle = 0f;

    private Cauldron data;
    
    public Action<float> OnExplodeVisual;
    public Action<ItemStack> OnSpawnSolid;
    
    //DEBUG
    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private FluidStorage fluidStorage;

    private void Awake()
    {
        data = new Cauldron(recipes, itemSlots, fluidTanks, tankCapacity);
        data.OnExplode += HandleExplosion;
    }
    
    void Update()
    {
        if (!handle) return;

        float angle = handle.Angle;

        if (angle - prevAngle >= 0.0001f) GainProgress((angle - prevAngle) / 360 * progressPerTurn);
        
        /*
        if (prevAngle < -170f && angle > 170f)
            OnHandleFullTurn();
            */

        prevAngle = angle;
        
        //DEBUG
        itemStorage = data.GetItemStorage();
        fluidStorage = data.GetFluidStorage();

    }

    #region Spoon
    private void OnHandleFullTurn() => GainProgress(progressPerTurn);

    private void GainProgress(float amt)
    {
        progress = Mathf.Min(progress + amt, progressMax);
        //Debug.Log("[Cauldron] Stirring progress increased");
        if (progress >= progressMax)
        {
            Debug.Log("[Cauldron] Stirring progress reached max");
            TryProcessCauldron();
        }
            
    }
    #endregion

    #region API
    public void ReceiveSolid(SolidObject sObj)
    {
        if (!sObj) return;
        //var ing = sObj.GetIngredient() as ItemStack ?? default;
        //ItemStack ing = sObj.GetIngredient() as ItemStack;
        float before = data.GetTotalAmount();
        
        ItemStack ing = (ItemStack)sObj.GetIngredient();

        var stack = ing;

        if (stack.IsEmpty) return;
        
        Debug.Log($"[Cauldron] Inserting a solid object with: {stack.Def.GetId()}");
        
        data.InsertSolid(stack);
        Destroy(sObj.gameObject);
        
        float after = data.GetTotalAmount();
        if (after > 0f) progress *= before / after;
        
        AfterInsert();
    }

    public float ReceiveLiquid(FluidStack fluid)
    {
        if (fluid.IsEmpty) 
            return 0;
        
        Debug.Log($"[Cauldron] Inserting fluid of: {fluid.Def.GetId()}");
        
        float before = data.GetTotalAmount();
        float remain = data.InsertLiquid(fluid);
        float after = data.GetTotalAmount();
        if (after > 0f)
            progress *= before / after;

        AfterInsert();
        return remain;
    }

    
    public void TryScoopLiquid(IFluidContainer container)
    {
        //if (!container) return;

        if (!data.TryGetOnlyFluid(out var cauldronFluid))
        {
            Debug.Log("[CauldronObj] Cannot scoop: none or multiple liquids");
            return;
        }
        if (!container.CanAccept(cauldronFluid.Def))
        {
            Debug.Log("[Cauldron] Container can't accept this fluid");
            return;
        }

        float capacityLeft = container.Capacity - container.CurrentFluid.volume;
        if (capacityLeft <= 0f)
        {
            Debug.Log("[Cauldron] Container is full");
            return;
        }
        
        //float take = Mathf.Min(capacityLeft, cauldronVol);
        FluidStack take = data.TakeFluid(cauldronFluid);
        take.volume = container.Fill(take);
        data.InsertLiquid(take);

        Debug.Log($"[Cauldron] Scooped {take.volume}, container now has {container.CurrentFluid.volume}");
    }
    #endregion

    #region PRIVATE
    private void AfterInsert()
    {
        float total = data.GetTotalAmount();
        if (total > maxCapacity)
        {
            HandleExplosion(total - maxCapacity);
            return;
        }

        //data.ShowIngredients();
        
        /*
        bool any = false;
        while (data.TryProcessOnce(out var solids, out var liquids))
        {
            any = true;
            if (solids != null)
                foreach (var s in solids) SpawnSolid(s);
            if (liquids != null)
            {
                liquidPlane.GetComponent<MeshRenderer>().material = liquids[0].Def.GetMaterial();
            }
        }
        if (any) Debug.Log("[CauldronObj] Recipes processed.");
        */
    }
    
    private void TryProcessCauldron()
    {
        bool anyRecipe = false;

        while (data.TryProcessOnce(out var solids, out var liquids))
        {
            anyRecipe = true;

            if (solids != null)
                foreach (var s in solids) SpawnSolid(s);

            if (liquids != null && liquids.Count > 0)
                liquidPlane.GetComponent<MeshRenderer>().material = liquids[0].Def.GetMaterial();
        }
        if (anyRecipe) Debug.Log("[Cauldron] Recipes processed");

        // TODO Byproduct Logic
        if (!anyRecipe && !data.IsEmpty() && data.ByproductDef)
        {
            data.InsertLiquid(new FluidStack(data.ByproductDef, data.ByproductVol));
            liquidPlane.GetComponent<MeshRenderer>().material = data.ByproductDef.GetMaterial();
            Debug.Log("[Cauldron] No recipe found, produced by-product");
        }

        progress = 0f;
    }

    private void SpawnSolid(ItemStack stack)
    {
        if (stack.IsEmpty || !stack.Def) return;
        int count = stack.amount;
        for (int i = 0; i < count; i++)
        {
            if (OnSpawnSolid != null)
            {
                OnSpawnSolid.Invoke(new ItemStack(stack.Def, 1));
                continue;
            }

            GameObject go = Instantiate(solidObjectPrefab);
            var so = go.GetComponent<SolidObject>();
            var rb = go.GetComponent<Rigidbody>();
            so.Init(stack);

            go.transform.position = outputPoint ? outputPoint.position : transform.position + Vector3.up * 2f;
            rb.AddForce(new Vector3(UnityEngine.Random.Range(-100,100), 10000, UnityEngine.Random.Range(-100,100)), ForceMode.Impulse);
        }

        Debug.Log($"[Cauldron] Spawned item {stack.Def.GetId()} x{count}");
    }

    private void HandleExplosion(float power)
    {
        Debug.Log($"[Cauldron] EXPLOSION! power={power}");
        OnExplodeVisual?.Invoke(power);
        // TODO VISUAL
    }
    #endregion
}