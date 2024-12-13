using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootItem
{
    [Tooltip("The loot prefab to drop.")]
    public GameObject prefab;

    [Tooltip("The guaranteed number of this loot to drop.")]
    public int fixedCount = 1;

    [Tooltip("Minimum number of random additional drops.")]
    public int randomMin = 0;

    [Tooltip("Maximum number of random additional drops.")]
    public int randomMax = 0;

    [Tooltip("Chance for this loot item to drop (0 to 1, where 1 is 100%).")]
    public float dropChance = 1f;
}

public class LootDropper : MonoBehaviour
{
    [Header("Loot Settings")]
    [Tooltip("List of loot items to potentially drop.")]
    public List<LootItem> lootItems = new List<LootItem>();

    [Header("Loot Spread Settings")]
    [Tooltip("Maximum spread angle (in degrees) for loot.")]
    public float spreadAngle = 180f;
    [Tooltip("Minimum distance loot can land from the enemy.")]
    public float minDistance = 0.05f;
    [Tooltip("Maximum distance loot can land from the enemy.")]
    public float maxDistance = 0.1f;

    public void DropLoot()
    {
        foreach (LootItem lootItem in lootItems)
        {
            // Skip loot if no prefab or if chance to drop fails
            if (lootItem.prefab == null || Random.value > lootItem.dropChance)
                continue;

            // Calculate total number of items to drop
            int totalCount = lootItem.fixedCount + Random.Range(lootItem.randomMin, lootItem.randomMax + 1);

            for (int i = 0; i < totalCount; i++)
            {
                // Spawn loot item
                GameObject loot = Instantiate(lootItem.prefab, transform.position, Quaternion.identity);

                // Determine random target position
                float angle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f) * Mathf.Deg2Rad;
                float distance = Random.Range(minDistance, maxDistance);
                Vector2 targetPosition = (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                // Initialize arc motion on the loot
                CoinArc lootArc = loot.GetComponent<CoinArc>();
                if (lootArc != null)
                {
                    lootArc.Initialize(targetPosition);
                }
                else
                {
                    Debug.LogWarning($"Loot prefab '{lootItem.prefab.name}' is missing the CoinArc component!");
                }
            }
        }
    }
}
