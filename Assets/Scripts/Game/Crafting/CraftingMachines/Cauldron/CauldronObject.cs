using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CauldronObject : MonoBehaviour
{
    [SerializeField] private float maxCapacity = 100f;
    [SerializeField] private int   itemSlots   = 16;
    [SerializeField] private int   fluidTanks  = 1;
    [SerializeField] private float tankCapacity = 9999f;
    [SerializeField] private Transform outputPoint;
    [SerializeField] private CauldronRecipe[] recipes;
    [SerializeField] private GameObject solidObjectPrefab;

    private Cauldron data;
    
    public Action<float> OnExplodeVisual;
    public Action<ItemStack> OnSpawnSolid;

    private void Awake()
    {
        data = new Cauldron("cauldron", recipes, itemSlots, fluidTanks, tankCapacity);
        data.OnExplode += HandleExplosion;
    }

    #region API
    public void ReceiveSolid(SolidObject sObj)
    {
        if (!sObj) return;
        //var ing = sObj.GetIngredient() as ItemStack? ?? default;
        ItemStack ing = sObj.GetIngredient() as ItemStack;
        var stack = ing;
        if (stack.IsEmpty) return;

        data.InsertSolid(stack);
        Destroy(sObj.gameObject);
        AfterInsert();
    }

    public void ReceiveLiquid(FluidDef def, float vol)
    {
        if (!def || vol <= 0f) return;
        data.InsertLiquid(new FluidStack(def, vol));
        AfterInsert();
    }

    
    public void TryScoopLiquid(IFluidContainer container)
    {
        //if (!container) return;

        if (!data.TryGetOnlyFluid(out var cauldronDef, out float cauldronVol))
        {
            Debug.Log("[CauldronObj] Cannot scoop: none or multiple liquids.");
            return;
        }
        if (!container.CanAccept(cauldronDef))
        {
            Debug.Log("[CauldronObj] Container can't accept this fluid.");
            return;
        }

        float capacityLeft = container.Capacity - container.CurrentFluid.volume;
        if (capacityLeft <= 0f)
        {
            Debug.Log("[CauldronObj] Container full.");
            return;
        }

        float take = Mathf.Min(capacityLeft, cauldronVol);
        float real = data.TakeFluid(cauldronDef, take);
        container.Fill(new FluidStack(cauldronDef, real));

        Debug.Log($"[CauldronObj] Scooped {real}/{take}. Container now {container.CurrentFluid.volume}");
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
        
        bool any = false;
        while (data.TryProcessOnce(out var solids, out var liquids))
        {
            any = true;
            if (solids != null)
                foreach (var s in solids) SpawnSolid(s);
        }
        if (any) Debug.Log("[CauldronObj] Recipes processed.");
    }

    private void SpawnSolid(ItemStack stack)
    {
        if (stack.IsEmpty) return;
        if (!stack.Def) return;
        
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

            go.transform.position = outputPoint ? outputPoint.position : transform.position + Vector3.up * 0.5f;
            rb.AddForce(new Vector3(UnityEngine.Random.Range(-100,100), 10000, UnityEngine.Random.Range(-100,100)), ForceMode.Impulse);
        }

        Debug.Log($"[CauldronObj] Spawned item {stack.Def.GetId()} x{count}");
    }

    private void HandleExplosion(float power)
    {
        Debug.LogError($"[CauldronObj] EXPLOSION! power={power}");
        OnExplodeVisual?.Invoke(power);
        // TODO VISUAL
    }
    #endregion
}