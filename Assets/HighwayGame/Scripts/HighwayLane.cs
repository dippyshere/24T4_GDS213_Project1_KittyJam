using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the spawning of notes based on the MIDI file for Game Type 2 (HighwayGame)
/// </summary>
public class HighwayLane : MonoBehaviour
{
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn notes"), Range(0, 127)] private int noteNumber;
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn lift notes"), Range(0, 127)] private int liftNoteNumber;
    [SerializeField, Tooltip("The note prefab to spawn")] private GameObject notePrefab;
    [SerializeField, Tooltip("The note prefab to spawn for lift notes")] private GameObject liftNotePrefab;
    [SerializeField, Tooltip("Reference to the note smasher renderer")] private SpriteRenderer noteSmasher;
    [Tooltip("List of previous + current notes that have been spawned")] private List<HighwayNote> notes = new List<HighwayNote>();
    [Tooltip("List of all timestamps that notes will be spawned at")] private List<Dictionary<double, List<bool>>> timeStamps = new List<Dictionary<double, List<bool>>>();
    [Tooltip("List of sustain note durations")] private List<MetricTimeSpan> sustainDurations = new List<MetricTimeSpan>();
    [Tooltip("The index of the currently spawned note")] private int spawnIndex = 0;
    [Tooltip("The index of the currently pending input")] private int inputIndex = 0;
    [Tooltip("Reference to the current sustain note")] private HighwayNote currentSustainNote;

    private void Start()
    {
        noteSmasher.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
    }

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
                if (note.LengthAs<MetricTimeSpan>(SongManager.midiFile.GetTempoMap()).TotalMicroseconds > 175000)
                {
                    timeStamps.Add(new Dictionary<double, List<bool>> { { (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f, new List<bool> { false, true, false } } });
                    sustainDurations.Add(note.LengthAs<MetricTimeSpan>(SongManager.midiFile.GetTempoMap()));
                }
                else
                {
                    timeStamps.Add(new Dictionary<double, List<bool>> { { (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f, new List<bool> { false, false, false } } });
                }
            }
            if (note.NoteNumber == liftNoteNumber)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                timeStamps.Add(new Dictionary<double, List<bool>> { { (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f, new List<bool> { true, false, false } } });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.Instance.GetAudioSourceTime() >= timeStamps[spawnIndex].Keys.First() - SongManager.Instance.noteTime)
            {
                if (timeStamps[spawnIndex].Values.First()[0])
                {
                    GameObject liftNote = Instantiate(liftNotePrefab, transform);
                    notes.Add(liftNote.GetComponent<HighwayNote>());
                    liftNote.GetComponent<HighwayNote>().assignedTime = (float)timeStamps[spawnIndex].Keys.First();
                    spawnIndex++;
                }
                else
                {
                    GameObject note = Instantiate(notePrefab, transform);
                    notes.Add(note.GetComponent<HighwayNote>());
                    note.GetComponent<HighwayNote>().assignedTime = (float)timeStamps[spawnIndex].Keys.First();
                    if (timeStamps[spawnIndex].Values.First()[1])
                    {
                        note.GetComponent<HighwayNote>().sustainDuration = sustainDurations[0];
                        sustainDurations.RemoveAt(0);
                    }
                    spawnIndex++;
                }
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex].Keys.First();
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
    public void Hit(InputAction.CallbackContext context)
    {
        if (inputIndex < timeStamps.Count)
        {
            if (context.canceled && currentSustainNote != null)
            {
                currentSustainNote.DeactivateSustain();
                currentSustainNote.isSustaining = false;
                currentSustainNote = null;
            }
            bool liftHit = false;
            if (context.canceled && timeStamps[inputIndex].Values.First()[0])
            {
                liftHit = true;
            }
            else if (context.started)
            {
                liftHit = false;
            }
            else
            {
                return;
            }
            double timeStamp = timeStamps[inputIndex].Keys.First();
            double audioTime = SongManager.Instance.GetAudioSourceTime() - SongManager.Instance.inputOffset;
            double inputOffset = Time.realtimeSinceStartupAsDouble - context.startTime;
            NoteFeedback result = ScoreManager.Instance.Hit(audioTime - timeStamp - inputOffset, transform.position - new Vector3(0, 0, 5), isLiftNote: liftHit);
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
                if (timeStamps[inputIndex].Values.First()[1])
                {
                    notes[inputIndex].ActivateSustain();
                    notes[inputIndex].isSustaining = true;
                    currentSustainNote = notes[inputIndex];
                }
                else
                {
                    notes[inputIndex].DeleteNote();
                }
                inputIndex++;
            }
            if (result == NoteFeedback.Miss)
            {
                inputIndex++;
            }
        }
    }
}
