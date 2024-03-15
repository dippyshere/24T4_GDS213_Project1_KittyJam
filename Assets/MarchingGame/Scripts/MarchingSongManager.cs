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
using UnityEngine.InputSystem;

public class MarchingSongManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the song manager")] public static MarchingSongManager Instance;
    [Header("Game Configuration")]
    [SerializeField, Tooltip("Audio source that is used to play the song")] private AudioSource audioSource;
    [SerializeField, Tooltip("Reference to the text object that displays the song name")] private TextMeshProUGUI songNameText;
    [SerializeField, Tooltip("Reference to the text object that displays the artist name")] private TextMeshProUGUI artistNameText;
    [SerializeField, Tooltip("Reference to the image object that displays the albumn art")] private Image albumnArtImage;
    [SerializeField, Tooltip("Reference to the note manager")] private MarchingLane[] marchingLanes;
    [SerializeField, Tooltip("Reference to the beat managers")] private MarchingBeat[] marchingBeats;
    [SerializeField, Tooltip("Reference to the win screen game object")] private GameObject winScreen;
    [SerializeField, Tooltip("Reference to the text object that displays the final winning score")] private TextMeshProUGUI winScore;
    [SerializeField, Tooltip("Reference to the text object that displays a tally of the various scores")] private TextMeshProUGUI tallyScore;
    [SerializeField, Tooltip("Reference to cat animator")] private Animator catAnimator;
    [SerializeField, Tooltip("Reference to the player controller")] private MarchingPlayer playerController;
    [SerializeField, Tooltip("A delay to add before the song begins to play")] private float songDelayInSeconds;
    [Tooltip("The range that a perfect hit can be achieved in")] public float perfectRange;
    [Tooltip("The range that a good hit can be achieved in")] public float goodRange;
    [Tooltip("The track speed multiplier to change how fast notes move")] public float trackSpeed = 1f;
    [Tooltip("The Y coordinate that beat notes spawn at")] public float beatNoteSpawnY = 300f;
    [Tooltip("The Y coordinate that beat notes should be tapped at")] public float beatNoteTapY = 0;
    [Tooltip("An input delay offset to account for when determining hit accuracy")] public int inputDelayInMilliseconds;
    [Header("Song Configuration")]
    [SerializeField, Tooltip("The default song data to use")] private SongData songData;
    [HideInInspector, Tooltip("Name of the song MIDI from StreamingAssets")] private string fileLocation;
    [HideInInspector, Tooltip("Singleton reference to the current MIDI file")] public static MidiFile midiFile;
    [HideInInspector, Tooltip("The BPM of the song")] private float bpm;
    [HideInInspector, Tooltip("The song to play")] private AudioClip song;
    [HideInInspector, Tooltip("The name of the song to display")] private string songName;
    [HideInInspector, Tooltip("The name of the artist to display")] private string artistName;
    [HideInInspector, Tooltip("The albumn art to display")] private Sprite albumnArt;
    [HideInInspector, Tooltip("The calculated speed for the notes")] public float noteTime;
    [HideInInspector, Tooltip("The calculated speed for the beat notes")] public float beatNoteTime;
    [HideInInspector, Tooltip("The calculated Y coordinate beat notes despawn at")]
    public float beatNoteDespawnY
    {
        get
        {
            return beatNoteTapY - (beatNoteSpawnY - beatNoteTapY);
        }
    }

    private void OnEnable()
    {
        PauseMenu.OnPauseGameplay += PauseGameplay;
    }

    private void OnDisable()
    {
        PauseMenu.OnPauseGameplay -= PauseGameplay;
    }

    // Start is called before the first frame update
    IEnumerator Start()
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
        songNameText.text = songName;
        artistNameText.text = artistName;
        albumnArtImage.sprite = albumnArt;
        noteTime = 60f / bpm * 4 * trackSpeed;
        beatNoteTime = 60f / bpm * 2 * trackSpeed;
        catAnimator.SetFloat("Speed", bpm / 109.0909090909f);
        yield return new WaitForSecondsRealtime((0.2833333333f * (bpm / 109.0909090909f)) + songDelayInSeconds);
        catAnimator.SetTrigger("Marching");
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

        foreach (var lane in marchingLanes) lane.SetTimeStamps(array);

        float beatTime = 60f / bpm;
        float time = songDelayInSeconds;
        List<double> upBeatTimestamps = new List<double>();
        List<double> downBeatTimestamps = new List<double>();
        while (time < midiFile.GetDuration<MetricTimeSpan>().TotalSeconds)
        {
            upBeatTimestamps.Add(time);
            time += beatTime;
            downBeatTimestamps.Add(time);
            time += beatTime;
        }
        upBeatTimestamps.RemoveRange(0, 8);
        downBeatTimestamps.RemoveRange(0, 8);
        marchingBeats[0].SetTimeStamps(upBeatTimestamps);
        marchingBeats[1].SetTimeStamps(downBeatTimestamps);

        Invoke(nameof(StartSong), songDelayInSeconds);
        Invoke(nameof(EndSong), audioSource.clip.length + songDelayInSeconds - 1f);
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
    /// Hit the up beat lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            marchingBeats[0].Hit();
        }
    }

    /// <summary>
    /// Hit the down beat lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            marchingBeats[1].Hit();
        }
    }

    /// <summary>
    /// Handles the mouse input for the wand movement on the player controller
    /// </summary>
    /// <param name="context">The input context</param>
    public void WandMovement(InputAction.CallbackContext context)
    {
        playerController.HandleMouseInput(context);
    }

    /// <summary>
    /// End the song and display the win screen
    /// </summary>
    public void EndSong()
    {
        winScreen.SetActive(true);
        PauseMenu.OnPauseGameplay?.Invoke(true);
        winScore.text = "Final Score: " + MarchingScoreManager.Instance.score.ToString("N0", CultureInfo.InvariantCulture);
        tallyScore.text = "Perfect: " + MarchingScoreManager.Instance.perfectCount.ToString("N0", CultureInfo.InvariantCulture) + "\nGood: " + MarchingScoreManager.Instance.hitCount.ToString("N0", CultureInfo.InvariantCulture) + "\nMiss: " + MarchingScoreManager.Instance.missCount.ToString("N0", CultureInfo.InvariantCulture);
    }
}
