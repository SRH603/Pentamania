using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    private readonly List<ActiveTask> activeTasks = new List<ActiveTask>();
    

    public event Action<TaskDefinition> OnTaskAccepted;
    public event Action<TaskDefinition, float> OnTaskProgress; // ratio 0 - 1
    public event Action<TaskDefinition> OnTaskCompleted;
    
    internal void RaiseProgress(TaskDefinition d, float ratio)
        => OnTaskProgress?.Invoke(d, ratio);
    internal void RaiseCompleted(TaskDefinition d)
        => OnTaskCompleted?.Invoke(d);

    #region Awake
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

    #region API
    public bool RegisterTask(TaskDefinition def)
    {
        if (def == null) return false;
        if (activeTasks.Exists(t => t.definition.taskId == def.taskId)) return false;

        var at = new ActiveTask(def, this);   // 传入 TaskManager 引用
        activeTasks.Add(at);

        OnTaskAccepted?.Invoke(def);
        return true;
    }
    #endregion

    #region Update
    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = activeTasks.Count - 1; i >= 0; i--)
        {
            var t = activeTasks[i];
            if (t.UpdateCountdown(dt))
            {
                // Failed
                Debug.LogWarning($"[TMS] Task failed: {t.definition.title}");
                activeTasks.RemoveAt(i);
            }
        }
    }

    public void OnItemCrafted(string itemId)
    {
        foreach (var t in activeTasks)
            t.TryProgress(RequirementType.CraftItem, itemId, 1);
    }

    public void OnItemDelivered(string itemId, int qty)
    {
        Debug.Log($"[OnItemDelivered] listCount={activeTasks.Count}");
        foreach (var t in activeTasks)
        {
            t.TryProgress(RequirementType.DeliverItem, itemId, qty);
        }
    }
    #endregion

    #region ActiveTask
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
            _mgr.activeTasks.Remove(this);
        }
    }
    #endregion
}
