

using System;
using UnityEngine;

public class SolidObject : PassableIngredientObject
{
    [SerializeField] private ItemStack ingredient;

    public void Start()
    {
        Init(ingredient);
    }

    public override IngredientStack GetIngredient()
    {
        return ingredient;
    }

    public override void SetIngredient(IngredientStack ingredient)
    {
        if (ingredient is ItemStack solidIngredient)
        {
            // handle logic for the change in ingredient
            this.ingredient = solidIngredient;
            // set material and mesh
            gameObject.transform.localScale = solidIngredient.Def.GetScale();
            gameObject.GetComponent<MeshRenderer>().material = solidIngredient.Def.GetMaterial();
            gameObject.GetComponent<MeshFilter>().mesh = solidIngredient.Def.GetMesh();
            gameObject.GetComponent<MeshCollider>().sharedMesh = solidIngredient.Def.GetMesh();
        }
        else
        {
            // throw some error
        }
    }

    public void Init(ItemStack item)
    {
        ingredient = item;
        gameObject.transform.localScale = item.Def.GetScale();
        gameObject.GetComponent<MeshRenderer>().material = item.Def.GetMaterial();
        gameObject.GetComponent<MeshFilter>().mesh = item.Def.GetMesh();
        gameObject.GetComponent<MeshCollider>().sharedMesh = item.Def.GetMesh();
    }
}
