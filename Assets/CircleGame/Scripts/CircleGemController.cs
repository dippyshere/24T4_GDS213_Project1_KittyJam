using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the gem that the player must hit
/// </summary>
public class CircleGemController : MonoBehaviour
{
    [SerializeField, Tooltip("List of potential sprites for the gem")] private Sprite[] sprites;
    [SerializeField, Tooltip("The sprite renderers for the gem")] private SpriteRenderer[] spriteRenderers;
    [HideInInspector, Tooltip("Time in the song that the gem was instantiated")] public double timeInstantiated;
    [HideInInspector, Tooltip("Time in the song the gem is assigned to")] public float assignedTime;

    // Start is called before the first frame update
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        // Randomly select a sprite from the list of potential sprites
        int spriteIndex = Random.Range(0, sprites.Length);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
        // Set the animation speed to match the song's note time
        GetComponent<Animator>().speed = 1 / SongManager.Instance.noteTime;
        // Automatically destroy the gem if it's not picked up in time
        Invoke("OnMiss", (float)(SongManager.Instance.noteTime * 1.15));
    }

    /// <summary>
    /// Called when the gem is touched by the player hand
    /// </summary>
    public void OnPickup()
    {
        double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);
        ScoreManager.Instance.Hit(audioTime - assignedTime, transform.position);
        CancelInvoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when the gem is not picked up in time
    /// </summary>
    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.position);
        Destroy(gameObject);
    }
}
