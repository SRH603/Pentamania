using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    private readonly List<ActiveTask> _activeTasks = new List<ActiveTask>();
    
    // === 新增事件 ===
    public event Action<TaskDefinition> OnTaskAccepted;
    public event Action<TaskDefinition, float> OnTaskProgress; // ratio 0~1
    public event Action<TaskDefinition> OnTaskCompleted;

    // 供内部任务调用，用于把进度/完成通知抛给 UI
    internal void RaiseProgress(TaskDefinition d, float ratio)
        => OnTaskProgress?.Invoke(d, ratio);
    internal void RaiseCompleted(TaskDefinition d)
        => OnTaskCompleted?.Invoke(d);

    #region 生命周期
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region 对外API
    // 修改 RegisterTask，成功后触发 OnTaskAccepted
    public bool RegisterTask(TaskDefinition def)
    {
        if (def == null) return false;
        if (_activeTasks.Exists(t => t.definition.taskId == def.taskId)) return false;

        var at = new ActiveTask(def, this);   // 传入 TaskManager 引用
        _activeTasks.Add(at);

        OnTaskAccepted?.Invoke(def);
        return true;
    }
    #endregion

    #region 更新与事件转发
    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = _activeTasks.Count - 1; i >= 0; i--)
        {
            var t = _activeTasks[i];
            if (t.UpdateCountdown(dt))
            {
                // Failed
                Debug.LogWarning($"[TMS] Task failed: {t.definition.title}");
                _activeTasks.RemoveAt(i);
            }
        }
    }

    public void OnItemCrafted(string itemId)
    {
        foreach (var t in _activeTasks)
            t.TryProgress(RequirementType.CraftItem, itemId, 1);
    }

    public void OnItemDelivered(string itemId, int qty)
    {
        Debug.Log($"[OnItemDelivered] listCount={_activeTasks.Count}");
        foreach (var t in _activeTasks)
        {
            t.TryProgress(RequirementType.DeliverItem, itemId, qty);
        }
    }
    #endregion

    #region 内部ActiveTask
    private class ActiveTask
    {
        public TaskDefinition definition;
        private readonly Dictionary<RequirementDefinition, int> _progress = new();

        private float _timer;
        private TaskManager _mgr;

        public ActiveTask(TaskDefinition def, TaskManager mgr)
        {
            definition = def;
            _mgr = mgr;
            _timer = def.timeLimitSeconds;
            foreach (var req in def.requirements)
                _progress[req] = 0;
        }
        
        /// <summary>
        /// Countdown for finish or fail
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool UpdateCountdown(float delta)
        {
            if (definition.timeLimitSeconds <= 0) return false;
            _timer -= delta;
            if (_timer <= 0f)
            {
                return true;
            }
            return false;
        }

        public void TryProgress(RequirementType type, string id, int qty)
        {
            Debug.Log("111");
            foreach (var req in definition.requirements)
            {
                if (req.requirementType != type || req.targetId != id) continue;

                _progress[req] = Mathf.Clamp(
                    _progress[req] + qty, 0, req.quantity);
                
                // 进度条：完成量 / 需求量
                int current = _progress[req];
                float ratio = Mathf.Clamp01((float)current / req.quantity);
                _mgr.RaiseProgress(definition, ratio);

                // Finished
                if (IsFinished())
                {
                    Complete();
                }
            }
        }

        private bool IsFinished()
        {
            foreach (var kv in _progress)
            {
                if (kv.Value < kv.Key.quantity) return false;
            }
            return true;
        }

        private void Complete()
        {
            RewardSystem.Give(definition.rewardPack);
            _mgr.RaiseCompleted(definition);
            _mgr._activeTasks.Remove(this);
        }
    }
    #endregion
}
