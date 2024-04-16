using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the menu music for the game
/// </summary>
public class AudioManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the audio manager")] public static AudioManager instance;

    [SerializeField, Tooltip("The background music to play on loop")] private AudioClip backgroundMusic;
    [SerializeField, Tooltip("The audio source to play music from")] private AudioSource audioSource;
    [SerializeField, Tooltip("How long to fade in/out the music (in seconds)")] private float fadeDuration = 1.5f;
    
    [Tooltip("Boolean for whether the music is already fading in/out")] private bool isFading = false;

    void Awake()
    {
        // Ensure that only one instance of the audio manager exists
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

    void Start()
    {
        // Play the background music on loop
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    /// <summary>
    /// Stops the background music and fades it out
    /// </summary>
    public void StopMusic()
    {
        if (!isFading)
        {
            StopCoroutine(FadeOutMusic());
            StartCoroutine(FadeOutMusic());
        }
    }

    /// <summary>
    /// Plays the background music and fades it in
    /// </summary>
    public void PlayMusic()
    {
        if (!isFading)
        {
            StopCoroutine(FadeInMusic());
            StartCoroutine(FadeInMusic());
        }
    }

    /// <summary>
    /// Fades out the music
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator FadeOutMusic()
    {
        isFading = true;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.unscaledDeltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        isFading = false;
    }

    /// <summary>
    /// Fades in the music
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator FadeInMusic()
    {
        isFading = true;
        audioSource.Play();
        audioSource.volume = 0f;

        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.unscaledDeltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 1f;
        isFading = false;
    }
}
