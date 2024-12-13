using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Animator transitionAnim; // Assign in Inspector
    [SerializeField] private float transitionTime = 1f; // Duration of Crossfade_Start animation

    public static SceneController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (transitionAnim != null)
        {
            // Trigger Crossfade_Start animation
            transitionAnim.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime); // Wait for fade-out to finish
        }

        // Reset player's keys to 0
        if (GameManager.instance != null)
        {
            GameManager.instance.keys = 0;
            Debug.Log("Player's keys have been reset to 0.");
        }

        // Load the next scene
        SceneManager.LoadScene(sceneName);
    }
}
