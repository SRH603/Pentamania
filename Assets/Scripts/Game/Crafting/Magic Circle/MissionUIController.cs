using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-100)]
public class MissionUIController : MonoBehaviour
{
    public static MissionUIController Instance { get; private set; }

    [Header("UI 引用")]
    [SerializeField] private TaskBar taskBarPrefab;
    [SerializeField] private Transform content;  // ScrollView/Content

    // taskId 索引 TaskBar
    private readonly Dictionary<string, TaskBar> _barDict = new();

    void Awake()
    {
        Instance = this;
        TaskManager tm = TaskManager.Instance;
        tm.OnTaskAccepted  += HandleAccepted;
        tm.OnTaskProgress  += HandleProgress;
        tm.OnTaskCompleted += HandleCompleted;
    }

    #region 事件回调
    public void HandleAccepted(TaskDefinition def)
    {
        TaskBar bar = Instantiate(taskBarPrefab, content);
        bar.nameDisplay.text        = def.title;
        bar.descriptionDisplay.text = def.description;
        bar.acceptButton.gameObject.SetActive(false);

        _barDict.Add(def.taskId, bar);
    }

    private void HandleProgress(TaskDefinition def, float ratio)
    {
        if (!_barDict.TryGetValue(def.taskId, out var bar)) return;
        bar.descriptionDisplay.text = $"{Mathf.RoundToInt(ratio * 100)}% finished";
    }

    private void HandleCompleted(TaskDefinition def)
    {
        if (!_barDict.TryGetValue(def.taskId, out var bar)) return;
        bar.descriptionDisplay.text = "TASK FINISHED";
        bar.acceptButton.interactable = false;
    }
    #endregion
}