using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of the notes on the highway
/// </summary>
public class HighwayNote : MonoBehaviour
{
    [HideInInspector, Tooltip("The time that the note was instantiated at")] public double timeInstantiated;
    [HideInInspector, Tooltip("The time that the note needs to be hit")] public float assignedTime;
    [HideInInspector, Tooltip("The sustain note duration")] public MetricTimeSpan sustainDuration;
    [SerializeField, Tooltip("The sprite renderer of the note, to apply effects when missing the note")] private SpriteRenderer visualSprite;
    [SerializeField, Tooltip("Reference to the line renderer for displaying sustain notes")] private LineRenderer lineRenderer;

    void Start()
    {
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        transform.localPosition = Vector3.forward * HighwayNoteManager.Instance.noteSpawnZ;
        Invoke(nameof(OnMiss), (float)(SongManager.Instance.noteTime + ScoreManager.Instance.goodRange));
        if (sustainDuration != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(1, new Vector3(0, 0.02f, (float)(sustainDuration.TotalMicroseconds / 1000000f) / (SongManager.Instance.noteTime * 2) * ((HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * -1)));
            Invoke(nameof(DeleteNote), (float)(sustainDuration.TotalMicroseconds / 1000000f) + (float)(SongManager.Instance.noteTime * 2));
        }
        else
        {
            Invoke(nameof(DeleteNote), (float)(SongManager.Instance.noteTime * 2));
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * Time.smoothDeltaTime * Vector3.forward / (SongManager.Instance.noteTime * 2));
    }

    /// <summary>
    /// Called when the note is missed
    /// </summary>
    private void OnMiss()
    {
        DeactivateSustain();
        ScoreManager.Instance.Miss(transform.position);
        visualSprite.color = new Color(0.8f, 0.45f, 0.45f, 0.45f);
    }

    public void ActivateSustain()
    {
        if (lineRenderer != null)
        {
            lineRenderer.material.color = new Color(0.8f, 0.45f, 0.45f, 0.45f);
        }
    }

    public void UpdateSustain()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, new Vector3(0, 0.02f, (float)(SongManager.Instance.GetAudioSourceTime() - timeInstantiated) / (SongManager.Instance.noteTime * 2) * ((HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * -1)));
        }
    }

    private void DeactivateSustain()
    {
        if (lineRenderer != null)
        {
            lineRenderer.material.color = new Color(0.8f, 0.45f, 0.45f, 0.45f);
        }
    }

    /// <summary>
    /// Called to destroy the note
    /// </summary>
    private void DeleteNote()
    {
        Destroy(gameObject);
    }
}
