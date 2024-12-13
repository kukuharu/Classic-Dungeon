using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLocked : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isOpen = false;       // Tracks if the door is open
    public bool isLocked = true;     // Tracks if the door is locked
    private Animator animator;       // Animator reference

    private float cooldown = 1.0f;
    private float lastShout = -1;    // Cooldown for "Locked" feedback

    [Header("Key Settings")]
    public int requiredKeys = 1;     // Keys required to unlock the door

    public AudioClip unlockClip;  // new line
    public AudioClip openClip;  // new line


    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
    }

    public void Interact()
    {
        if (isLocked)
        {
            TryUnlockDoor();
        }
        else if (!isOpen)
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("Door is already open!");
        }
    }

    private void TryUnlockDoor()
    {
        // Check if the player has enough keys
        if (GameManager.instance.keys >= requiredKeys)
        {
            GameManager.instance.keys -= requiredKeys; // Deduct keys
            Debug.Log($"Door unlocked! Remaining keys: {GameManager.instance.keys}");
            isLocked = false;                         // Unlock the door
            TriggerUnlockAnimation();                // Trigger unlocking animation
        }
        else
        {
            // Check if the cooldown has passed
            if (Time.time - lastShout > cooldown)
            {
                lastShout = Time.time;
                GameManager.instance.ShowText(
                    "Locked...",
                    25,
                    Color.white,
                    transform.position + new Vector3(0, 0.05f, 0), // Position for the text
                    Vector3.zero,
                    cooldown,
                    true,
                    0.0032f // Optional scale parameter
                );
            }

            Debug.Log("Not enough keys to unlock the door!");
        }
    }

    private void TriggerUnlockAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("DoorUnlocked", true); // Trigger unlocking animation
        }
        
        AudioSource.PlayClipAtPoint(unlockClip, transform.position, 1.0f);

        Debug.Log("Door unlocked visually.");
    }


    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Opening door...");
        if (animator != null)
        {
            animator.SetBool("DoorOpen", true); // Trigger door opening animation
        }
        AudioSource.PlayClipAtPoint(openClip, transform.position, 1.0f);
    }
}
