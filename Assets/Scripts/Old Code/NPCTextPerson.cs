using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTextPerson : Collidable
{
    public string message;

    private float cooldown = 4.0f;
    private float lastShout = -4;   // So he can instantly speak at start

    protected override void OnCollide(Collider2D coll)
    {
        // Check if the collider is tagged as "Fighter" and named "Player"
        if (coll.CompareTag("Fighter") && coll.name == "Player")
        {
            if (Time.time - lastShout > cooldown)
            {
                lastShout = Time.time;
                bool isWorldSpace = true;
                GameManager.instance.ShowText(message, 25, Color.white, transform.position + new Vector3(0, 0.14f, 0), Vector3.zero, cooldown, true, 0.0032f); // Optional scale parameter

            }
        }
    }
}