﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class DDRSongManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the song manager")] public static DDRSongManager Instance;
    [SerializeField, Tooltip("Audio source that is used to play the song")] private AudioSource audioSource;
    [SerializeField, Tooltip("Reference to the text object that displays the song name")] private TextMeshProUGUI songNameText;
    [SerializeField, Tooltip("Reference to the text object that displays the artist name")] private TextMeshProUGUI artistNameText;
    [SerializeField, Tooltip("Reference to the image object that displays the albumn art")] private Image albumnArtImage;
    [SerializeField, Tooltip("Reference to the lane managers")] private DDRLane[] DDRLanes;
    [SerializeField, Tooltip("The animator for the bongo cat")] private Animator bongoCatAnimator;
    [Tooltip("The prefab to spawn when hitting a note with good timing")] public GameObject goodHitPrefab;
    [Tooltip("The prefab to spawn when hitting a note with perfect timing")] public GameObject perfectHitPrefab;
    [SerializeField, Tooltip("The prefab to use on up beat markers")] private GameObject upBeatPrefab;
    [SerializeField, Tooltip("The prefab to use on down beat markers")] private GameObject downBeatPrefab;
    [Tooltip("Timestamps for up beat markers")] private List<float> upBeatTimestamps = new List<float>();
    [Tooltip("Timestamps for down beat markers")] private List<float> downBeatTimestamps = new List<float>();
    [Tooltip("The current index of the up beat marker")] private int upBeatIndex = 0;
    [Tooltip("The current index of the down beat marker")] private int downBeatIndex = 0;
    [SerializeField, Tooltip("Reference to the win screen game object")] private GameObject winScreen;
    [SerializeField, Tooltip("Reference to the text object that displays the final winning score")] private TextMeshProUGUI winScore;
    [SerializeField, Tooltip("Reference to the text object that displays a tally of the various scores")] private TextMeshProUGUI tallyScore;
    [SerializeField, Tooltip("Reference to the pause menu game object")] private PauseMenu pauseMenu;
    [SerializeField, Tooltip("A delay to add before the song begins to play")] private float songDelayInSeconds;
    [Tooltip("The range that a perfect hit can be achieved in")] public float perfectRange;
    [Tooltip("The range that a good hit can be achieved in")] public float goodRange;
    [Tooltip("The track speed multiplier to change how fast notes move")] public float trackSpeed = 1f;
    [Tooltip("The Y coordinate that notes spawn at")] public float noteSpawnZ = 10f;
    [Tooltip("The Y coordinate that notes should be tapped at")] public float noteTapY = -5f;
    [Tooltip("An input delay offset to account for when determining hit accuracy")] public int inputDelayInMilliseconds;
    [SerializeField, Tooltip("Name of the song MIDI from StreamingAssets")] private string fileLocation;
    [SerializeField, Tooltip("The BPM of the song")] private float bpm;
    [HideInInspector, Tooltip("Singleton reference to the current MIDI file")] public static MidiFile midiFile;
    [SerializeField, Tooltip("The song to play")] private AudioClip song;
    [SerializeField, Tooltip("The name of the song to display")] private string songName;
    [SerializeField, Tooltip("The name of the artist to display")] private string artistName;
    [SerializeField, Tooltip("The albumn art to display")] private Sprite albumnArt;
    [HideInInspector, Tooltip("The calculated speed for the notes")] public float noteTime;
    [HideInInspector, Tooltip("The calculated Y coordinate notes despawn at")]
    public float noteDespawnZ
    {
        get
        {
            return noteTapY - (noteSpawnZ - noteTapY);
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Instance = this;
        // Check if the streaming assets path is a URL (WebGL/Android) or a file path (Everything else)
        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            // If it's a URL, use a coroutine to asynchronously read the MIDI file
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }
        songNameText.text = songName;
        artistNameText.text = artistName;
        albumnArtImage.sprite = albumnArt;
        noteTime = 60f / bpm * 4 * trackSpeed;
        float bopSpeed = bpm / 60f;
        bongoCatAnimator.SetFloat("BPMMultiplier", bopSpeed);
        yield return new WaitForSecondsRealtime(60f / bpm);
        bongoCatAnimator.SetTrigger("StartBop");
    }

    /// <summary>
    /// Read the MIDI file from the streaming assets folder or website asynchronously
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator ReadFromWebsite()
    {
        using UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation);
        yield return www.SendWebRequest();

        // If there was an error, log it
        if (UnityWebRequest.Result.ConnectionError.Equals(www.result) || UnityWebRequest.Result.ProtocolError.Equals(www.result))
        {
            Debug.LogError(www.error);
        }
        else
        {
            // Otherwise, read the MIDI file from the downloaded data
            byte[] results = www.downloadHandler.data;
            using var stream = new MemoryStream(results);
            midiFile = MidiFile.Read(stream);
            GetDataFromMidi();
        }
    }

    /// <summary>
    /// Read the MIDI file from the streaming assets folder
    /// </summary>
    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        GetDataFromMidi();
    }

    /// <summary>
    /// Read the note data from the MIDI file and set the timestamps for the notes to be spawned at
    /// </summary>
    public void GetDataFromMidi()
    {
        var notes = midiFile.GetNotes();
        var array = new Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in DDRLanes) lane.SetTimeStamps(array);

        //float beatTime = 60f / bpm;
        //float time = songDelayInSeconds;
        //while (time < midiFile.GetDuration<MetricTimeSpan>().TotalSeconds)
        //{
        //    upBeatTimestamps.Add(time);
        //    time += beatTime;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        downBeatTimestamps.Add(time);
        //        time += beatTime;
        //    }
        //}
        //upBeatTimestamps.RemoveRange(0, 2);
        //downBeatTimestamps.RemoveRange(0, 6);

        float firstNoteTime = 0;
        foreach (var note in notes)
        {
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
            if (metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f < firstNoteTime)
            {
                firstNoteTime = metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f;
            }
        }
        float lastNoteTime = 0;
        foreach (var note in notes)
        {
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
            if (metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f > lastNoteTime)
            {
                lastNoteTime = metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f;
            }
        }
        Invoke(nameof(StartSong), songDelayInSeconds);
        Invoke(nameof(EndSong), songDelayInSeconds + lastNoteTime + 2.5f);
    }

    private void Update()
    {
        if (upBeatIndex < upBeatTimestamps.Count)
        {
            if (GetAudioSourceTime() >= upBeatTimestamps[upBeatIndex] - noteTime)
            {
                Instantiate(upBeatPrefab, transform);
                upBeatIndex++;
            }
        }

        if (downBeatIndex < downBeatTimestamps.Count)
        {
            if (GetAudioSourceTime() >= downBeatTimestamps[downBeatIndex] - noteTime)
            {
                Instantiate(downBeatPrefab, transform);
                downBeatIndex++;
            }
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Pause))
        {
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Start playing the song
    /// </summary>
    public void StartSong()
    {
        audioSource.clip = song;
        audioSource.Play();
    }

    public void PauseMusic(bool pause)
    {
        if (audioSource == null)
        {
            return;
        }
        if (pause)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    /// <summary>
    /// Get the current time of the audio source
    /// </summary>
    /// <returns>The current time of the audio source</returns>
    public static double GetAudioSourceTime()
    {
        if (Instance == null || Instance.audioSource == null)
        {
            return 0;
        }
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    /// <summary>
    /// End the song and display the win screen
    /// </summary>
    public void EndSong()
    {
        winScreen.SetActive(true);
        pauseMenu.PauseAction(true);
        PauseMusic(true);
        winScore.text = "Final Score: " + DDRScoreManager.Instance.score.ToString("N0", CultureInfo.InvariantCulture);
        tallyScore.text = "Perfect: " + DDRScoreManager.Instance.perfectCount.ToString("N0", CultureInfo.InvariantCulture) + "\nGood: " + DDRScoreManager.Instance.hitCount.ToString("N0", CultureInfo.InvariantCulture) + "\nMiss: " + DDRScoreManager.Instance.missCount.ToString("N0", CultureInfo.InvariantCulture);
    }
}
