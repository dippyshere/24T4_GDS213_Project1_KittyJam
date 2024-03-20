using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of notes based on the MIDI file for Game Type 1 (CircleGame)
/// </summary>
public class NoteManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the score manager")] public static NoteManager Instance;
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn notes"), Range(0, 127)] private int noteNumber;
    [SerializeField, Tooltip("The gem prefab to spawn for notes")] private GameObject notePrefab;
    [SerializeField, Tooltip("The FollowPoint prefab to spawn")] private GameObject followPoint;
    [SerializeField, Tooltip("Reference to the score manager")] private ScoreManager scoreManager;
    [Tooltip("List of previous + current notes that have been spawned")] private List<CircleGemController> notes = new List<CircleGemController>();
    [SerializeField, Tooltip("List of locations to manually place a specific note (Leave at 0,0,0 for random; Ensure Z is always at 4.89)")] private List<Vector3> spawnLocations = new List<Vector3>();
    [Tooltip("List of all timestamps that notes will be spawned at")] private List<double> timeStamps = new List<double>();
    [SerializeField, Tooltip("Top left position of the random spawn area")] private Vector3 spawnAreaTopLeft;
    [SerializeField, Tooltip("Bottom right position of the random spawn area")] private Vector3 spawnAreaBottomRight;

    [Tooltip("The index of the currently spawned note")] private int spawnIndex = 0;

    private void Start()
    {
        Instance = this;
        followPoint.GetComponent<TrailRenderer>().enabled = false; 
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
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.Instance.noteTime)
            {
                var note = Instantiate(notePrefab, SpawnLocation(), Quaternion.identity);
                notes.Add(note.GetComponent<CircleGemController>());
                notes[spawnIndex].assignedTime = (float)timeStamps[spawnIndex];

                //if it has been a couple of beats since a new note spawns, reset the trail path
                if (spawnIndex > 0)
                {
                    if (timeStamps[spawnIndex] - timeStamps[spawnIndex - 1] >= 1.2)
                    {
                        followPoint.GetComponent<TrailRenderer>().enabled = false;
                        Debug.Log("Cut Trail");
                    }
                }
              

                followPoint.transform.position = notes[spawnIndex].transform.position;
                followPoint.GetComponent<TrailRenderer>().enabled = true;
                spawnIndex++;
            }
        }

   

    }

    /// <summary>
    /// Determine the location to spawn the note at (Either pre-determined or random, trying to avoid overlapping notes)
    /// </summary>
    /// <returns>A Vector3 representing the location to spawn the note at</returns>
    private Vector3 SpawnLocation()
    {
        if (spawnLocations.Count - 1 < spawnIndex || spawnLocations[spawnIndex] == new Vector3(0, 0, 0))
        {
            List<Vector3> existingNoteLocations = new List<Vector3>();
            foreach (var note in notes)
            {
                if (note != null)
                {
                    existingNoteLocations.Add(note.transform.position);
                }
            }
            Vector3 newLocation = new Vector3(
                UnityEngine.Random.Range(spawnAreaTopLeft.x, spawnAreaBottomRight.x),
                UnityEngine.Random.Range(spawnAreaTopLeft.y, spawnAreaBottomRight.y),
                spawnAreaTopLeft.z
            );
            // Attempt to find a location that doesn't overlap with any existing notes, up to 100 attempts to avoid infinite loop
            int attempts = 0;
            while (true)
            {
                attempts++;
                bool tooClose = false;
                foreach (var location in existingNoteLocations)
                {
                    if (Vector3.Distance(location, newLocation) < 6)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose && attempts < 100)
                {
                    newLocation = new Vector3(
                        UnityEngine.Random.Range(spawnAreaTopLeft.x, spawnAreaBottomRight.x),
                        UnityEngine.Random.Range(spawnAreaTopLeft.y, spawnAreaBottomRight.y),
                        spawnAreaTopLeft.z
                    );
                }
                else
                {
                    break;
                }
            }
            return newLocation;
        }
        else
        {
            return spawnLocations[spawnIndex];
        }
    }

    private void OnDrawGizmos()
    {
        // Draws the spawn area
        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnAreaTopLeft, new Vector3(spawnAreaBottomRight.x, spawnAreaTopLeft.y, spawnAreaTopLeft.z));
        Gizmos.DrawLine(spawnAreaTopLeft, new Vector3(spawnAreaTopLeft.x, spawnAreaBottomRight.y, spawnAreaTopLeft.z));
        Gizmos.DrawLine(spawnAreaBottomRight, new Vector3(spawnAreaBottomRight.x, spawnAreaTopLeft.y, spawnAreaTopLeft.z));
        Gizmos.DrawLine(spawnAreaBottomRight, new Vector3(spawnAreaTopLeft.x, spawnAreaBottomRight.y, spawnAreaTopLeft.z));
    }

    private void OnDrawGizmosSelected()
    {
        // Draws the spawn locations when the object is selected
        Gizmos.color = Color.red;
        foreach (var location in spawnLocations)
        {
            if (location != new Vector3(0, 0, 0))
            {
                Gizmos.DrawSphere(location, 0.5f);
            }
        }
    }
}
