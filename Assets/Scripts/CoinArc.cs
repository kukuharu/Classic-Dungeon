using UnityEngine;

public class CoinArc : MonoBehaviour
{
    [Header("Arc Motion Settings")]
    [Tooltip("Time it takes for the coin to complete its arc.")]
    public float flightDuration = 1f;
    [Tooltip("Height multiplier for the arc.")]
    public float arcHeightMultiplier = 1f;

    [Header("Collision Settings")]
    [Tooltip("Layer for collision detection.")]
    public LayerMask collisionLayer;
    [Tooltip("Radius used to detect collisions.")]
    public float collisionRadius = 0.05f;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float elapsedTime = 0f;
    private bool isLaunched = false;

    private Collider2D coinCollider;
    private Collectable collectable;

    public void Initialize(Vector2 target)
    {
        startPosition = transform.position;
        targetPosition = target;
        isLaunched = true;

        // Disable collectable functionality at the start
        coinCollider = GetComponent<Collider2D>();
        collectable = GetComponent<Collectable>();

        if (coinCollider != null)
            coinCollider.enabled = false; // Disable collision

        if (collectable != null)
            collectable.enabled = false; // Disable collectable behavior
    }

    private void Update()
    {
        if (!isLaunched) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / flightDuration);

        float height = Mathf.Sin(Mathf.PI * t) * arcHeightMultiplier;
        Vector2 position = Vector2.Lerp(startPosition, targetPosition, t);
        position.y += height;

        // Check for collisions
        if (CheckCollision(position))
        {
            isLaunched = false;
            transform.position = position;
            OnHitGround();
            return;
        }

        transform.position = position;

        if (t >= 1f)
        {
            isLaunched = false;
            // Ensure components are enabled even if no collision is detected
            OnHitGround();
        }
    }

    private bool CheckCollision(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, collisionRadius, collisionLayer);

        if (hit != null)
        {
            // Ignore collision with objects tagged as "Interactable"
            //if (hit.CompareTag("Interactable") || hit.CompareTag("Enemy") || hit.CompareTag("Hitbox"))
            if (hit.CompareTag("Interactable"))
            {
                return false;
            }

            return true; // Valid collision
        }

        return false; // No collision
    }

    private void OnHitGround()
    {
        if (coinCollider != null)
        {
            coinCollider.enabled = true;
            Debug.Log($"Collider enabled on {gameObject.name}");
        }

        if (collectable != null)
        {
            collectable.enabled = true;
            Debug.Log($"Collectable enabled on {gameObject.name}");
        }

        Debug.Log("Coin hit the ground!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);
    }
}
