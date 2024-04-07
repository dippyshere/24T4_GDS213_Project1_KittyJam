using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the display of the song name, artist name, and album art for the currently playing song
/// </summary>
public class SongDisplayManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the song ui manager")] public static SongDisplayManager Instance;
    [Header("References")]
    [SerializeField, Tooltip("Reference to the text object that displays the song name")] private TextMeshProUGUI songNameText;
    [SerializeField, Tooltip("Refernce to the text object that displays the artist name")] private TextMeshProUGUI artistNameText;
    [SerializeField, Tooltip("Reference to the image object that displays the album art")] private Image albumArtImage;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Sets the song data to display
    /// </summary>
    /// <param name="songName">The name of the song</param>
    /// <param name="artistName">The name of the artist</param>
    /// <param name="albumArt">The album art for the song</param>
    public void SetSongData(string songName, string artistName, Sprite albumArt)
    {
        songNameText.text = songName;
        artistNameText.text = artistName;
        albumArtImage.sprite = albumArt;
    }
}
