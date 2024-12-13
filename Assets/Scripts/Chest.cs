using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add TextMeshPro namespace

public class Chest : Collectable
{
    public Sprite emptyChest;
    public int goldAmount = 5;
    public AudioClip goldSoundClip;

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            bool isWorldSpace = true;

            // Grant gold and show TextMeshPro text
            GameManager.instance.gold += goldAmount;

            GameManager.instance.ShowText("+" + goldAmount + " gold!", 25, Color.yellow, transform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);

            

            // Play sound effect
            AudioSource.PlayClipAtPoint(goldSoundClip, transform.position, 1f);
        }
    }
}