using UnityEngine;

[DefaultExecutionOrder(100)]
public class TaskAssigner : MonoBehaviour
{
    public static TaskAssigner Instance { get; private set; }
    
    [SerializeField] private TaskDef[] taskSequence;

    private int nextIndex;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        nextIndex = 0;
        TryAssignNext();
        
    }
    
    public void TryAssignNext()
    {
        if (TaskManager.Instance.ActiveTasks.Count > 0)
        {
            Debug.Log("[Task System] One or more task is running");
            return;
        }

        if (nextIndex >= taskSequence.Length)
        {
            Debug.Log("You finished the game, thank you for playing our game -- Us"); return;
        }

        TaskManager.Instance.PushTask(taskSequence[nextIndex]);
        nextIndex++;
    }
}