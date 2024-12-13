using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour, IInteractable
{
    [Header("Chest Settings")]
    public bool isOpen = false; // Tracks chest state
    private Animator animator;  // Animator reference

    [System.Serializable]
    public class LootItem
    {
        public GameObject prefab;         // Prefab for the loot
        [Tooltip("Fixed number of this loot type to spawn.")]
        public int fixedCount = 0;        // Fixed number of items
        [Tooltip("Maximum random additional count.")]
        public int maxRandomCount = 0;    // Additional random amount
    }

    [Header("Loot Settings")]
    [Tooltip("List of loot types to spawn when the chest is opened.")]
    public List<LootItem> lootItems = new List<LootItem>(); // List of loot types

    [Header("Spawn Settings")]
    [Tooltip("Maximum spread angle (in degrees) for loot.")]
    public float spreadAngle = 30f;     // Spread angle for the loot
    [Tooltip("Minimum distance the loot can land.")]
    public float minDistance = 0.05f;   // Minimum distance from chest
    [Tooltip("Maximum distance the loot can land.")]
    public float maxDistance = 0.1f;    // Maximum distance from chest

    [Header("Collision Settings")]
    [Tooltip("Layer to check for loot collisions.")]
    public LayerMask collisionLayer;    // Layer to check for collisions

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
    }

    public void Interact()
    {
        OpenChest();
    }

    public void OpenChest()
    {
        if (!isOpen)
        {
            isOpen = true;
            Debug.Log("Opening chest...");
            animator.SetBool("IsOpen", true);
            StartCoroutine(SpawnLoot());
        }
        else
        {
            Debug.Log("Chest is already open!");
        }
    }

    private IEnumerator SpawnLoot()
    {
        foreach (LootItem loot in lootItems)
        {
            if (loot.prefab == null) continue;

            // Calculate total items to spawn
            int totalCount = loot.fixedCount + Random.Range(0, loot.maxRandomCount + 1);

            for (int i = 0; i < totalCount; i++)
            {
                GameObject lootInstance = Instantiate(loot.prefab, transform.position, Quaternion.identity);

                // Randomize a target position within a radius
                float angle = Random.Range(-spreadAngle / 2f, spreadAngle / 2f) * Mathf.Deg2Rad;
                float distance = Random.Range(minDistance, maxDistance);
                Vector2 targetPosition = (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                // Initialize the loot's arc motion
                CoinArc coinArc = lootInstance.GetComponent<CoinArc>();
                if (coinArc != null)
                {
                    coinArc.Initialize(targetPosition);
                }
                else
                {
                    Debug.LogWarning("Loot prefab is missing the CoinArc component!");
                }

                yield return new WaitForSeconds(0.1f); // Optional delay between spawns
            }
        }
    }
}
