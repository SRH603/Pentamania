using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MortarSolidReceiver : MonoBehaviour
{
    private MortarObject mortar;
    private readonly HashSet<SolidObject> inside = new();

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        mortar = GetComponentInParent<MortarObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SolidObject solidObject = other.GetComponent<SolidObject>();
        if (!solidObject)
            return;
        if (solidObject.GetType() != typeof(SolidObject))
            return;
        
        if (inside.Add(solidObject))
        {
            mortar.SolidEntered(solidObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SolidObject solidObject = other.GetComponent<SolidObject>();
        if (!solidObject)
            return;
        if (solidObject.GetType() != typeof(SolidObject))
            return;
        
        if (inside.Remove(solidObject))
        {
            mortar.SolidExited(solidObject);
        }
    }
}