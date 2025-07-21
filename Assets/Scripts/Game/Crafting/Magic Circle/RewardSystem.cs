using UnityEngine;

public static class RewardSystem
{
    public static void Give(RewardPack pack)
    {
        if (pack == null)
        { Debug.LogError("[TMS] RewardPack is null"); return; }

        //Debug.Log($"[TMS] +SkillPoint {pack.skillPoints}, Sanity {pack.sanityDelta}");
        // TODO

        foreach (var prefab in pack.rewardPrefabs)
        {
            if (prefab == null) continue;
            Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}