﻿using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the spawning of notes based on the MIDI file for Game Type 2 (HighwayGame)
/// </summary>
public class HighwayLane : MonoBehaviour
{
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn notes"), Range(0, 127)] private int noteNumber;
    [SerializeField, Tooltip("The note prefab to spawn")] private GameObject notePrefab;
    [Tooltip("List of previous + current notes that have been spawned")] private List<HighwayNote> notes = new List<HighwayNote>();
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
                notes.Add(note.GetComponent<HighwayNote>());
                note.GetComponent<HighwayNote>().assignedTime = (float)timeStamps[spawnIndex];
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
#if UNITY_IOS
                try
                {
                    Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
                }
                catch (Exception)
                {

                }
#elif UNITY_ANDROID
                Vibration.VibratePop();
#endif
                Destroy(notes[inputIndex].gameObject);
                inputIndex++;
            }
            if (result == NoteFeedback.Miss)
            {
                inputIndex++;
            }
        }
    }
}
