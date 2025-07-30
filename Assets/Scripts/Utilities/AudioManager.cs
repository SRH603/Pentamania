using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioManager : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private EventReference firepitEvent;
    [SerializeField] private EventReference clockTickEvent;


    [Header("Machines")]
    [SerializeField] private EventReference crystalBallAmbience;
    [SerializeField] private EventReference crystallBallUsage;
    public EventInstance crystallBallUsageInstance;

    [SerializeField] private EventReference mortarGrind;

    [SerializeField] private EventReference cauldronStart;
    [SerializeField] private EventReference cauldronSuccess;
    [SerializeField] private EventReference cauldronFailure;
    [SerializeField] private EventReference cauldronTakeIngredient;
    [SerializeField] private EventReference cauldronShaking;

    [SerializeField] private EventReference bunsenBurnerStart;
    [SerializeField] private EventReference bunsenBurnerStop;


    [Header("Items")]
    [SerializeField] private EventReference pickupEvent;
    [SerializeField] private EventReference dropEvent;
    [SerializeField] private EventReference pageFlip;

    [Header("Voicelines")]
    [SerializeField] private EventReference voiceLineManager;
    public EventInstance voiceLineManagerInstance;

    [Header("Music")] 
    [SerializeField] private EventReference musicManager;
    public EventInstance musicManagerInstance;

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

    public void playEventInstance(string instance, string paramName, int param, Transform location)
    {
        EventInstance eventInstance;

        switch (instance)
        {
            case "pickup":
                eventInstance = RuntimeManager.CreateInstance(pickupEvent);
                break;
            
            case "drop":
                eventInstance = RuntimeManager.CreateInstance(dropEvent);
                break;
            
            default:
                Debug.LogError("AudioManager: playEventInstance - Invalid instance name provided: " + instance);
                return;

        }        
        RuntimeManager.AttachInstanceToGameObject(eventInstance, location);
        eventInstance.setParameterByName(paramName, param);
        eventInstance.start();
        eventInstance.release();
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
    
    public void PlayVoiceLine(int voicelineEvent, Transform location, int potionId = -1)
    {
        voiceLineManagerInstance = RuntimeManager.CreateInstance(voiceLineManager);
        RuntimeManager.AttachInstanceToGameObject(voiceLineManagerInstance, location);
        voiceLineManagerInstance.setParameterByName("Voiceline Event", voicelineEvent);
        if (potionId != -1)
        {
            voiceLineManagerInstance.setParameterByName("Potion Brewed", potionId);
        }

        voiceLineManagerInstance.start();
        voiceLineManagerInstance.release();
    }
}
