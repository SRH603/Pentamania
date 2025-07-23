using UnityEngine;

[CreateAssetMenu(menuName = "TMS/RewardPack")]
public class RewardPack : ScriptableObject
{
    public int skillPoints;
    public int sanityDelta;
    public GameObject[] rewardPrefabs;
}