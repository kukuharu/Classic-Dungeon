using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehavior : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float cooldown = 1;
    public float maxRange = 10;
    public int projectileCount = 1;
    public float spreadAngle = 15f;
    public float bulletSpeed = 5f;
    public bool randomizeSpread = false;
    public bool burstFire = false;
    public int burstCount = 3;
    public float burstCooldown = 0.1f;
    public AudioClip shootingSound;

    // Fixed direction settings
    public bool fixedDirection = false;
    public Vector3 fixedDirectionAngle = Vector3.right; // Default to right

    // Wave pattern variables
    public bool useWavePattern = false;
    public float waveAmplitude = 5f;
    public float waveFrequency = 2f;

    // Line formation variables
    public bool useLineFormation = false;
    public float lineSpreadDistance = 1f; // Distance between projectiles in a line

    private float timer;
    private GameObject player;

    void Start()
    {
        if (!fixedDirection)
        {
            player = GameObject.Find("Player");
        }
    }

    void Update()
    {
        if (fixedDirection || Vector2.Distance(transform.position, player.transform.position) < maxRange)
        {
            timer += Time.deltaTime;
            if (timer > cooldown)
            {
                timer = 0;
                if (burstFire)
                {
                    StartCoroutine(BurstFire());
                }
                else
                {
                    ShootProjectiles();
                }
                AudioSource.PlayClipAtPoint(shootingSound, transform.position, 1f);
            }
        }
    }

    void ShootProjectiles()
    {
        if (useLineFormation)
        {
            ShootInLine();
            return;
        }

        float angleToShoot;

        if (fixedDirection)
        {
            // Use a fixed direction (converts Vector3 to an angle in degrees)
            angleToShoot = Mathf.Atan2(fixedDirectionAngle.y, fixedDirectionAngle.x) * Mathf.Rad2Deg;
        }
        else
        {
            // Aim at player
            Vector3 directionToPlayer = (player.transform.position - bulletPos.position).normalized;
            angleToShoot = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        }

        // Calculate starting angle for spread
        float startAngle = projectileCount > 1 ? angleToShoot - spreadAngle / 2 : angleToShoot;
        float angleIncrement = spreadAngle / Mathf.Max(1, projectileCount - 1);

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = startAngle + (angleIncrement * i);

            if (useWavePattern)
            {
                float waveOffset = Mathf.Sin(Time.time * waveFrequency + i) * waveAmplitude;
                angleOffset += waveOffset;
            }
            else if (randomizeSpread)
            {
                angleOffset += Random.Range(-spreadAngle / 2, spreadAngle / 2);
            }

            Vector3 direction = new Vector3(Mathf.Cos(angleOffset * Mathf.Deg2Rad), Mathf.Sin(angleOffset * Mathf.Deg2Rad), 0);
            GameObject newBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);

            EnemyBulletScript bulletScript = newBullet.GetComponent<EnemyBulletScript>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(direction, bulletSpeed);
            }
        }
    }

    void ShootInLine()
    {
        Vector3 directionToShoot;

        if (fixedDirection)
        {
            // Use a fixed direction
            directionToShoot = fixedDirectionAngle.normalized;
        }
        else
        {
            // Aim directly at player
            directionToShoot = (player.transform.position - bulletPos.position).normalized;
        }

        // Determine perpendicular direction for the line
        Vector3 perpendicular = Vector3.Cross(directionToShoot, Vector3.forward).normalized;

        // Calculate starting position offsets for the line
        float startOffset = -((projectileCount - 1) * lineSpreadDistance) / 2;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 finalPositionOffset = perpendicular * (startOffset + i * lineSpreadDistance);

            GameObject newBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);

            EnemyBulletScript bulletScript = newBullet.GetComponent<EnemyBulletScript>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(directionToShoot, bulletSpeed);
            }

            StartCoroutine(SpreadBulletToLinePosition(newBullet, finalPositionOffset, directionToShoot));
        }
    }

    IEnumerator SpreadBulletToLinePosition(GameObject bullet, Vector3 finalPositionOffset, Vector3 forwardDirection)
    {
        float moveDuration = 0.2f; // Duration for the spread animation
        float elapsedTime = 0;

        Vector3 startPosition = bullet.transform.position;

        while (elapsedTime < moveDuration)
        {
            // Check if the bullet still exists
            if (bullet == null)
            {
                yield break; // Exit the coroutine if the bullet is destroyed
            }

            elapsedTime += Time.deltaTime;

            // Lerp the bullet's position toward the line formation offset
            Vector3 spreadPosition = Vector3.Lerp(Vector3.zero, finalPositionOffset, elapsedTime / moveDuration);

            // Add the forward movement to ensure the bullet moves toward the target
            bullet.transform.position = startPosition + spreadPosition + forwardDirection * (bulletSpeed * elapsedTime);

            yield return null;
        }

        // Final position adjustment (ensure the bullet is still valid)
        if (bullet != null)
        {
            bullet.transform.position = startPosition + finalPositionOffset + forwardDirection * (bulletSpeed * moveDuration);
        }
    }

    IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            ShootProjectiles();
            yield return new WaitForSeconds(burstCooldown);
        }
    }
}