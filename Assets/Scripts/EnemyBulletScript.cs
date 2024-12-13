using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : Collidable
{
    private Vector3 moveDirection;
    public float force;
    public int damage = 1;
    public float pushForce = 0;
    private float decay;
    public float duration = 2;

    public GameObject impactEffect;

    public void Initialize(Vector3 direction, float speed)
    {
        moveDirection = direction;
        force = speed;
    }

    private void FixedUpdate()
    {
        // Move the bullet in the calculated direction
        transform.position += moveDirection * force * Time.fixedDeltaTime;
        decay += Time.deltaTime;
        if (decay > duration)
        {
            Destroy(gameObject);
        }
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Enemy" || coll.tag == "Projectile" || coll.name == "Hitbox" || coll.tag == "Trap" || coll.tag == "Interactable")
        {
            return;
        }

        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };
            coll.SendMessage("ReceiveDamage", dmg);
        }

        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
