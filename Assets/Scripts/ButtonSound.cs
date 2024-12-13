using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource gameAudio;
    private void playButton()
    {
        gameAudio.Play();
    }

    // This script is currently in the Weapon Container upgrade button on the Character Menu

}
