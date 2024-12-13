using UnityEngine;

public class DetectZone : Collidable
{
    private MovingHalt enemy; // Reference to the parent enemy script

    protected override void Start()
    {
        base.Start();
        // Get reference to the parent enemy script
        enemy = GetComponentInParent<MovingHalt>();
        if (enemy == null)
        {
            Debug.LogError("MovingHalt script not found on the parent object!");
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.CompareTag("Fighter") && coll.name == "Player")
        {
            Debug.Log("Player detected in DetectZone.");
            enemy.HaltMovement(); // Trigger halt movement on the enemy
        }
    }
}
