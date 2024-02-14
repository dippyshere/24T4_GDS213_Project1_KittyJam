using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public KeyCode input;
    public GameObject notePrefab;
    public ScoreManager scoreManager;
    List<CircleGemController> notes = new List<CircleGemController>();
    public List<double> timeStamps = new List<double>();
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
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(spawnAreaTopLeft.x, spawnAreaBottomRight.x),
                    UnityEngine.Random.Range(spawnAreaTopLeft.y, spawnAreaBottomRight.y),
                    spawnAreaTopLeft.z
                );
                var note = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
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
}
