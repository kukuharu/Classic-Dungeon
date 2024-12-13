using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    // Adjustable variables for damage and delay
    public int damage = 1;
    public float delay = 2f;

    // Animator reference
    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Start()
    {
        // Get the Animator and BoxCollider2D components
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Repeatedly trigger the trap's effect at the specified delay
        InvokeRepeating(nameof(TriggerTrap), 0f, delay);
    }

    private void TriggerTrap()
    {
        // Play the trap animation when it activates
        if (anim != null)
        {
            anim.SetTrigger("Activate"); // Ensure you have a trigger named "Activate" in your Animator
        }

        // Check for collisions within the trap's area using its BoxCollider2D
        Collider2D[] hits = new Collider2D[10]; // Array for storing results
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter(); // Match all layers, or customize if needed

        // Perform the overlap check using the BoxCollider2D's bounds
        int hitCount = boxCollider.Overlap(filter, hits);

        // Loop through the hits and apply damage to the player
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hits[i];

            // Check for the "player" tag (or change based on your player object setup)
            if (hit.name == "Player")
            {
                // Create a damage object with the defined damage values
                Damage dmg = new Damage
                {
                    damageAmount = damage,
                    origin = transform.position,
                    pushForce = 0 // No push force for a static trap
                };

                // Send the damage to the player
                hit.SendMessage("ReceiveDamage", dmg, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // Optional: Draw the collider size in the Scene view for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Draw the collider bounds for visualization
     //   if (boxCollider != null)
    //    {
     //       Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
     //   }
    }
}
