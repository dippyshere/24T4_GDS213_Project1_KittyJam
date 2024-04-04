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
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        rectTransform.localPosition = Vector3.up * MarchingNoteManager.Instance.beatNoteSpawnY;
        Invoke(nameof(OnMiss), (float)(MarchingNoteManager.Instance.beatNoteTime + ScoreManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.Instance.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (MarchingNoteManager.Instance.beatNoteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            rectTransform.localPosition += Vector3.up * (MarchingNoteManager.Instance.beatNoteDespawnY - MarchingNoteManager.Instance.beatNoteSpawnY) * Time.deltaTime / (MarchingNoteManager.Instance.beatNoteTime * 2);
        }
    }

    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.parent.transform.position);
    }
}
