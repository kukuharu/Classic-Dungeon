using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;

    public AudioSource audioSource;
    public AudioClip[] typingSounds;
    public float typingSoundPitch = 1f;

    private Queue<string> sentences;
    private bool isTyping = false;
    public static bool isDialogueActive = false;

    // Add this method to reset dialogue state
    public void ResetDialogue()
    {
        isDialogueActive = false;
        // Add any additional reset logic here, such as clearing queued dialogues or resetting variables
        Debug.Log("Dialogue has been reset.");
    }

    private void Awake()
    {
        if (FindObjectsOfType<DialogueManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject); // Persist DialogueManager
    }


    void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
        isDialogueActive = false;

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetDialogueManager();
    }

    private void ResetDialogueManager()
    {
        StopAllCoroutines(); // Stop any ongoing typing animations
        sentences.Clear(); // Clear pending sentences
        isTyping = false;
        isDialogueActive = false;
        dialoguePanel.SetActive(false); // Hide the dialogue panel
        if (portraitImage != null)
        {
            portraitImage.enabled = false; // Disable portrait image
        }
    }

    public void StartDialogue(string[] dialogueLines, Sprite portrait = null)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        sentences.Clear();

        foreach (string line in dialogueLines)
        {
            sentences.Enqueue(line);
        }

        SetPortrait(portrait);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping) return;

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;

            if (typingSounds.Length > 0 && audioSource != null)
            {
                AudioClip randomClip = typingSounds[Random.Range(0, typingSounds.Length)];
                audioSource.pitch = typingSoundPitch;
                audioSource.PlayOneShot(randomClip);
            }

            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && isDialogueActive && !isTyping)
        {
            DisplayNextSentence();
        }
    }

    public void SetPortrait(Sprite newPortrait)
    {
        if (portraitImage != null)
        {
            portraitImage.sprite = newPortrait;
            portraitImage.enabled = newPortrait != null;
        }
    }
}
