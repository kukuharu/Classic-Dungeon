using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    public string message;

    private float cooldown = 4.0f;
    private float lastShout = -4; // So the NPC can instantly speak at start

    public void Interact()
    {
        // Check if the cooldown has passed
        if (Time.time - lastShout > cooldown)
        {
            lastShout = Time.time;
            GameManager.instance.ShowText(
                message,
                25,
                Color.white,
                transform.position + new Vector3(0, 0.14f, 0), // Position for the text
                Vector3.zero,
                cooldown,
                true,
                0.0032f // Optional scale parameter
            );
        }
    }
}