using System.Collections.Generic;
using UnityEngine;

public class MortarObject : MonoBehaviour
{
    [Header("Recipe / MixedDust")]
    [SerializeField] private MortarRecipe[] recipeArray;
    [SerializeField] private FluidDef mixedDust;

    [Header("Pestle / Output")]
    [SerializeField] private float pestleBaseStep = 0.15f;
    [SerializeField] private Transform outputPoint;

    [Header("VFX")]
    [SerializeField] private ParticleSystem vfxHit;
    [SerializeField] private ParticleSystem vfxInsert;
    [SerializeField] private ParticleSystem vfxSuccess;
    [SerializeField] private ParticleSystem vfxByproduct;
    [SerializeField] private ParticleSystem vfxScoop;

    private Mortar mortar;
    private readonly HashSet<SolidObject> solidsInside = new();
    
    //DEBUG
    [SerializeField] private List<ItemStack> itemStorage;
    [SerializeField] private FluidStorage fluidStorage;

    void Awake()
    {
        mortar = new Mortar(recipeArray, mixedDust);
        mortar.OnCraftComplete += PlayCraftFX;
        Debug.Log($"[MortarObj] Awake, recipes={recipeArray.Length}");
    }

    void Update()
    {
        //DEBUG
        itemStorage = mortar.GetItemStorage();
        fluidStorage = mortar.GetFluidStorage();
    }
    
    public void SolidEntered(SolidObject so)
    {
        if (!solidsInside.Add(so)) return;
        mortar.InsertSolid((ItemStack)so.GetIngredient());
        Debug.Log($"[MortarObj] Solid entered: {so.GetIngredient().GetAbstractDef().name}");
        if (vfxInsert) vfxInsert.Play();
    }

    public void SolidExited(SolidObject so)
    {
        if (!solidsInside.Remove(so)) return;

        ItemStack stack = (ItemStack)so.GetIngredient();
        mortar.RemoveSolid(stack);
        Debug.Log($"[MortarObj] Solid exited & removed: {stack.Def.GetId()}");
    }

    public void PestleHit()
    {
        mortar.PestleHit(pestleBaseStep);
        Debug.Log($"[MortarObj] Pestle hit, progress={mortar.Progress:F3}");
        if (vfxHit) vfxHit.Play();
        if (mortar.TryCraft())
        {
            Debug.Log("[MortarObj] TryCraft triggered");
            ClearInsertedSolids();
        }
    }

    public void TryScoop(IFluidContainer container)
    {
        if (!mortar.TryGetOnlyFluid(out var mortarFluid))
        {
            Debug.Log("[Mortar] Cannot scoop: none or multiple liquids");
            return;
        }
        if (!container.CanAccept(mortarFluid))
        {
            Debug.Log("[Mortar] Container can't accept this fluid");
            return;
        }

        float capacityLeft = container.Capacity - container.CurrentFluid.volume;
        if (capacityLeft <= 0f)
        {
            Debug.Log("[Mortar] Container is full");
            return;
        }
        FluidStack take = mortar.TakeFluid(mortarFluid);
        //Debug.Log($"[Mortar] Trying to Scoop with tags: {mortarFluid.tags.Count}");
        //Debug.Log($"[Mortar] Trying to Scoop with tags 2: {take.tags.Count}");
        float took = take.volume;
        take.volume = container.Fill(take);
        mortar.InsertLiquid(take);
        
        Debug.Log($"[Mortar] Scooped {took}, container now has {container.CurrentFluid.volume}");
    }

    private void PlayCraftFX(bool success)
    {
        if (success)
        {
            Debug.Log("[MortarObj] Craft success");
            if (vfxSuccess) vfxSuccess.Play();
        }
        else
        {
            Debug.Log("[MortarObj] Craft produced by-product");
            if (vfxByproduct) vfxByproduct.Play();
        }
    }

    private void ClearInsertedSolids()
    {
        foreach (var so in solidsInside)
            if (so) Destroy(so.gameObject);
        Debug.Log($"[MortarObj] Cleared {solidsInside.Count} solids after craft");
        solidsInside.Clear();
    }
}