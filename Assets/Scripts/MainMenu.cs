using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ResetGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetCharacter();
        }
        else
        {
            Debug.LogError("GameManager instance not found.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}