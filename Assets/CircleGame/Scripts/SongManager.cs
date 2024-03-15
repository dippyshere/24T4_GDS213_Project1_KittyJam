using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class SongManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the song manager")] public static SongManager Instance;
    [Header("Game Configuration")]
    [SerializeField, Tooltip("Audio source that is used to play the song")] private AudioSource audioSource;
    [SerializeField, Tooltip("Reference to the text object that displays the song name")] private TextMeshProUGUI songNameText;
    [SerializeField, Tooltip("Refernce to the text object that displays the artist name")] private TextMeshProUGUI artistNameText;
    [SerializeField, Tooltip("Reference to the image object that displays the albumn art")] private Image albumnArtImage;
    [SerializeField, Tooltip("Reference to the note manager")] private NoteManager noteManager;
    [Tooltip("The game object that is used to display note feedback")] public GameObject noteFeedbackPrefab;
    [SerializeField, Tooltip("Refernece to the win screen game object")] private GameObject winScreen;
    [SerializeField, Tooltip("Reference to the text object that displays the final winning score")] private TextMeshProUGUI winScore;
    [SerializeField, Tooltip("Reference to the text object that displays a tally of the various scores")] private TextMeshProUGUI tallyScore;
    [SerializeField, Tooltip("A delay to add before the song begins to play")] private float songDelayInSeconds;
    [Tooltip("The range that a perfect hit can be achieved in")] public float perfectRange;
    [Tooltip("The range that a good hit can be achieved in")] public float goodRange;
    [Tooltip("How many seconds a note appears for before it needs to be hit")] public float noteTime;
    [Tooltip("An input delay offset to account for when determining hit accuracy")] public int inputDelayInMilliseconds;
    [Header("Song Configuration")]
    [SerializeField, Tooltip("The default song data to use")] private SongData songData;
    [HideInInspector, Tooltip("Name of the song MIDI from StreamingAssets")] private string fileLocation;
    [HideInInspector, Tooltip("Singleton reference to the current MIDI file")] public static MidiFile midiFile;
    [HideInInspector, Tooltip("The song to play")] private AudioClip song;
    [HideInInspector, Tooltip("The name of the song to display")] private string songName;
    [HideInInspector, Tooltip("The name of the artist to display")] private string artistName;
    [HideInInspector, Tooltip("The albumn art to display")] private Sprite albumnArt;

    private void OnEnable()
    {
        PauseMenu.OnPauseGameplay += PauseGameplay;
    }

    private void OnDisable()
    {
        PauseMenu.OnPauseGameplay -= PauseGameplay;
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (GlobalVariables.Get<SongData>("activeSong") != null)
        {
            songData = GlobalVariables.Get<SongData>("activeSong");
        }
        fileLocation = songData.MidiName;
        song = songData.SongAudio;
        songName = songData.SongName;
        artistName = songData.ArtistName;
        albumnArt = songData.AlbumCover;
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

        noteManager.SetTimeStamps(array);

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
        PauseMenu.OnPauseGameplay?.Invoke(true);
        winScore.text = "Final Score: " + ScoreManager.Instance.score.ToString("N0", CultureInfo.InvariantCulture);
        tallyScore.text = "Perfect: " + ScoreManager.Instance.perfectCount.ToString("N0", CultureInfo.InvariantCulture) + "\nGood: " + ScoreManager.Instance.hitCount.ToString("N0", CultureInfo.InvariantCulture) + "\nMiss: " + ScoreManager.Instance.missCount.ToString("N0", CultureInfo.InvariantCulture);
    }
}
