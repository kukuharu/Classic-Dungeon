using UnityEngine;

public class FixedDirectionShooter : MonoBehaviour
{
    public GameObject bullet; // The bullet prefab
    public Transform bulletPos; // Where the bullet spawns
    public Vector3 fixedDirection = Vector3.right; // Default shooting direction
    public float cooldown = 1f; // Time between shots
    public float maxRange = 10f; // Maximum range to detect player
    public float bulletSpeed = 5f; // Speed of the bullet
    public AudioClip shootingSound; // Optional shooting sound

    private float timer;
    private GameObject player;
    private bool isAlive = true; // Tracks if this object is still valid

    void Start()
    {
        player = GameObject.Find("Player");

        // Warn if key references are missing
        if (bullet == null)
        {
            Debug.LogError("Bullet prefab not assigned in FixedDirectionShooter.");
        }

        if (bulletPos == null)
        {
            Debug.LogError("Bullet position (bulletPos) not assigned in FixedDirectionShooter.");
        }
    }

    void Update()
    {
        // Stop all logic if the object is destroyed
        if (!isAlive || player == null)
        {
            return;
        }

        // Ensure the player is within range
        if (Vector2.Distance(transform.position, player.transform.position) <= maxRange)
        {
            timer += Time.deltaTime;

            if (timer >= cooldown)
            {
                timer = 0;
                ShootInFixedDirection();

                if (shootingSound != null)
                {
                    AudioSource.PlayClipAtPoint(shootingSound, transform.position, 1f);
                }
            }
        }
    }

    void ShootInFixedDirection()
    {
        // Ensure bullet and bulletPos are assigned
        if (bullet == null || bulletPos == null)
        {
            Debug.LogWarning("Cannot shoot: bullet or bulletPos is not assigned.");
            return;
        }

        // Normalize the fixed direction to ensure proper scaling
        Vector3 shootDirection = fixedDirection.normalized;

        // Instantiate the bullet
        GameObject newBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);

        // Assign velocity to the bullet
        EnemyBulletScript bulletScript = newBullet.GetComponent<EnemyBulletScript>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(shootDirection, bulletSpeed);
        }
        else
        {
            Debug.LogWarning("Bullet prefab missing EnemyBulletScript component.");
        }
    }

    private void OnDestroy()
    {
        // Mark the object as no longer valid to stop all logic
        isAlive = false;
    }
}
