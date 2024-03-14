using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    void Start()
    {
        timeInstantiated = MarchingSongManager.GetAudioSourceTime();
        Invoke(nameof(OnMiss), (float)(MarchingSongManager.Instance.noteTime + MarchingSongManager.Instance.goodRange));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            double audioTime = MarchingSongManager.GetAudioSourceTime() - (MarchingSongManager.Instance.inputDelayInMilliseconds / 1000.0);
            MarchingScoreManager.Instance.Hit(audioTime - assignedTime, gameObject, false);
            Destroy(gameObject);
        }
    }

    public void OnMiss()
    {
        MarchingScoreManager.Instance.Miss(gameObject, isBeatNote: false);
        Destroy(gameObject);
    }
}
