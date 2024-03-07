using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingBeatNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    private RectTransform rectTransform => GetComponent<RectTransform>();

    void Start()
    {
        timeInstantiated = MarchingSongManager.GetAudioSourceTime();
        rectTransform.localPosition = Vector3.up * MarchingSongManager.Instance.beatNoteSpawnY;
        Invoke(nameof(OnMiss), (float)(MarchingSongManager.Instance.beatNoteTime + MarchingSongManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = MarchingSongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (MarchingSongManager.Instance.beatNoteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            rectTransform.localPosition += Vector3.up * (MarchingSongManager.Instance.beatNoteDespawnY - MarchingSongManager.Instance.beatNoteSpawnY) * Time.deltaTime / (MarchingSongManager.Instance.beatNoteTime * 2);
        }
    }

    public void OnMiss()
    {
        MarchingScoreManager.Instance.Miss(gameObject);
    }
}
