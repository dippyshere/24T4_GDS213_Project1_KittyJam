using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of notes based on the MIDI file for Game Type 1 (CircleGame), in addition to handling other game specific behaviours and logic
/// </summary>
public class CircleNoteManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the circle note manager")] public static CircleNoteManager Instance;
    [Header("Configuration")]
    [SerializeField, Tooltip("Note number in the MIDI that will be used to spawn notes"), Range(0, 127)] private int noteNumber;
    [SerializeField, Tooltip("List of locations to manually place a specific note (Leave at 0,0,0 for random; Ensure Z is always at 4.89)")] private List<Vector3> spawnLocations = new List<Vector3>();
    [SerializeField, Tooltip("Top left position of the random spawn area")] private Vector3 spawnAreaTopLeft;
    [SerializeField, Tooltip("Bottom right position of the random spawn area")] private Vector3 spawnAreaBottomRight;
    [SerializeField, Tooltip("The radius around notes that new notes can't spawn within"), Range(0, 10)] private float noteRadius = 6f;
    [SerializeField, Tooltip("How fast the follow point moves to the target position"), Range(0, 100)] private float followPointSpeed = 10f;
    [SerializeField, Tooltip("How long notes will be on screen for before they need to be hit"), Range(0, 10)] public float noteTime = 1f;
    [Header("References")]
    [SerializeField, Tooltip("The gem prefab to spawn for notes")] private GameObject notePrefab;
    [SerializeField, Tooltip("The FollowPoint prefab to use")] private GameObject followPoint;
    [SerializeField, Tooltip("The note feedback prefab to use")] private GameObject noteFeedbackPrefab;
    [Tooltip("List of previous + current notes that have been spawned")] private List<CircleGemController> notes = new List<CircleGemController>();
    [Tooltip("List of all timestamps that notes will be spawned at")] private List<double> timeStamps = new List<double>();
    [Tooltip("The index of the currently spawned note")] private int spawnIndex = 0;
    [Tooltip("The target position of the follow point")] private Vector3 targetPosition;

    private IEnumerator Start()
    {
        Instance = this;
        followPoint.GetComponent<TrailRenderer>().enabled = false; 
        yield return new WaitUntil(() => SongManager.Instance != null);
        yield return new WaitUntil(() => SongManager.Instance.noteTimestamps != null);
        SetTimeStamps(SongManager.Instance.noteTimestamps);
        yield return new WaitUntil(() => ScoreManager.Instance != null);
        ScoreManager.Instance.noteScoreEvent += ShowNoteFeedback;
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
            if (SongManager.Instance.GetAudioSourceTime() >= timeStamps[spawnIndex] - noteTime)
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

                targetPosition = notes[spawnIndex].transform.position;
                followPoint.GetComponent<TrailRenderer>().enabled = true;
                
                spawnIndex++;
            }
        }

        if (followPoint.GetComponent<TrailRenderer>().enabled)
        {
            followPoint.transform.position = Vector3.Lerp(followPoint.transform.position, targetPosition, Time.smoothDeltaTime * followPointSpeed);
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
                    if (Vector2.Distance(location, newLocation) < noteRadius)
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
        Gizmos.DrawWireSphere(Vector3.zero, noteRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, noteRadius / 1.65f);
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

    /// <summary>
    /// Show the note feedback at the given position
    /// </summary>
    /// <param name="noteFeedback">The type of feedback to show</param>
    /// <param name="position">The position to show the feedback at</param>
    public void ShowNoteFeedback(NoteFeedback noteFeedback, Vector3 position)
    {
        GameObject feedbackObject = Instantiate(noteFeedbackPrefab, position, Quaternion.identity);
        feedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(noteFeedback);
    }
}
