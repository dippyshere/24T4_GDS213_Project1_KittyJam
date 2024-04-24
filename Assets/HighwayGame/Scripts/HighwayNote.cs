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
    [SerializeField, Tooltip("The sprite renderer of the note, to apply effects when missing the note")] private SpriteRenderer visualSprite;

    void Start()
    {
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        transform.localPosition = Vector3.forward * HighwayNoteManager.Instance.noteSpawnZ;
        Invoke(nameof(OnMiss), (float)(SongManager.Instance.noteTime + ScoreManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.Instance.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate((HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * Time.deltaTime * Vector3.forward / (SongManager.Instance.noteTime * 2));
        }
    }

    /// <summary>
    /// Called when the note is missed
    /// </summary>
    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.position);
        visualSprite.color = new Color(0.8f, 0.45f, 0.45f, 0.45f);
    }
}
