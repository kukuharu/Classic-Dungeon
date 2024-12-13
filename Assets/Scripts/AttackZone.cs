using UnityEngine;

public class AttackZone : Collidable
{
    private AttackingEnemy enemy;

    protected override void Start()
    {
        base.Start();
        // Get reference to parent enemy script
        enemy = GetComponentInParent<AttackingEnemy>();
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            if (!enemy.PlayerInAttackZone && !enemy.IsAttacking) // Use public IsAttacking property
            {
                // Notify enemy that player is within attack range only if not already attacking
                enemy.PlayerInAttackZone = true;
          //      Debug.Log("AttackZone=True: Player entered attack zone.");
            }
        }
    }
}