using System;
using System.Collections.Generic;
using UnityEngine;

public class MortarObject : MonoBehaviour
{
    [SerializeField] private MortarRecipe[] recipeArray;
    [SerializeField] private int fluidTankCount = 1;
    [SerializeField] private float fluidTankCapacity = 9999f;
    [SerializeField] private Transform outputPoint;
    [SerializeField] private GameObject solidPrefab;
    [SerializeField] private float pestleBaseStep = 0.12f;

    private Mortar mortarData;
    private readonly HashSet<SolidObject> solidObjectSet = new HashSet<SolidObject>();
    
    public Action<ItemStack> OnSpawnSolid;

    private void Awake()
    {
        mortarData = new Mortar(recipeArray, fluidTankCount, fluidTankCapacity);
    }

    public void SolidEntered(SolidObject enteredObject)
    {
        if (!solidObjectSet.Add(enteredObject))
            return;

        IngredientStack ingredientInfo = enteredObject.GetIngredient();
        if (ingredientInfo is ItemStack)
        {
            ItemStack itemStackValue = (ItemStack)ingredientInfo;
            if (!itemStackValue.IsEmpty)
                mortarData.InsertSolid(itemStackValue);
        }

        TryCraft();
    }

    public void SolidExited(SolidObject leftObject)
    {
        if (solidObjectSet.Remove(leftObject))
        {
            RebuildSolidData();
            mortarData.RaiseInventoryChanged();
            TryCraft();
        }
    }

    public void ReceiveLiquid(FluidDef definition, float volume)
    {
        FluidStack stack = new FluidStack(definition, volume);
        mortarData.InsertLiquid(stack);
        TryCraft();
    }

    public void PestleHit()
    {
        mortarData.PestleHit(pestleBaseStep);
        TryCraft();
    }

    private void TryCraft()
    {
        MortarRecipe matchedRecipe;
        Dictionary<IngredientDef, float> consumeMap;
        List<ItemStack> productList;

        bool success = mortarData.TryCraft(out matchedRecipe, out consumeMap, out productList);
        if (!success)
            return;

        foreach (SolidObject eachSolid in solidObjectSet)
            if (eachSolid != null)
                Destroy(eachSolid.gameObject);
        solidObjectSet.Clear();

        mortarData.DoCraft(consumeMap);

        for (int i = 0; i < productList.Count; i++)
            SpawnProduct(productList[i]);
    }

    private void SpawnProduct(ItemStack stack)
    {
        if (stack.IsEmpty || stack.Def == null)
            return;

        int count = stack.amount;
        for (int i = 0; i < count; i++)
        {
            if (OnSpawnSolid != null)
            {
                ItemStack single = new ItemStack(stack.Def, 1);
                OnSpawnSolid(single);
                continue;
            }

            GameObject created = Instantiate(solidPrefab);
            SolidObject solidScript = created.GetComponent<SolidObject>();
            Rigidbody   rigidbodyComponent = created.GetComponent<Rigidbody>();

            solidScript.Init(stack);

            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            if (outputPoint != null)
                spawnPos = outputPoint.position;
            
            created.transform.position = spawnPos;

            if (rigidbodyComponent != null)
            {
                Vector3 forceVec = UnityEngine.Random.insideUnitSphere * 4f + Vector3.up * 5f;
                rigidbodyComponent.AddForce(forceVec, ForceMode.Impulse);
            }
        }
    }

    private void RebuildSolidData()
    {
        List<SolidObject> allSolids = new List<SolidObject>(solidObjectSet);
        mortarData.RaiseInventoryChanged();

        for (int i = 0; i < allSolids.Count; i++)
        {
            SolidObject eachObject = allSolids[i];
            if (eachObject == null)
                continue;

            IngredientStack info = eachObject.GetIngredient();
            if (info is ItemStack)
            {
                ItemStack itemStackValue = (ItemStack)info;
                if (!itemStackValue.IsEmpty)
                    mortarData.InsertSolid(itemStackValue);
                
            }
        }
    }
}