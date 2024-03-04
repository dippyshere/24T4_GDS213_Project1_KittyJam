using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDRLane : MonoBehaviour
{
    [SerializeField, Tooltip("Notes in the MIDI that will be used to spawn gems")] private Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    [Tooltip("The keybind to activate the lane")] public KeyCode primaryInput;
    [Tooltip("The secondary keybind to activate the lane")] public KeyCode secondaryInput;
    [SerializeField, Tooltip("The note prefab to spawn")] private GameObject notePrefab;
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
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, DDRSongManager.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (DDRSongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - DDRSongManager.Instance.noteTime)
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
            double audioTime = DDRSongManager.GetAudioSourceTime() - (DDRSongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(primaryInput) || Input.GetKeyDown(secondaryInput))
            {
                NoteFeedback result = DDRScoreManager.Instance.Hit(audioTime - timeStamp, transform.position - new Vector3(0, 0, 5));
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
            if (timeStamp + DDRSongManager.Instance.goodRange <= audioTime)
            {
                inputIndex++;
            }
        }       
    
    }
}
