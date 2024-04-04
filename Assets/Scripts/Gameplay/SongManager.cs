using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the song manager")] public static SongManager Instance;
    [Header("Game Configuration")]
    [SerializeField, Tooltip("Audio source that is used to play the song")] private AudioSource audioSource;
    [SerializeField, Tooltip("A delay to add before the song begins to play")] public float songDelayInSeconds;
    [Tooltip("The track speed multiplier to change how fast notes move")] public float trackSpeed = 1f;
    [Tooltip("An input delay offset to account for when determining hit accuracy")] public int inputDelayInMilliseconds;
    [Header("Song Configuration")]
    [SerializeField, Tooltip("The default song data to use")] private SongData songData;
    [HideInInspector, Tooltip("Name of the song MIDI from StreamingAssets")] private string fileLocation;
    [HideInInspector, Tooltip("Singleton reference to the current MIDI file")] public static MidiFile midiFile;
    [HideInInspector, Tooltip("The BPM of the song")] public float bpm;
    [HideInInspector, Tooltip("The song to play")] private AudioClip song;
    [HideInInspector, Tooltip("The calculated speed for the notes")] public float noteTime;
    [HideInInspector, Tooltip("A list of note timestamps")] public Note[] noteTimestamps;
    [HideInInspector, Tooltip("The time of the first note")] public float firstNoteTime = float.MaxValue;
    [HideInInspector, Tooltip("The time of the last note")] public float lastNoteTime = float.MinValue;

    private void OnEnable()
    {
        StartCoroutine(WaitForPauseMenuInstance());
    }

    private IEnumerator WaitForSongDisplayManagerInstance()
    {
        yield return new WaitUntil(() => SongDisplayManager.Instance != null);
        SongDisplayManager.Instance.SetSongData(songData.SongName, songData.ArtistName, songData.AlbumCover);
    }

    private IEnumerator WaitForPauseMenuInstance()
    {
        yield return new WaitUntil(() => PauseMenu.Instance != null);
        PauseMenu.Instance.OnPauseGameplay += PauseGameplay;
    }

    void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        // wait until the song data is loaded
        yield return null;
        if (GlobalVariables.Get<SongData>("activeSong") != null)
        {
            songData = GlobalVariables.Get<SongData>("activeSong");
        }
        fileLocation = songData.MidiName;
        song = songData.SongAudio;
        bpm = songData.Bpm;
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
        noteTime = 60f / bpm * 4 * trackSpeed;
        StartCoroutine(WaitForSongDisplayManagerInstance());
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
            StartCoroutine(GetDataFromMidi());
        }
    }

    /// <summary>
    /// Read the MIDI file from the streaming assets folder
    /// </summary>
    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        StartCoroutine(GetDataFromMidi());
    }

    /// <summary>
    /// Read the note data from the MIDI file and return the timestamps for the notes to be spawned at, then start the song & queue the end screen to display
    /// </summary>
    public IEnumerator GetDataFromMidi()
    {
        yield return new WaitUntil(() => midiFile != null);

        var notes = midiFile.GetNotes();
        var array = new Note[notes.Count];
        notes.CopyTo(array, 0);
        noteTimestamps = array;

        foreach (var note in notes)
        {
            var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
            if (metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f < firstNoteTime)
            {
                firstNoteTime = metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + metricTimeSpan.Milliseconds / 1000f;
            }
        }
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

    /// <summary>
    /// Start playing the song
    /// </summary>
    public void StartSong()
    {
        audioSource.clip = song;
        audioSource.Play();
    }

    /// <summary>
    /// Pauses the music
    /// </summary>
    /// <param name="pause">Whether to pause or unpause the music</param>
    public void PauseGameplay(bool pause)
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
    public double GetAudioSourceTime()
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
        PauseMenu.Instance.OnPauseGameplay?.Invoke(true);
    }
}
