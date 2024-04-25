using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the beat notes
/// </summary>
public class MarchingBeatNote : MonoBehaviour
{
    [HideInInspector, Tooltip("The time that the note was instantiated at")] public double timeInstantiated;
    [HideInInspector, Tooltip("The time that the note needs to be hit")] public float assignedTime;
    [Tooltip("Reference to the rect transform of the beat note")] private RectTransform rectTransform => GetComponent<RectTransform>();

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
            rectTransform.localPosition += (MarchingNoteManager.Instance.beatNoteDespawnY - MarchingNoteManager.Instance.beatNoteSpawnY) * Time.smoothDeltaTime * Vector3.up / (MarchingNoteManager.Instance.beatNoteTime * 2);
        }
    }

    /// <summary>
    /// Called when the note is missed
    /// </summary>
    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.parent.transform.position);
    }
}
