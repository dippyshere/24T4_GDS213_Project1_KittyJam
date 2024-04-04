using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lane manager for Game Type 4 (DDR)
/// </summary>
public class DDRLane : MonoBehaviour
{
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn notes"), Range(0, 127)] private int noteNumber;
    [SerializeField, Tooltip("The note prefab to spawn")] private GameObject notePrefab;
    [SerializeField, Tooltip("The cat arm to enable when the lane is active")] private BongoCatArm catArm;
    [Tooltip("List of previous + current notes that have been spawned")] private List<DDRNote> notes = new List<DDRNote>();
    [Tooltip("List of all timestamps that notes will be spawned at")] private List<double> timeStamps = new List<double>();
    [Tooltip("The index of the currently spawned note")] private int spawnIndex = 0;
    [Tooltip("The index of the currently pending input")] private int inputIndex = 0;


    /// <summary>
    /// Set the timestamps for the notes to be spawned at based on the MIDI file and the note restriction
    /// </summary>
    /// <param name="array">The array of notes from the MIDI file</param>
    public void SetTimeStamps(Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteNumber == noteNumber)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.Instance.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<DDRNote>());
                note.GetComponent<DDRNote>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double audioTime = SongManager.Instance.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (timeStamp + ScoreManager.Instance.goodRange <= audioTime)
            {
                inputIndex++;
            }
        }
    }

    /// <summary>
    /// Hits the note
    /// </summary>
    public void Hit()
    {
        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double audioTime = SongManager.Instance.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);
            NoteFeedback result = ScoreManager.Instance.Hit(audioTime - timeStamp, transform.position - new Vector3(0, 0, 5));
            Debug.Log(result);
            if (result == NoteFeedback.Good || result == NoteFeedback.Perfect)
            {
                Destroy(notes[inputIndex].gameObject);
                inputIndex++;
            }
            if (result == NoteFeedback.Miss)
            {
                inputIndex++;
            }
        }
    }

    /// <summary>
    /// Makes the cat arm go down
    /// </summary>
    public void ArmDown()
    {
        if (BongoCatController.Instance != null)
            BongoCatController.Instance.EnableArm(catArm);
    }

    /// <summary>
    /// Makes the cat arm go up
    /// </summary>
    public void ArmUp()
    {
        if (BongoCatController.Instance != null)
            BongoCatController.Instance.DisableArm(catArm);
    }
}
