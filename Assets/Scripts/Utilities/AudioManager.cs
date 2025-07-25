using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioManager : MonoBehaviour
{
    public EventReference pickupEvent;
    public EventReference dropEvent;
    public EventReference firepitEvent;


    public EventInstance pickupInstance;
    public EventInstance dropInstance;


    public static AudioManager instance;
    void Start()
    {

    }

    void Awake()
    {
        // Who up singling their ton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public void playPickup(int itemType, Transform location)
    {
        pickupInstance = RuntimeManager.CreateInstance(pickupEvent);
        RuntimeManager.AttachInstanceToGameObject(pickupInstance, location);
        pickupInstance.setParameterByName("Item", itemType);
        pickupInstance.start();
        pickupInstance.release();
    }

    public void playDrop(int itemType, Transform location)
    {
        dropInstance = RuntimeManager.CreateInstance(dropEvent);
        RuntimeManager.AttachInstanceToGameObject(dropInstance, location);
        dropInstance.setParameterByName("Item", itemType);
        dropInstance.start();
        dropInstance.release();
    }

    public void playSoundAtLocation(string sound, Transform location)
    {
        switch (sound)
        {
            case "firepit":
                EventInstance firepitInstance = RuntimeManager.CreateInstance(firepitEvent);
                RuntimeManager.AttachInstanceToGameObject(firepitInstance, location);
                firepitInstance.start();
                firepitInstance.release();
                break;
        }
    }

    public void PickupIngredient(PassableIngredientObject ingredientObject)
    {
        pickupInstance = RuntimeManager.CreateInstance(pickupEvent);
        RuntimeManager.AttachInstanceToGameObject(pickupInstance, ingredientObject.gameObject);
        print("playing ts pmo " + ingredientObject.GetAudioType());
        pickupInstance.setParameterByName("Item", ingredientObject.GetAudioType());
        pickupInstance.start();
        //pickupInstance.release();
    }
}
