using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectionManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Instance of the SongSelectionManager")] public static SongSelectionManager instance;
    [SerializeField, Tooltip("Reference to the song list shelf")] private GameObject songListShelf;
    [SerializeField, Tooltip("Reference to the selection ui animator")] private Animator selectionUIAnimator;
    [SerializeField, Tooltip("Reference to the game selection manager")] private GameSelectionManager gameSelectionManager;
    [SerializeField, Tooltip("Reference to the song shelf scroller")] private SongShelfScrolling songShelfScrolling;
    [SerializeField, Tooltip("Reference to the prefab for the song tile")] private GameObject songTilePrefab;
    [SerializeField, Tooltip("A list of all the songs to populate the menu with")] private List<SongData> songs = new List<SongData>();
    [HideInInspector, Tooltip("The currently active song tile")] public SongTileManager activeSongTile;
    [SerializeField, Tooltip("Reference to the audio sources for the song selection menu")] private AudioSource[] audioSources = new AudioSource[2];
    [HideInInspector, Tooltip("References to all the song tiles")] private List<SongTileManager> songTiles = new List<SongTileManager>();
    [Tooltip("The currently active audio source index")] private int activeAudioSource = 0;
    [Tooltip("The next time that audio will start")] private double nextStartTime = 0;
    [Tooltip("If the song has started looping or not")] private bool isLooping;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        PopulateSongList();
        songShelfScrolling.UpdatePages();
        selectionUIAnimator.SetBool("ShowSongUI", true);
    }

    private void PopulateSongList()
    {
        foreach (SongData song in songs)
        {
            GameObject songTile = Instantiate(songTilePrefab, songListShelf.transform);
            SongTileManager songTileManager = songTile.GetComponent<SongTileManager>();
            songTileManager.songData = song;
            songTiles.Add(songTileManager);
        }
    }

    public void ChangeSong(SongTileManager songTileManager)
    {
        if (songTileManager == activeSongTile)
        {
            return;
        }
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }
        foreach (SongTileManager songTile in songTiles)
        {
            songTile.StopBPM();
        }
        activeSongTile = songTileManager;
        CrossfadeSong();
    }

    private void CrossfadeSong()
    {
        isLooping = false;
        int otherAudioSource = activeAudioSource == 0 ? 1 : 0;
        if (audioSources[0].clip == null)
        {
            audioSources[0].clip = activeSongTile.songData.SongAudio;
            audioSources[1].clip = activeSongTile.songData.SongAudio;
        }
        else
        {
            audioSources[otherAudioSource].Stop();
            audioSources[otherAudioSource].clip = activeSongTile.songData.SongAudio;
        }
        audioSources[otherAudioSource].time = activeSongTile.songData.PreviewStart;
        audioSources[otherAudioSource].PlayScheduled(AudioSettings.dspTime);
        nextStartTime = AudioSettings.dspTime + (activeSongTile.songData.PreviewEnd - activeSongTile.songData.PreviewStart);
        audioSources[otherAudioSource].SetScheduledEndTime(nextStartTime);
        audioSources[activeAudioSource].Stop();
        audioSources[activeAudioSource].clip = activeSongTile.songData.SongAudio;
        activeAudioSource = activeAudioSource == 0 ? 1 : 0;
        isLooping = true;
    }

    void Update()
    {
        if (isLooping && AudioSettings.dspTime >= nextStartTime - 1)
        {
            activeAudioSource = activeAudioSource == 0 ? 1 : 0;
            audioSources[activeAudioSource].time = activeSongTile.songData.LoopPoint;
            audioSources[activeAudioSource].PlayScheduled(nextStartTime);
            nextStartTime += activeSongTile.songData.PreviewEnd - activeSongTile.songData.LoopPoint;
            audioSources[activeAudioSource].SetScheduledEndTime(nextStartTime);
        }
    }

    public void SelectSong(SongData songData)
    {
        GlobalVariables.Set("activeSong", songData);
        gameSelectionManager.UpdateGameSelectionScreen();
        selectionUIAnimator.SetBool("ShowGameUI", true);
    }

    public void ReturnToSongSelection()
    {
        selectionUIAnimator.SetBool("ShowSongUI", true);
    }

    public IEnumerator fadeOutAudioSources()
    {
        isLooping = false;
        int otherAudioSource = activeAudioSource == 0 ? 1 : 0;
        float startVolume = audioSources[activeAudioSource].volume;
        while (audioSources[activeAudioSource].volume > 0)
        {
            audioSources[activeAudioSource].volume -= startVolume * Time.deltaTime / 1;
            audioSources[otherAudioSource].volume -= startVolume * Time.deltaTime / 1;
            yield return null;
        }
        audioSources[activeAudioSource].Stop();
        audioSources[otherAudioSource].Stop();
        audioSources[activeAudioSource].volume = startVolume;
        audioSources[otherAudioSource].volume = startVolume;
    }
}
