using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collidable : MonoBehaviour
{
    public ContactFilter2D filter;
    private new Collider2D collider2D; // Can be either BoxCollider2D or CapsuleCollider2D
    protected Collider2D[] hits = new Collider2D[10];

    protected virtual void Start()
    {
        // Try to get a BoxCollider2D or CapsuleCollider2D
        collider2D = GetComponent<BoxCollider2D>();
        if (collider2D == null)
        {
            collider2D = GetComponent<CapsuleCollider2D>();
        }

        // Check if no collider was found
        if (collider2D == null)
        {
            Debug.LogWarning("No BoxCollider2D or CapsuleCollider2D found on " + gameObject.name);
        }
    }

    protected virtual void Update()
    {
        // Ensure collider is set
        if (collider2D == null)
            return;

        // Perform collision work using whichever collider is found
        collider2D.Overlap(filter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            OnCollide(hits[i]);

            // Clear out the hit entry
            hits[i] = null;
        }
    }

    protected virtual void OnCollide(Collider2D coll)
    {
        Debug.Log("OnCollide was not implemented in " + this.name);
    }
}