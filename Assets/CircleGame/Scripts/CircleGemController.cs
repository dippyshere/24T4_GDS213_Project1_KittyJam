using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
        GetComponent<Animator>().speed = 1 / SongManager.Instance.noteTime;
        Invoke("OnMiss", (float)(SongManager.Instance.noteTime * 1.15));
    }

    public void OnPickup()
    {
        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));
        double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);
        ScoreManager.Instance.Hit(Mathf.Abs((float)audioTime - assignedTime), transform.position);
        CancelInvoke();
        Destroy(gameObject);
    }

    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.position);
        Destroy(gameObject);
    }
}
