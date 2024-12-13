using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarthaSzabolcs.Tutorial_SpriteFlash;

public class Fighter : MonoBehaviour
{
    // Public fields
    public int hitpoint = 5;
    public int maxHitpoint = 5;
    public float pushRecoverySpeed = 0.2f;
    public AudioClip damageSoundClip;

    // New field to toggle pushback
    public bool canBePushed = true;

    // Immunity
    public float immuneTime = 0.4f;
    protected float lastImmune;

    // Push
    protected Vector3 pushDirection;

    // Flash effect
    private SimpleFlash simpleFlash; // Reference to the SimpleFlash component

    // Unity Callback
    protected virtual void Awake()
    {
        // Cache the SimpleFlash component
        simpleFlash = GetComponent<SimpleFlash>();
    }

    // All fighters can ReceiveDamage / Die
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;

            AudioSource.PlayClipAtPoint(damageSoundClip, transform.position, 2f); // Play damage sound

            // Apply push force only if canBePushed is true
            if (canBePushed)
            {
                pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            }

            // Show damage text
            GameManager.instance.ShowText(
                dmg.damageAmount.ToString(),
                25,
                Color.red,
                transform.position,
                Vector3.up * 0.3f,
                0.8f,
                true,
                0.0032f,
                arcFactor: 0.5f,
                sizeChangeRate: 0.0045f
            );

            // Trigger the flash effect
            if (simpleFlash != null)
            {
                simpleFlash.Flash(Color.white);
            }

            if (hitpoint <= 0)
            {
                hitpoint = 0;
                Death();
            }
        }
    }

    protected virtual void Death()
    {
        // Drop loot before destroying the enemy
        LootDropper lootDropper = GetComponent<LootDropper>();
        if (lootDropper != null)
        {
            lootDropper.DropLoot();
        }
    }
}
