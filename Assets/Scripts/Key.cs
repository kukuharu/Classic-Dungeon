using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Key : Collectable
{
    public int keyAmount = 1; // Number of keys this item provides
    public AudioClip keySoundClip; // Sound to play when the key is collected

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;

            // Add keys to GameManager and display floating text
            GameManager.instance.AddKey(keyAmount);
            GameManager.instance.ShowText("Found  Key!", 25, Color.cyan, transform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);

            // Play sound effect
            AudioSource.PlayClipAtPoint(keySoundClip, transform.position, 1f);

            // Destroy the key object
            Destroy(gameObject);
        }
    }
}
