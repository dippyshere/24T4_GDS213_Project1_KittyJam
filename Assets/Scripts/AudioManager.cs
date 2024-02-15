using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    private float fadeDuration = 1.5f; // Duration for fading in/out in seconds

    private bool isFading = false;

    void Awake()
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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (!isFading)
            StartCoroutine(FadeOutMusic());
    }

    public void PlayMusic()
    {
        if (!isFading)
            StartCoroutine(FadeInMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        isFading = true;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        isFading = false;
    }

    private IEnumerator FadeInMusic()
    {
        isFading = true;
        audioSource.Play();
        audioSource.volume = 0f;

        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 1f;
        isFading = false;
    }
}
