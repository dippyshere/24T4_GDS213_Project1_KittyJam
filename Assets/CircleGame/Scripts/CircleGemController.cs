using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGemController : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer[] spriteRenderers;
    double timeInstantiated;
    public float assignedTime;

    // Start is called before the first frame update
    void Start()
    {
        timeInstantiated = SongManager.GetAudioSourceTime();
        int spriteIndex = Random.Range(0, sprites.Length);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    public void OnPickup()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));
        double marginOfError = SongManager.Instance.marginOfError;
        double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);
        Debug.Log("Time since instantiated: " + Mathf.Abs((float)audioTime - assignedTime));
        Destroy(gameObject);
    }
}
