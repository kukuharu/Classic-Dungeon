using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueWindow : MonoBehaviour, IInteractable
{
    public string[] dialogueLines;
    public Sprite npcPortrait;
    public float typingSoundPitch = 1f; // Individual pitch setting for this NPC

    private bool canInteract = true; // Prevent interaction when dialogue is active

    public void Interact()
    {
        if (!canInteract || DialogueManager.isDialogueActive) return; // Prevent interaction when dialogue is active

        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found in the scene!");
            return;
        }
        if (DialogueManager.isDialogueActive) return;

        // Pass the npcPortrait and typingSoundPitch to the DialogueManager
        dialogueManager.typingSoundPitch = typingSoundPitch; // Set the pitch dynamically
        dialogueManager.StartDialogue(dialogueLines, npcPortrait);
        canInteract = false; // Disable interaction while dialogue is active
    }

    void Update()
    {
        // Re-enable interaction if dialogue has ended
        if (!DialogueManager.isDialogueActive && !canInteract)
        {
            canInteract = true;
        }
    }
}
