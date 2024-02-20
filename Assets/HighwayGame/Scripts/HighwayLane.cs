using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayLane : MonoBehaviour
{
    [SerializeField, Tooltip("Notes in the MIDI that will be used to spawn gems")] private Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    [Tooltip("The keybind to activate the lane")] public KeyCode input;
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
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, HighwaySongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (HighwaySongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - HighwaySongManager.Instance.noteTime)
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
            double audioTime = HighwaySongManager.GetAudioSourceTime() - (HighwaySongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                NoteFeedback result = HighwayScoreManager.Instance.Hit(audioTime - timeStamp, transform.position - new Vector3(0, 0, 5));
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
            if (timeStamp + HighwaySongManager.Instance.goodRange <= audioTime)
            {
                inputIndex++;
            }
        }
    }
}
