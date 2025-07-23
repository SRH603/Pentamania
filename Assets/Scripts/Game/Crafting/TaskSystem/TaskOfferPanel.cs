using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskOfferPanel : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text   titleText;
    public TMP_Text   descText;
    public Button     acceptBtn;
    //public Button     declineBtn;

    private TentacleTaskGiver giver;
    private TaskDefinition    task;
    
    public void Init(TentacleTaskGiver giver, TaskDefinition def)
    {
        this.giver = giver;
        task       = def;

        titleText.text = def.title;
        descText.text  = def.description;

        acceptBtn.onClick.AddListener(OnAccept);
            //declineBtn.onClick.AddListener(OnDecline);
    }

    private void OnAccept()
    {
        giver.Accept();
        Destroy(gameObject);
    }

    private void OnDecline()
    {
        giver.Decline();
        Destroy(gameObject);
    }
}