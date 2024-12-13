using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isOpen = false; // Tracks door state
    private Animator animator;  // Animator reference
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
        if (!isOpen)
        {
            isOpen = true;
            Debug.Log("Opening door...");
            animator.SetBool("DoorOpen", true);
            AudioSource.PlayClipAtPoint(openClip, transform.position, 1.0f);
        }
        else
        {
            Debug.Log("Door is already open!");
        }
    }
}
