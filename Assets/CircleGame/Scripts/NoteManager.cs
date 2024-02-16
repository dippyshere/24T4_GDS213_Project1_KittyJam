using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public GameObject notePrefab;
    public ScoreManager scoreManager;
    List<CircleGemController> notes = new List<CircleGemController>();
    public List<Vector3> spawnLocations = new List<Vector3>();
    List<double> timeStamps = new List<double>();
    public Vector3 spawnAreaTopLeft;
    public Vector3 spawnAreaBottomRight;

    int spawnIndex = 0;

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
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
                spawnIndex++;
            }
        }
           
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnAreaTopLeft, new Vector3(spawnAreaBottomRight.x, spawnAreaTopLeft.y, spawnAreaTopLeft.z));
        Gizmos.DrawLine(spawnAreaTopLeft, new Vector3(spawnAreaTopLeft.x, spawnAreaBottomRight.y, spawnAreaTopLeft.z));
        Gizmos.DrawLine(spawnAreaBottomRight, new Vector3(spawnAreaBottomRight.x, spawnAreaTopLeft.y, spawnAreaTopLeft.z));
        Gizmos.DrawLine(spawnAreaBottomRight, new Vector3(spawnAreaTopLeft.x, spawnAreaBottomRight.y, spawnAreaTopLeft.z));
    }

    private Vector3 SpawnLocation()
    {
        // return the vector3 of the spawn location in the spawnLocations list if it is not 0, 0, 0
        // if it is 0, 0, 0, get the location of all notes that currently exist, and pick a random location that is not within a certain distance of any of the existing notes, within the spawn area

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // draw a sphere at each spawn location if it is not 0, 0, 0
        foreach (var location in spawnLocations)
        {
            if (location != new Vector3(0, 0, 0))
            {
                Gizmos.DrawSphere(location, 0.5f);
            }
        }
    }
}
