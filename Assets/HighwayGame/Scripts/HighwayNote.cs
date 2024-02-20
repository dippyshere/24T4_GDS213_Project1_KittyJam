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
        transform.localPosition = Vector3.forward * HighwaySongManager.Instance.noteSpawnY;
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
            //transform.localPosition = Vector3.Lerp(Vector3.forward * HighwaySongManager.Instance.noteSpawnY, Vector3.forward * HighwaySongManager.Instance.noteDespawnY, t);
            transform.Translate(Vector3.forward * (HighwaySongManager.Instance.noteDespawnY - HighwaySongManager.Instance.noteSpawnY) * Time.deltaTime / (HighwaySongManager.Instance.noteTime * 2));
        }
    }
}
