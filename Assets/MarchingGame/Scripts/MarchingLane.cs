using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;

/// <summary>
/// Handles the spawning of marhcing directional arrows for Game Type 3 (MarchingGame)
/// </summary>
public class MarchingLane : MonoBehaviour
{
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn notes"), Range(0, 127)] private int noteNumber;
    [SerializeField, Tooltip("The note prefab to spawn")] private GameObject notePrefab;
    [Tooltip("List of previous + current notes that have been spawned")] private List<MarchingNote> notes = new List<MarchingNote>();
    [Tooltip("List of all timestamps that notes will be spawned at")] private List<double> timeStamps = new List<double>();
    [Tooltip("The index of the currently spawned note")] private int spawnIndex = 0;

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
                notes.Add(note.GetComponent<MarchingNote>());
                note.GetComponent<MarchingNote>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }
    }
}
