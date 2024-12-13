using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Mover
{
    public int xpValue = 1;
    public GameObject deathEffect;
    private Transform playerTransform;
   

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        canBePushed = false;

    }

    void Update()
    {
        // Flip the sprite to face the player
        Vector3 direction = playerTransform.position - transform.position;

        // Check if the enemy should be flipped
        if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
        {
            // Flip the sprite by inverting the X scale
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        UpdateMotor(Vector3.zero);
    }

    protected override void Death()
    {
        Destroy(gameObject);
        Instantiate(deathEffect, transform.position, transform.rotation);
        GameManager.instance.GrantXp(xpValue);
        //        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, playerTransform.position, Vector3.up * 40, 1.0f);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, playerTransform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);
    }
}
