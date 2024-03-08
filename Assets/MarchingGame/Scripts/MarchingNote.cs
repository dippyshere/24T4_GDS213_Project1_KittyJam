using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    private RectTransform rectTransform => GetComponent<RectTransform>();

    void Start()
    {
        timeInstantiated = MarchingSongManager.GetAudioSourceTime();
        rectTransform.localPosition = Vector3.right * MarchingSongManager.Instance.noteSpawnX;
        Invoke(nameof(OnMiss), (float)(MarchingSongManager.Instance.noteTime + MarchingSongManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = MarchingSongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (MarchingSongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            rectTransform.localPosition += Vector3.right * (MarchingSongManager.Instance.noteDespawnX - MarchingSongManager.Instance.noteSpawnX) * Time.deltaTime / (MarchingSongManager.Instance.noteTime * 2);
        }
    }

    public void OnMiss()
    {
        MarchingScoreManager.Instance.Miss(gameObject);
    }
}
