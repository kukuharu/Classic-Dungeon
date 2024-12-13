using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemy : AttackingEnemy
{
    public float chargeSpeed = 5.0f; // Speed during the charge
    public float chargeDistance = 5.0f; // Maximum distance the charge can cover

    private Vector3 chargeDirection;
    private float chargeRemainingDistance;

    protected override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        HandleAttackCooldown();

        if (!IsAttacking)
        {
            if (PlayerInRange() && canAttack && attackTimer <= 0)
            {
                StartCharge();
            }
            else
            {
                HandleMovement(); // Movement resumes here when not attacking
            }
        }
        else
        {
            ExecuteCharge();
        }
    }

    private bool PlayerInRange()
    {
        return Vector3.Distance(playerTransform.position, transform.position) <= attackRange;
    }

    private void StartCharge()
    {
        IsAttacking = true; // Lock movement during charge
        attackTimer = attackCooldown;

        // Calculate direction and set initial charge distance
        chargeDirection = (playerTransform.position - transform.position).normalized;
        chargeRemainingDistance = chargeDistance;

        // Face the player's current location
        FacePlayer();

        // Trigger charge animation
        animate.SetTrigger("PerformCharge");
        Debug.Log("Enemy starts charging!");
    }

    private void ExecuteCharge()
    {
        if (chargeRemainingDistance > 0)
        {
            float moveDistance = chargeSpeed * Time.deltaTime;

            // Move the enemy in the charge direction
            transform.position += chargeDirection * moveDistance;
            chargeRemainingDistance -= moveDistance;
        }
        else
        {
            // End the charge if the distance is covered
            EndAttack();
        }
    }

    // Called via Animation Event at the end of the charge animation
    public override void EndAttack()
    {
        base.EndAttack(); // Reset IsAttacking
        Debug.Log("Charge attack ended.");
    }
}
