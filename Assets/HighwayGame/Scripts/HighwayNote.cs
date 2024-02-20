using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;

    void Start()
    {
        timeInstantiated = HighwaySongManager.GetAudioSourceTime();
        transform.localPosition = Vector3.forward * HighwaySongManager.Instance.noteSpawnZ;
        Invoke(nameof(OnMiss), (float)(HighwaySongManager.Instance.noteTime + HighwaySongManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = HighwaySongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (HighwaySongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(Vector3.forward * (HighwaySongManager.Instance.noteDespawnZ - HighwaySongManager.Instance.noteSpawnZ) * Time.deltaTime / (HighwaySongManager.Instance.noteTime * 2));
        }
    }

    public void OnMiss()
    {
        HighwayScoreManager.Instance.Miss(transform.position);
    }
}
