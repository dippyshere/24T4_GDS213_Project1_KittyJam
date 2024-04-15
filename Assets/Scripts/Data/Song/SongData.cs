using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Holds the data for a song, including the song's name, artist, album, genre, year, length, usage rights, MIDI file, audio file, album cover, BPM, preview start, preview end, loop point, and supported game modes
/// </summary>
[
    CreateAssetMenu(fileName = "New Song Data", menuName = "Kitty Jam/Song Data"),
    Icon("Assets/Plugins/Demigiant/DemiLib/Core/Editor/Imgs/project/ico_audio.png"),
    Tooltip("A scriptable object that contains data for a song"),
    HelpURL("https://discord.com/channels/@me/1137585685864402974/1214421770657071145")
]
public class SongData : ScriptableObject
{
    [Header("Song Information")]
    [SerializeField, Tooltip("The name of the song")] private string songName;
    [SerializeField, Tooltip("The name of the artist")] private string artistName;
    [SerializeField, Tooltip("The name of the albumn the song is from")] private string albumName;
    [SerializeField, Tooltip("The genre of the song")] private Genre genre;
    [SerializeField, Tooltip("The year the song/albumn was released")] private string year;
    [SerializeField, Tooltip("The length of the song (in seconds)")] private float songLength;
    [SerializeField, Tooltip("The usage rights for the song")] private UsageLicense usageLicense;
    [SerializeField, Tooltip("Songs will be listed by order of priority, then alphabetically. Set the priority higher to move the song to the front of the list")] private int priority;
    [SerializeField, Tooltip("The key that is used for the song icon in Discord Rich Presence")] private string discordIconKey;
    [Header("Song Data")]
    [SerializeField, Tooltip("The name of the song's MIDI file")] private string midiName;
    [SerializeField, Tooltip("The audio file associated with the song")] private AudioClip songAudio;
    [SerializeField, Tooltip("The albumn cover associated with the song")] private Sprite albumCover;
    [SerializeField, Tooltip("The BPM of the song")] private float bpm;
    [Header("Song Preview")]
    [SerializeField, Tooltip("The timestamp where the song preview should start (in seconds)")] private float previewStart;
    [SerializeField, Tooltip("The timestamp where the song preview should end (in seconds)")] private float previewEnd;
    [SerializeField, Tooltip("The timestamp where the song preview should loop (in seconds)")] private float loopPoint;
    [Header("Song Support")]
    [SerializeField, Tooltip("The supported game modes for the song, and their data")] private List<GameMode> gameModes;
    
    public string SongName { get => songName; }
    public string ArtistName { get => artistName; }
    public string AlbumName { get => albumName; }
    public Genre Genre { get => genre; }
    public string Year { get => year; }
    public float SongLength { get => songLength; }
    public UsageLicense UsageLicense { get => usageLicense; }
    public int Priority { get => priority; }
    public string DiscordIconKey { get => discordIconKey; }
    public string MidiName { get => midiName; }
    public AudioClip SongAudio { get => songAudio; }
    public Sprite AlbumCover { get => albumCover; }
    public float Bpm { get => bpm; }
    public float PreviewStart { get => previewStart; }
    public float PreviewEnd { get => previewEnd; }
    public float LoopPoint { get => loopPoint; }
    public List<GameMode> GameModes { get => gameModes; }
    public SongData(string songName, string artistName, string albumName, Genre genre, string year, float songLength, UsageLicense usageLicense, int priority, string discordIconKey, string midiName, AudioClip songAudio, Sprite albumCover, float bpm, float previewStart, float previewEnd, float loopPoint, List<GameMode> gameModes)
    {
        this.songName = songName;
        this.artistName = artistName;
        this.albumName = albumName;
        this.genre = genre;
        this.year = year;
        this.songLength = songLength;
        this.usageLicense = usageLicense;
        this.priority = priority;
        this.discordIconKey = discordIconKey;
        this.midiName = midiName;
        this.songAudio = songAudio;
        this.albumCover = albumCover;
        this.bpm = bpm;
        this.previewStart = previewStart;
        this.previewEnd = previewEnd;
        this.loopPoint = loopPoint;
        this.gameModes = gameModes;
    }
    public SongData(SongData songData)
    {
        songName = songData.songName;
        artistName = songData.artistName;
        albumName = songData.albumName;
        genre = songData.genre;
        year = songData.year;
        songLength = songData.songLength;
        usageLicense = songData.usageLicense;
        priority = songData.priority;
        discordIconKey = songData.discordIconKey;
        midiName = songData.midiName;
        songAudio = songData.songAudio;
        albumCover = songData.albumCover;
        bpm = songData.bpm;
        previewStart = songData.previewStart;
        previewEnd = songData.previewEnd;
        loopPoint = songData.loopPoint;
        gameModes = songData.gameModes;
    }
}

