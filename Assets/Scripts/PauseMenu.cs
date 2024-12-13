using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public Animator characterMenuAnimator; // Reference to the Animator of the character menu
    private bool isCharacterMenuOpen = false; // Tracks if the character menu is open

    private void Start()
    {
        ResetPauseState();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetPauseState();
    }

    private void ResetPauseState()
    {
        Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the character menu is open, close it first
            if (isCharacterMenuOpen)
            {
                CloseCharacterMenu();
                return;
            }

            // Otherwise, toggle pause menu
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.volume = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Exposed method to close the character menu
    public void CloseCharacterMenu()
    {
        if (characterMenuAnimator != null)
        {
            characterMenuAnimator.SetTrigger("Hide");
            isCharacterMenuOpen = false;
        }
    }

    // Exposed method to open the character menu
    public void OpenCharacterMenu()
    {
        if (characterMenuAnimator != null)
        {
            characterMenuAnimator.SetTrigger("Show");
            isCharacterMenuOpen = true;
        }
    }
}
