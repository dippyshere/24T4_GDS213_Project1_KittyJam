﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;
using TMPro;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;
    public AudioSource audioSource;
    public NoteManager noteManager;
    public GameObject noteFeedbackPrefab;
    public GameObject winScreen;
    public TextMeshProUGUI winScore;
    public TextMeshProUGUI tallyScore;
    public PauseMenu pauseMenu;
    public float songDelayInSeconds;
    public float perfectRange;
    public float goodRange;
    public float noteTime;

    public int inputDelayInMilliseconds;
    

    public string fileLocation;

    public static MidiFile midiFile;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }
    }

    // Used for WebGL and Android, as streaming assets on those platforms are URLs instead of file paths
    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (UnityWebRequest.Result.ConnectionError.Equals(www.result) || UnityWebRequest.Result.ProtocolError.Equals(www.result))
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    midiFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }
        }
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        noteManager.SetTimeStamps(array);

        Invoke(nameof(StartSong), songDelayInSeconds);
        Invoke(nameof(EndSong), (float)midiFile.GetDuration<MetricTimeSpan>().TotalSeconds + songDelayInSeconds);
    }

    public void StartSong()
    {
        audioSource.Play();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    public static double GetAudioSourceTime()
    {
        if (Instance == null || Instance.audioSource == null)
        {
            return 0;
        }
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    public void EndSong()
    {
        winScreen.SetActive(true);
        pauseMenu.PauseAction(true);
        OnApplicationPause(true);
        winScore.text = "Final Score: " + ScoreManager.Instance.score;
        tallyScore.text = "Perfect: " + ScoreManager.Instance.perfectCount + "\nGood: " + ScoreManager.Instance.hitCount + "\nMiss: " + ScoreManager.Instance.missCount;
    }
}
