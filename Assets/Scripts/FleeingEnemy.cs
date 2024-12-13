using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingEnemy : Mover
{
    public int xpValue = 1;
    public GameObject deathEffect;
    private Transform playerTransform;
    public float fleeRange = 5f;               // Distance within which the enemy starts fleeing
    public float obstacleCheckDistance = 0.5f; // Distance to check for obstacles
    public float bounceAngle = 45f;            // Angle to adjust when hitting an obstacle
    public LayerMask obstacleLayer;            // Layer for obstacles like walls

    private Vector3 currentFleeDirection;

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        canBePushed = false;
        currentFleeDirection = (transform.position - playerTransform.position).normalized;
    }

    void Update()
    {
        // Update flee direction based on player position if in range
        if (Vector3.Distance(transform.position, playerTransform.position) < fleeRange)
        {
            currentFleeDirection = (transform.position - playerTransform.position).normalized;

            // Check for obstacles in the flee direction
            if (Physics2D.Raycast(transform.position, currentFleeDirection, obstacleCheckDistance, obstacleLayer))
            {
                // Bounce off by changing direction at a random angle
                currentFleeDirection = BounceDirection(currentFleeDirection);
            }

            // Move in the current (adjusted if necessary) flee direction
            UpdateMotor(currentFleeDirection);
        }
        else
        {
            // Stop if player is out of range
            UpdateMotor(Vector3.zero);
        }

        // Ensure the enemy always faces the player while fleeing
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        if ((directionToPlayer.x > 0 && transform.localScale.x < 0) || (directionToPlayer.x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    // Bounce the flee direction by a random angle to avoid obstacles
    Vector3 BounceDirection(Vector3 originalDirection)
    {
        // Choose a random angle to rotate the direction (either clockwise or counter-clockwise)
        float randomAngle = bounceAngle * (Random.Range(0, 2) * 2 - 1); // Randomly -bounceAngle or +bounceAngle
        Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);

        // Apply rotation to the original flee direction
        return rotation * originalDirection;
    }

    protected override void Death()
    {
        base.Death();
        Destroy(gameObject);
        Instantiate(deathEffect, transform.position, transform.rotation);
        GameManager.instance.GrantXp(xpValue);
        //GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, playerTransform.position, Vector3.up * 40, 1.0f); old
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f); 
        
    }
}