/// <summary>
/// The type of license the song is under
/// </summary>
public enum UsageLicense
{
    PublicDomain,
    CreativeCommons,
    RoyaltyFree,
    Licensed,
    Unlicensed
}

/// <summary>
/// The type of genre the song is
/// </summary>
public enum Genre
{
    Rock,
    Pop,
    HipHop,
    Jazz,
    Classical,
    Country,
    Electronic,
    Reggae,
    Metal,
    Punk,
    Blues,
    RnB,
    Soul,
    Funk,
    Disco,
    Techno,
    House,
    Dubstep,
    Trance,
    DrumAndBass,
    Dub,
    Ska,
    Grunge,
    Indie,
    Alternative,
    Folk,
    World,
    Latin,
    Gospel,
    NewAge,
    Ambient,
    Experimental,
    AvantGarde,
    Soundtrack,
    Vocal,
    EasyListening,
    Childrens,
    Comedy,
    SpokenWord,
    Other
}

/// <summary>
/// A custom inspector for the SongData class to display the album cover in the inspector
/// </summary>
#if UNITY_EDITOR
[
    CustomEditor(typeof(SongData), true),
    CanEditMultipleObjects
]
public class SongDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        try
        {
            SongData songData = (SongData)target;
            if (songData == null || songData.AlbumCover == null)
            {
                return null;
            }
            Texture2D source = songData.AlbumCover.texture;
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            EditorUtility.CopySerialized(songData.AlbumCover.texture, readableText);
            return readableText;
        }
        catch
        {
            return null;
        }
    }
}
#endif

/// <summary>
/// This class is used to reference a song data asset in the project.
/// </summary>
[System.Serializable]
public class SongDataAssetReference : AssetReference
{
    public SongDataAssetReference(string guid) : base(guid) { }

#if UNITY_EDITOR
    public override bool ValidateAsset(string path)
    {
        return path.EndsWith(".asset") && path.Contains("Songs");
    }
#endif

    /// <summary>
    /// Returns the GUID of the scene asset reference
    /// </summary>
    /// <param name="songDataAssetReference">The scene asset reference to get the GUID of</param>
    public static implicit operator string(SongDataAssetReference songDataAssetReference)
    {
        return songDataAssetReference.AssetGUID;
    }

    /// <summary>
    /// Returns the scene asset reference from a GUID
    /// </summary>
    /// <param name="guid">The GUID to get the scene asset reference from</param>
    public static implicit operator SongDataAssetReference(string guid)
    {
        return new SongDataAssetReference(guid);
    }

    /// <summary>
    /// Handles the equality comparison between two SongDataAssetReference objects
    /// </summary>
    /// <param name="obj">The object to compare against</param>
    /// <returns>The result of the equality comparison</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SongDataAssetReference other = (SongDataAssetReference)obj;
        return AssetGUID == other.AssetGUID;
    }

    /// <summary>
    /// Handles the hash code generation for the SongDataAssetReference object
    /// </summary>
    /// <returns>The hash code for the SongDataAssetReference object</returns>
    public override int GetHashCode()
    {
        return AssetGUID.GetHashCode();
    }
}
