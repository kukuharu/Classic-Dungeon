using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarthaSzabolcs.Tutorial_SpriteFlash;

public class Player : Mover
{
    // Existing variables...
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;
    public AudioClip deathSound;
    public AudioClip levelSound;

    private Animator animator;   // Animator for Idle/Run/Dodge
    public bool isMoving;        // Animator for Idle/Run
    private bool isDodging = false; // Tracks if the player is currently dodging
    private float lastDodgeTime = 0; // Tracks the last dodge time
    public float dodgeCooldown = 1.0f; // Adjustable cooldown for dodging

    public float dodgeSpeedMultiplier = 2.5f; // Speed multiplier during dodge
    public float dodgeDuration = 0.5f;       // Duration of the dodge movement

    private BoxCollider2D bc; // Idle/Run

    public float interactRadius = 0.2f; // How far the player can interact

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Animator for Idle/Run/Dodge
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive || isDodging) // Can't take damage while dodging
            return;

        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitpointChange();
    }

    protected override void Death()
    {
        isAlive = false;
        AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
        GameManager.instance.deathMenuAnimator.SetTrigger("Show");
    }

    private void Update()
    {
        if (DialogueManager.isDialogueActive) return; // Skip input processing if dialogue is active

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        isMoving = (x != 0 || y != 0);      // Set isMoving to true if either x or y is non-zero
        animator.SetBool("isMoving", isMoving); // Animator for Idle/Run

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryDodge();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
            Debug.Log("Interact key pressed!");
        }
    }

    private void FixedUpdate()
    {
        if (DialogueManager.isDialogueActive) return; // Skip movement if dialogue is active
        if (isDodging)
            return; // Skip normal movement during dodge

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (isAlive)
            UpdateMotor(new Vector3(x, y, 0));
    }

    private void TryDodge()
    {
        // Check if dodging is allowed
        if (isDodging || Time.time - lastDodgeTime < dodgeCooldown)
            return;

        // Start the dodge
        StartCoroutine(Dodge());
    }

    private IEnumerator Dodge()
    {
        isDodging = true;
        lastDodgeTime = Time.time;
        animator.SetTrigger("Dodge"); // Trigger the dodge animation

        // Temporarily ignore collisions with enemies
        int playerLayer = gameObject.layer;
        int enemyLayer = LayerMask.NameToLayer("Actor");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // Capture the current movement direction or facing direction
        Vector3 dodgeDirection = moveDelta.normalized; // Use last move direction
        if (dodgeDirection == Vector3.zero) // Default to facing direction if stationary
        {
            dodgeDirection = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        }

        float dodgeTime = 0;
        float dodgeSpeed = xSpeed * dodgeSpeedMultiplier;

        while (dodgeTime < dodgeDuration)
        {
            // Move in the dodge direction with collision detection (ignoring enemies)
            Vector3 dodgeStep = dodgeDirection * dodgeSpeed * Time.deltaTime;

            // Perform collision checks for walls only
            if (mainCollider != null)
            {
                Vector2 colliderSize = mainCollider.bounds.size;

                // Check for Y-axis collision
                hit = Physics2D.BoxCast(
                    transform.position,
                    colliderSize,
                    0,
                    new Vector2(0, dodgeStep.y),
                    Mathf.Abs(dodgeStep.y),
                    LayerMask.GetMask("Blocking") // Exclude enemies
                );
                if (hit.collider == null)
                {
                    transform.Translate(0, dodgeStep.y, 0);
                }

                // Check for X-axis collision
                hit = Physics2D.BoxCast(
                    transform.position,
                    colliderSize,
                    0,
                    new Vector2(dodgeStep.x, 0),
                    Mathf.Abs(dodgeStep.x),
                    LayerMask.GetMask("Blocking") // Exclude enemies
                );
                if (hit.collider == null)
                {
                    transform.Translate(dodgeStep.x, 0, 0);
                }
            }

            dodgeTime += Time.deltaTime;
            yield return null;
        }

        // Wait for the animation event to end dodge
        while (!AnimatorDodgeDone())
        {
            yield return null;
        }

        // Re-enable collisions with enemies
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        isDodging = false;
    }


    private bool AnimatorDodgeDone()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log($"Current State: {stateInfo.fullPathHash}, Tag: {stateInfo.tagHash}");
        return stateInfo.IsTag("AnimatorDodgeDone");
    }

    private void Interact()
    {
        // Find all objects in the interaction radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

        foreach (var hit in hits)
        {
            // Check if the object implements IInteractable
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(); // Call the Interact method
                return; // Exit after interacting to avoid interacting with multiple things at once
            }
        }
    }

    public void SwapSprite(int skinId)
    {
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinId];
    }

    public void OnLevelUp()
    {
        maxHitpoint++;
        hitpoint = maxHitpoint;
    }

    public void SetLevel(int level)
    {
        OnLevelUp();
    }

    public void Heal(int healingAmount)
    {
        if (hitpoint == maxHitpoint)
            return;

        hitpoint += healingAmount;
        if (hitpoint > maxHitpoint)
            hitpoint = maxHitpoint;

        GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);
        GameManager.instance.OnHitpointChange();


    }

    public void Respawn()
    {
        Heal(maxHitpoint);
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // The color of the circle
        Gizmos.DrawWireSphere(transform.position, interactRadius); // Replace 1.5f with your radius
    }
}

