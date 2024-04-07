using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// Handles the selection of songs and the transition to the game selection screen
/// </summary>
public class SongSelectionManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Instance of the SongSelectionManager")] public static SongSelectionManager instance;
    [SerializeField, Tooltip("Reference to the song list shelf")] private GameObject songListShelf;
    [SerializeField, Tooltip("Reference to the selection ui animator")] private Animator selectionUIAnimator;
    [SerializeField, Tooltip("Reference to the game selection manager")] private GameSelectionManager gameSelectionManager;
    [SerializeField, Tooltip("Reference to the song shelf scroller")] private SongShelfScrolling songShelfScrolling;
    [SerializeField, Tooltip("Reference to the prefab for the song tile")] private GameObject songTilePrefab;
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
        selectionUIAnimator.SetBool("ShowSongUI", true);
    }

    /// <summary>
    /// Populates the song list with the songs in the songs list
    /// </summary>
    private void PopulateSongList()
    {
        List<IResourceLocation> songPaths = new List<IResourceLocation>();
        Addressables.LoadResourceLocationsAsync("songdata", typeof(SongData)).Completed += (op) =>
        {
            foreach (var location in op.Result)
            {
                songPaths.Add(location);
            }

            StartCoroutine(AddSongTiles(songPaths));
        };
    }

    private IEnumerator AddSongTiles(List<IResourceLocation> songPaths)
    {
        List<SongData> songData = new List<SongData>();
        Dictionary<SongData, IResourceLocation> songDataLocations = new Dictionary<SongData, IResourceLocation>();
        foreach (IResourceLocation song in songPaths)
        {
            AsyncOperationHandle<SongData> opHandle = Addressables.LoadAssetAsync<SongData>(song);
            yield return new WaitUntil(() => opHandle.IsDone);

            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                songData.Add(opHandle.Result);
                songDataLocations.Add(opHandle.Result, song);
                Addressables.Release(opHandle);
            }
            else
            {
                Debug.LogError("Failed to load song data asset reference: " + song);
            }
        }
        songData.Sort((x, y) => x.Priority == y.Priority ? x.SongName.CompareTo(y.SongName) : y.Priority.CompareTo(x.Priority));
        foreach (SongData song in songData)
        {
            GameObject songTile = Instantiate(songTilePrefab, songListShelf.transform);
            SongTileManager songTileManager = songTile.GetComponent<SongTileManager>();
            songTileManager.songDataAssetLocation = songDataLocations[song];
            songTileManager.songData = song;
            songTiles.Add(songTileManager);
        }
        songShelfScrolling.UpdatePages();
    }

    /// <summary>
    /// Changes the song to the selected song
    /// </summary>
    /// <param name="songTileManager">The song tile manager of the selected song</param>
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

    /// <summary>
    /// Crossfades the song to the selected song
    /// </summary>
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

    /// <summary>
    /// Selects the song and transitions to the game selection screen
    /// </summary>
    /// <param name="songData"></param>
    public void SelectSong(IResourceLocation songDataLocation)
    {
        PersistentData.Instance.SetSelectedSong(songDataLocation);
        StartCoroutine(gameSelectionManager.UpdateGameSelectionScreen());
        selectionUIAnimator.SetBool("ShowGameUI", true);
    }

    /// <summary>
    /// Returns to the song selection screen
    /// </summary>
    public void ReturnToSongSelection()
    {
        selectionUIAnimator.SetBool("ShowSongUI", true);
    }

    /// <summary>
    /// Fades out the audio sources
    /// </summary>
    /// <returns>The coroutine</returns>
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
