using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDRNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    void Start()
    {
        timeInstantiated = DDRSongManager.GetAudioSourceTime();
        transform.localPosition += Vector3.back * DDRSongManager.Instance.noteSpawnZ;
        Invoke(nameof(OnMiss), (float)(DDRSongManager.Instance.noteTime + DDRSongManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = DDRSongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (DDRSongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(Vector3.back * (DDRSongManager.Instance.noteDespawnZ - DDRSongManager.Instance.noteSpawnZ) * Time.deltaTime / (DDRSongManager.Instance.noteTime * 2));
        }
    }

    public void OnMiss()
    {
        DDRScoreManager.Instance.Miss(gameObject.transform.position);
    }
}
