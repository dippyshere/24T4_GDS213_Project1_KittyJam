using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the spawning of beat notes
/// </summary>
public class MarchingBeat : MonoBehaviour
{
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
            if (SongManager.Instance.GetAudioSourceTime() >= timeStamps[spawnIndex] - MarchingNoteManager.Instance.beatNoteTime)
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
            double audioTime = SongManager.Instance.GetAudioSourceTime() - SongManager.Instance.inputOffset;

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
            double audioTime = SongManager.Instance.GetAudioSourceTime() - SongManager.Instance.inputOffset;
            NoteFeedback result = ScoreManager.Instance.Hit(audioTime - timeStamp, transform.position);
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
