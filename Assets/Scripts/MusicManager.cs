using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName; // Name of the scene
        public List<AudioClip> musicTracks; // List of tracks for the scene
    }

    public List<SceneMusic> sceneMusicList; // List of scene-specific music
    public AudioSource audioSource; // AudioSource component for playing music
    public float crossfadeDuration = 1.0f; // Duration of the crossfade transition
    public float defaultVolume = 0.3f; // Default volume level (adjust in Inspector)

    private string currentScene; // Keeps track of the current scene
    private Coroutine crossfadeCoroutine; // Handles crossfade transitions

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

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // Ensure music loops
        }

        audioSource.volume = defaultVolume; // Set the initial volume

        SceneManager.sceneLoaded += OnSceneLoaded;
        currentScene = SceneManager.GetActiveScene().name;
        PlaySceneMusic(currentScene);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentScene)
        {
            currentScene = scene.name;
            PlaySceneMusic(currentScene);
        }
    }

    public void PlaySceneMusic(string sceneName)
    {
        SceneMusic sceneMusic = sceneMusicList.Find(sm => sm.sceneName == sceneName);

        if (sceneMusic != null && sceneMusic.musicTracks.Count > 0)
        {
            AudioClip selectedTrack = sceneMusic.musicTracks[Random.Range(0, sceneMusic.musicTracks.Count)];

            if (crossfadeCoroutine != null)
                StopCoroutine(crossfadeCoroutine);

            crossfadeCoroutine = StartCoroutine(CrossfadeMusic(selectedTrack));
        }
        else
        {
            Debug.LogWarning($"No music tracks found for scene '{sceneName}'!");
        }
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        if (audioSource.isPlaying)
        {
            // Fade out
            float startVolume = audioSource.volume;
            for (float t = 0; t < crossfadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / crossfadeDuration);
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = startVolume;
        }

        // Switch to the new track and fade in
        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < crossfadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, defaultVolume, t / crossfadeDuration);
            yield return null;
        }
        audioSource.volume = defaultVolume;
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    public void StopMusic()
    {
        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        audioSource.Stop();
    }
}