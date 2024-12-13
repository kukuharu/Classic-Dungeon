using UnityEngine;

public class MovingHalt : MovingEnemy
{
    public float haltDuration = 2.0f; // Cooldown duration for halt
    private float haltTimer = 0.0f;   // Timer to manage halt state
    private bool isHalted = false;    // Tracks if movement is halted

    private void Update()
    {
        if (isHalted)
        {
            haltTimer -= Time.deltaTime;
            if (haltTimer <= 0)
            {
                ResumeMovement();
            }
        }
    }

    public void HaltMovement()
    {
        if (!isHalted)
        {
            isHalted = true;
            haltTimer = haltDuration;
            Debug.Log("Movement halted for " + haltDuration + " seconds.");
        }
    }

    private void ResumeMovement()
    {
        if (isHalted)
        {
            isHalted = false;
            Debug.Log("Movement resumed.");
        }
    }

    // Override movement behavior when halted
    protected override void FixedUpdate()
    {
        if (isHalted) return; // Stop all movement logic if halted
        base.FixedUpdate();   // Otherwise, proceed with default behavior
    }
}