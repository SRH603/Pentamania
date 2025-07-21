using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 触手生成的“任务说明面板”脚本。
/// 负责把文字填充进去，并把“接受 / 拒绝”按钮事件回调给 TentacleTaskGiver。
/// </summary>
public class TaskOfferPanel : MonoBehaviour
{
    [Header("UI 引用")]
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