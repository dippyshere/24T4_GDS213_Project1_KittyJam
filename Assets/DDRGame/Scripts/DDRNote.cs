using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement and destruction of a single DDR note
/// </summary>
public class DDRNote : MonoBehaviour
{
    [HideInInspector, Tooltip("The time that the note was instantiated at")] public double timeInstantiated;
    [HideInInspector, Tooltip("The time that the note needs to be hit")] public float assignedTime;

    void Start()
    {
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        transform.localPosition += Vector3.back * DDRNoteManager.Instance.noteSpawnZ;
        Invoke(nameof(OnMiss), (float)(SongManager.Instance.noteTime + ScoreManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.Instance.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 0.5 + ScoreManager.Instance.goodRange)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(Vector3.back * (DDRNoteManager.Instance.noteDespawnZ - DDRNoteManager.Instance.noteSpawnZ) * Time.deltaTime / (SongManager.Instance.noteTime * 2));
        }
    }

    /// <summary>
    /// Called when the note is missed
    /// </summary>
    public void OnMiss()
    {
        ScoreManager.Instance.Miss(gameObject.transform.position);
    }
}
