using Melanchall.DryWetMidi.Interaction;
using System;
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
    [SerializeField, Tooltip("The sprite renderer of the shadow")] private SpriteRenderer shadowSprite;
    [SerializeField, Tooltip("Reference to the line renderer for displaying sustain notes")] private LineRenderer lineRenderer;
    [HideInInspector, Tooltip("If the sustain is being held")] public bool isSustaining = false;
    [Tooltip("The percentage of the sustain note that has been awarded score")] private double sustainPercentage = 0;
    [Tooltip("Cached material index of the intensity property")] private int _intensityID = Shader.PropertyToID("_Intensity");

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
        if (isSustaining)
        {
            UpdateSustain();
        }
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
        visualSprite.enabled = false;
        shadowSprite.enabled = false;
        CancelInvoke(nameof(OnMiss));
        if (lineRenderer != null)
        {
            StartCoroutine(SustainActivate());
        }
    }

    public void UpdateSustain()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, new Vector3(0, 0.01f, (-transform.position.z) - (-HighwayNoteManager.Instance.noteTapZ)));
        }
        double currentSustainPercentage = Math.Clamp((SongManager.Instance.GetAudioSourceTime() - assignedTime) / sustainDuration.TotalSeconds, 0f, 1f);
        double deltaSustainPercentage = currentSustainPercentage - sustainPercentage;
        sustainPercentage = currentSustainPercentage;
        ScoreManager.Instance.AddScore((long)(deltaSustainPercentage * 150 * sustainDuration.TotalSeconds));
        if (currentSustainPercentage == 1)
        {
            DeactivateSustain();
            isSustaining = false;
        }
    }

    public void DeactivateSustain()
    {
        if (lineRenderer != null)
        {
            StartCoroutine(SustainDeactivate());
        }
    }

    /// <summary>
    /// Fade in the sustain bar
    /// </summary>
    /// <returns>The coroutine</returns>
    private IEnumerator SustainActivate()
    {
        while (true)
        {
            if (lineRenderer == null || lineRenderer.material.GetFloat(_intensityID) >= 7.5f)
            {
                break;
            }
            lineRenderer.material.SetFloat(_intensityID, Mathf.Clamp(lineRenderer.material.GetFloat(_intensityID) + (Time.smoothDeltaTime * 10), 1, 7.5f));
            yield return null;
        }
    }

    /// <summary>
    /// Fade out the sustain bar
    /// </summary>
    /// <returns>The coroutine</returns>
    private IEnumerator SustainDeactivate()
    {
        while (true)
        {
            if (lineRenderer == null || lineRenderer.material.GetFloat(_intensityID) <= 0.5f)
            {
                break;
            }
            lineRenderer.material.SetFloat(_intensityID, Mathf.Clamp(lineRenderer.material.GetFloat(_intensityID) - (Time.smoothDeltaTime * 10), 0.5f, 1));
            yield return null;
        }
    }

    /// <summary>
    /// Called to destroy the note
    /// </summary>
    public void DeleteNote()
    {
        StopAllCoroutines();
        CancelInvoke();
        Destroy(gameObject);
    }
}
