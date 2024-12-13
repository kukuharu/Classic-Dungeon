using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] monsterPrefabs;      // Array of possible monster prefabs to spawn
    public float spawnCooldown = 5f;         // Time between spawns
    public int maxSpawnedCreatures = 5;      // Max monsters allowed at a time
    public float spawnRadius = 3f;           // Radius around spawner to spawn monsters
    public float summonRange = 10f;          // Range within which the player must be to trigger summoning

    [Header("Summon Spell Effect")]
    public GameObject summonSpellEffect;     // Spell effect prefab to be instantiated on summon
    public Transform spellEffectSpawnPoint;  // Where to instantiate the effect on the necromancer
    public GameObject monsterSpawnEffect;   // Effect to be instantiated at monster's spawn location

    private float spawnTimer;
    private List<GameObject> spawnedMonsters = new List<GameObject>();

    private GameObject player;               // Reference to the player

    void Start()
    {
        // Find the GameObject named "Player"
        player = GameObject.Find("Player");
    }

    void Update()
    {
        // Remove any null entries from the list (monsters that have been destroyed)
        spawnedMonsters.RemoveAll(monster => monster == null);

        // Check if player is within summon range
        if (player && Vector3.Distance(transform.position, player.transform.position) <= summonRange)
        {
            // Check if we can spawn another monster
            if (spawnedMonsters.Count < maxSpawnedCreatures)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnCooldown)
                {
                    spawnTimer = 0;
                    TrySpawnMonster();
                }
            }
        }
    }

    void TrySpawnMonster()
    {
        Vector3 spawnPosition;
        if (FindValidSpawnPosition(out spawnPosition))
        {
            // Optional: Cast summon spell effect before spawning the monster
            if (summonSpellEffect && spellEffectSpawnPoint)
            {
                Instantiate(summonSpellEffect, spellEffectSpawnPoint.position, Quaternion.identity);
            }

            // Select a random monster prefab
            GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);

            // Instantiate the spawn effect at the monster's location if provided
            if (monsterSpawnEffect)
            {
                Instantiate(monsterSpawnEffect, spawnPosition, Quaternion.identity);
            }

            // Add the monster to the spawned monsters list
            spawnedMonsters.Add(newMonster);
        }
    }

    bool FindValidSpawnPosition(out Vector3 spawnPosition)
    {
        // Try to find a valid spawn position within a certain number of attempts
        const int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random position within a circle
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            Vector3 potentialPosition = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);

            // Perform a raycast to check line of sight from the spawner to the spawn position
            RaycastHit2D hit = Physics2D.Raycast(transform.position, potentialPosition - transform.position, spawnRadius);

            // Check if the raycast hit anything
            if (hit.collider == null)
            {
                spawnPosition = potentialPosition;
                return true;  // Valid position found
            }
        }

        // If no valid position found after maxAttempts, return false
        spawnPosition = Vector3.zero;
        return false;
    }
}
