﻿using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingBeat : MonoBehaviour
{
    [Tooltip("The keybind to activate the lane")] public KeyCode input;
    [SerializeField, Tooltip("The note prefab to spawn")] private GameObject notePrefab;
    [Tooltip("List of previous + current notes that have been spawned")] private List<MarchingBeatNote> notes = new List<MarchingBeatNote>();
    [Tooltip("List of all timestamps that notes will be spawned at")] private List<double> timeStamps = new List<double>();
    [Tooltip("The index of the currently spawned note")] private int spawnIndex = 0;
    [Tooltip("The index of the currently pending input")] private int inputIndex = 0;

    /// <summary>
    /// Set the timestamps for the notes to be spawned at based on the MIDI file and the note restriction
    /// </summary>
    /// <param name="array">The array of notes from the MIDI file</param>
    public void SetTimeStamps(List<double> timestampList)
    {
        timeStamps = timestampList;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (MarchingSongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - MarchingSongManager.Instance.beatNoteTime)
            {
                var note = Instantiate(notePrefab, transform);
                notes.Add(note.GetComponent<MarchingBeatNote>());
                note.GetComponent<MarchingBeatNote>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double audioTime = MarchingSongManager.GetAudioSourceTime() - (MarchingSongManager.Instance.inputDelayInMilliseconds / 1000.0);

            if (Input.GetKeyDown(input))
            {
                NoteFeedback result = MarchingScoreManager.Instance.Hit(audioTime - timeStamp, gameObject);
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
            if (timeStamp + MarchingSongManager.Instance.goodRange <= audioTime)
            {
                inputIndex++;
            }
        }
    }
}