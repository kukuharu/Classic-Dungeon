using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : Collectable
{
    public int healingAmount = 1;
    public AudioClip healPotionSound;
    public GameObject sparkleEffect;
    protected override void OnCollect()
        
    {
        if (GameManager.instance.player.hitpoint == GameManager.instance.player.maxHitpoint)
            return;
        else
        {
            GameManager.instance.player.Heal(healingAmount);
            AudioSource.PlayClipAtPoint(healPotionSound, transform.position, 1f);
            collected = true;
            Instantiate(sparkleEffect, transform.position, transform.rotation);  // Sparkle efect plays.
            Destroy(gameObject);                   

        }
    }
}
