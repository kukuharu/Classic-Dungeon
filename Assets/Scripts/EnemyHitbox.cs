using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Collidable
{
    // Damage
    // This script will cause contact damage to the player. It also bounces against them.
    

    public int damage = 1;
    public float pushForce = 5;

    protected override void OnCollide(Collider2D coll)

    {
        if(coll.tag == "Fighter" && coll.name == "Player")
        {
            //Create a new damage object, before sending it to the player
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }

}
