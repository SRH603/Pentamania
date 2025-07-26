using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InfiniteSource : MonoBehaviour
{
    public GameObject itemPrefab;
    
    public XRGrabInteractable initialItem;

    private XRGrabInteractable currentItem;

    void Start()
    {
        currentItem = initialItem != null ? initialItem : SpawnNewItem();
        Subscribe(currentItem);
    }
    
    private void Subscribe(XRGrabInteractable grab)
    {
        grab.selectEntered.RemoveListener(OnItemGrabbed);
        grab.selectEntered.AddListener(OnItemGrabbed);
    }
    
    private void OnItemGrabbed(SelectEnterEventArgs args)
    {
        SpawnNewItem();
        
        var oldGrab = args.interactableObject as XRGrabInteractable;
        oldGrab?.selectEntered.RemoveListener(OnItemGrabbed);
    }
    
    private XRGrabInteractable SpawnNewItem()
    {
        var go = Instantiate(itemPrefab, transform.position, transform.rotation, transform);
        var grab = go.GetComponent<XRGrabInteractable>();
        Subscribe(grab);
        currentItem = grab;
        return grab;
    }
}