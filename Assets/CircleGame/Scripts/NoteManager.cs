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
    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public Vector3 spawnAreaTopLeft;
    public Vector3 spawnAreaBottomRight;

    int spawnIndex = 0;
    int inputIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }
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
                notes.Add(note.GetComponent<Note>());
                note.GetComponent<CircleGemController>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }

        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.inputDelayInMilliseconds / 1000.0);

            //if (Input.GetKeyDown(input))
            //{
            //    if (Math.Abs(audioTime - timeStamp) < marginOfError)
            //    {
            //        Hit();
            //        print($"Hit on {inputIndex} note");
            //        Destroy(notes[inputIndex].gameObject);
            //        inputIndex++;
            //    }
            //    else
            //    {
            //        print($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
            //    }
            //}
            //if (timeStamp + marginOfError <= audioTime)
            //{
            //    Miss();
            //    print($"Missed {inputIndex} note");
            //    inputIndex++;
            //}
        }       
    
    }
    private void Hit()
    {
        scoreManager.Hit();
    }
    private void Miss()
    {
        scoreManager.Miss();
    }
}
