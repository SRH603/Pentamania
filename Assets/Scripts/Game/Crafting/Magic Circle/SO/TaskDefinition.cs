using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TMS/TaskDefinition")]
public class TaskDefinition : ScriptableObject
{
    public string taskId; //唯一编号
    [TextArea] public string title; //显示标题
    [TextArea] public string description; //显示解释
    public RequirementDefinition[] requirements; //实际内容
    public float timeLimitSeconds = 0f;          //0无时限
    public RewardPack rewardPack; //奖励
}