using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingEnemy : MovingEnemy
{
    // Fields
    public bool canAttack = true;
    public float attackRange = 0.2f;
    public float attackCooldown = 2.0f;

    protected float attackTimer = 0.0f; // Protected for subclass access
    protected Animator animate; // Accessible to subclasses

    public bool PlayerInAttackZone { get; set; } // Public setter for external control
    public bool IsAttacking { get; protected set; } // Protected setter for subclasses
    public AudioClip smashSoundClip; // impact sound

    // Screen shake settings
    public float screenShakeDuration = 0.3f; // Adjustable in inspector
    public float screenShakeMagnitude = 0.01f; // Adjustable in inspector

    protected override void Start()
    {
        base.Start();
        animate = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        HandleAttackCooldown();

        if (!IsAttacking)
        {
            if (PlayerInAttackZone && canAttack && attackTimer <= 0)
            {
                PerformAttack();
            }
            else
            {
                HandleMovement(); // Movement resumes here when not attacking
            }
        }
    }

    protected virtual void HandleAttackCooldown() // Virtual to allow overrides
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            //Debug.Log($"Cooldown active. Timer: {attackTimer:F2}");
        }
    }

    protected virtual void PerformAttack() // Virtual for overriding
    {
        IsAttacking = true; // Lock movement during attack
        attackTimer = attackCooldown;

        // Face the player
        FacePlayer();

        // Trigger attack animation
        animate.SetTrigger("PerformAttack");
       // Debug.Log("Enemy attacks!");

        // Reset PlayerInAttackZone to prevent continuous attacks
        PlayerInAttackZone = false;
    }

    public void PlaySmashSound()
    {
        AudioSource.PlayClipAtPoint(smashSoundClip, transform.position, 1f);
    }


    protected virtual void FacePlayer() // Virtual for customizable facing logic
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        if (directionToPlayer.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionToPlayer.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    protected virtual void HandleMovement() // Virtual for customizable movement logic
    {
        if (Vector3.Distance(playerTransform.position, transform.position) < chaseLength)
        {
            chasing = Vector3.Distance(playerTransform.position, transform.position) < triggerLength;

            if (chasing && !collidingWithPlayer)
            {
                UpdateMotor((playerTransform.position - transform.position).normalized);
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            chasing = false;
            UpdateMotor(startingPosition - transform.position);
        }
    }

    // Called via Animation Event at the end of the attack animation
    public virtual void EndAttack() // Virtual for custom behavior in subclasses
    {
        IsAttacking = false; // Allow movement again
        //Debug.Log("Attack animation ended; IsAttacking reset to false.");
    }

    public void TriggerScreenShake()
    {
        CameraMotor cameraMotor = Camera.main.GetComponent<CameraMotor>();
        if (cameraMotor != null)
        {
            cameraMotor.TriggerScreenShake(screenShakeDuration, screenShakeMagnitude);
        }
    }

}
