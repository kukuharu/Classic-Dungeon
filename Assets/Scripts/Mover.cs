using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter
{
    private Vector3 originalSize;
    protected Collider2D mainCollider; // Can be either BoxCollider2D or CapsuleCollider2D
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    public float ySpeed = 0.75f;
    public float xSpeed = 1.0f;

    // This just checks for a boxcollider2d or a capsule collider 2d
    protected virtual void Start()
    {
        originalSize = transform.localScale;

        // Try to get a BoxCollider2D or CapsuleCollider2D
        mainCollider = GetComponent<BoxCollider2D>();
        if (mainCollider == null)
        {
            mainCollider = GetComponent<CapsuleCollider2D>();
        }

        // Check if no collider was found
        if (mainCollider == null)
        {
            Debug.LogWarning("No BoxCollider2D or CapsuleCollider2D found on " + gameObject.name);
        }
    }

    // This is the movement script. 
    // this class manages basic movement for an object, accounting for sprite direction and pushing effects.
    protected virtual void UpdateMotor(Vector3 input)
    {
        // Reset moveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0);

        // Add push vector, if any
        moveDelta += pushDirection;

        // Reduce push force every frame, based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        // Ensure mainCollider is assigned
        if (mainCollider == null)
            return;

        // Determine the appropriate cast based on collider type
        Vector2 colliderSize = mainCollider.bounds.size;

        if (mainCollider is CapsuleCollider2D)
        {
            // Use CapsuleCast for CapsuleCollider2D
            CapsuleCollider2D capsuleCollider = (CapsuleCollider2D)mainCollider;

            hit = Physics2D.CapsuleCast(transform.position, colliderSize, capsuleCollider.direction, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
            }

            hit = Physics2D.CapsuleCast(transform.position, colliderSize, capsuleCollider.direction, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            // Use BoxCast for BoxCollider2D
            hit = Physics2D.BoxCast(transform.position, colliderSize, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
            }

            hit = Physics2D.BoxCast(transform.position, colliderSize, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
            if (hit.collider == null)
            {
                transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
            }
        }

        // If the object is the player, flip the sprite based on the mouse position
        if (gameObject.name == "Player")
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePosition.x > transform.position.x)
            {
                transform.localScale = originalSize;
            }
            else
            {
                transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.z);
            }
        }
        else
        {
            // Flip sprite direction for other objects based on movement
            if (moveDelta.x > 0)
                transform.localScale = originalSize;
            else if (moveDelta.x < 0)
                transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.z);
        }

        // Debug visualization for capsule and box cast
        Debug.DrawLine(transform.position, transform.position + (Vector3)moveDelta.normalized * colliderSize.magnitude, Color.red);
    }
}
