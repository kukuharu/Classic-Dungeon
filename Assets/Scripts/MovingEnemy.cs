using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : Mover
{

    // This script sets up the chase behavior for the enemy:

    // The enemy monitors its distance from the player.
    // When the player comes within a certain range, it starts moving towards them using UpdateMotor.
    // If the enemy collides with the player, it stops.
    // If the player moves out of range, the enemy returns to its starting point.

    // Experience
    public int xpValue = 1;

    // Logic
    public float triggerLength = 1;
    public float chaseLength = 5;
    protected bool chasing;                   // Is he chasing player?
    protected bool collidingWithPlayer;       // Is he colliding with player? Therefore, do not move
    protected Transform playerTransform;
    protected Vector3 startingPosition;
    public GameObject deathEffect;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10];

    private MonsterLoading monsterLoading; // Loading monster animations.

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        monsterLoading = GetComponent<MonsterLoading>();  // Monster loading 
    }

    protected virtual void FixedUpdate()
    {
        // Is the player in range?

        // Skip movement if loading is still active
        if (monsterLoading != null && monsterLoading.IsLoading)
        {
            return;
        }

        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
                chasing = true;

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        // Check for overlaps

        collidingWithPlayer = false;
        mainCollider.Overlap(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            // The array is not cleaned up, so we do it ourself
            hits[i] = null;
        }

    }

    protected override void Death()
    {
        // Drop loot before destroying the enemy
        base.Death();


        Destroy(gameObject);
        Instantiate(deathEffect, transform.position, transform.rotation);
        GameManager.instance.GrantXp(xpValue);
        //GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, playerTransform.position, Vector3.up * 40, 1.0f);

        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);
    }
}