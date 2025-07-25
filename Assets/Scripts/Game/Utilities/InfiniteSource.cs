using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class InfiniteSourceList : MonoBehaviour
{
    public List<XRGrabInteractable> initialItems = new();
    
    private readonly Dictionary<XRGrabInteractable, Vector3> spawnPos = new();
    private readonly Dictionary<XRGrabInteractable, Quaternion> spawnRot = new();
    private readonly Dictionary<XRGrabInteractable, GameObject> prefabs = new();

    void Awake()
    {
        foreach (var item in initialItems)
        {
            if (item == null)
                continue;


            spawnPos[item] = item.transform.position;
            spawnRot[item] = item.transform.rotation;
            prefabs[item] = item.gameObject;

            FreezeItem(item);
            Subscribe(item);
        }
    }
    
    private void Subscribe(XRGrabInteractable grab)
    {
        grab.selectEntered.RemoveListener(OnItemGrabbed);
        grab.selectEntered.AddListener(OnItemGrabbed);
    }

    private void OnItemGrabbed(SelectEnterEventArgs args)
    {
        var grabbed = args.interactableObject as XRGrabInteractable;
        if (grabbed == null)
            return;

        UnfreezeItem(grabbed);
        SpawnReplacement(grabbed);
        grabbed.selectEntered.RemoveListener(OnItemGrabbed);
    }
    
    private void SpawnReplacement(XRGrabInteractable source)
    {
        var position = spawnPos[source];
        var rotation = spawnRot[source];
        var prefab = prefabs[source];

        var instantiate = Instantiate(prefab, position, rotation, transform);
        var grab = instantiate.GetComponent<XRGrabInteractable>();

        FreezeItem(grab);
        spawnPos[grab] = position;
        spawnRot[grab] = rotation;
        prefabs[grab] = prefab;

        Subscribe(grab);
    }

    private static void FreezeItem(XRGrabInteractable grab)
    {
        var rigidbody = grab.GetComponent<Rigidbody>();
        if (rigidbody) rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private static void UnfreezeItem(XRGrabInteractable grab)
    {
        var rigidbody = grab.GetComponent<Rigidbody>();
        if (rigidbody)
            rigidbody.constraints = RigidbodyConstraints.None;
    }
}
