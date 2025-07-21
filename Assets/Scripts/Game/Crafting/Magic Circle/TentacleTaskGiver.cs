using UnityEngine;

public class TentacleTaskGiver : MonoBehaviour
{
    public TaskDefinition[] candidateTasks;
    
    public TaskOfferPanel offerPanelPrefab;
    public Transform content;

    private TaskDefinition _selected;
    private TaskOfferPanel _currentPanel;

    void Start()
    {
        _selected = PickRandomTask();
        ShowOfferUI();
    }

    private TaskDefinition PickRandomTask()
    {
        if (candidateTasks == null || candidateTasks.Length == 0) return null;
        return candidateTasks[Random.Range(0, candidateTasks.Length)];
    }
    
    private void ShowOfferUI()
    {
        if (_selected == null || offerPanelPrefab == null)
        {
            Debug.LogWarning($"[{name}] Offer UI Prefab / Task not assigned");
            return;
        }

        _currentPanel = Instantiate(offerPanelPrefab, content);
        _currentPanel.Init(this, _selected);
    }
    
    public void Accept()
    {
        Debug.Log($"Mission accepted {_selected.title}");
        if (_selected == null) return;
        
        MissionUIController.Instance?.HandleAccepted(_selected);
        TaskManager.Instance.RegisterTask(_selected);

        Despawn();
    }
    
    public void Decline()
    {
        //Despawn();
    }

    private void Despawn()
    {
        if (_currentPanel) Destroy(_currentPanel.gameObject);
        Destroy(gameObject);
    }
}