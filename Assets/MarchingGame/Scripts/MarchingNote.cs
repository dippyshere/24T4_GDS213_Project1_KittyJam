using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    void Start()
    {
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        Invoke(nameof(OnMiss), (float)(SongManager.Instance.noteTime + ScoreManager.Instance.goodRange));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            ScoreManager.Instance.Hit(0, transform.position, true);
            Destroy(gameObject);
        }
    }

    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.parent.transform.position, isAlternateNote: true);
        Destroy(gameObject);
    }
}
