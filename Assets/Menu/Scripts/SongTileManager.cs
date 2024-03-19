using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongTileManager : MonoBehaviour
{
    [SerializeField, Tooltip("The song data that this song tile represents")] public SongData songData;
    [SerializeField, Tooltip("The animator component for the song tile")] private Animator tileAnimatorController;
    [SerializeField, Tooltip("The Image component that will display the song's album art")] private Image albumArt;
    [SerializeField, Tooltip("The Text component that will display the song's title when hovered")] private TextMeshProUGUI songTitleHover;
    [SerializeField, Tooltip("The Text component that will display the song's title")] private TextMeshProUGUI songTitle;
    [SerializeField, Tooltip("The Text component that will display the song's artist")] private TextMeshProUGUI songArtist;
    [SerializeField, Tooltip("The icon for CircleGame compatibility & completion")] private GameObject circleGameIcon;
    [SerializeField, Tooltip("The icon for HighwayGame compatibility")] private GameObject highwayGameIcon;
    [SerializeField, Tooltip("The icon for MarchingGame compatibility")] private GameObject marchingGameIcon;
    [SerializeField, Tooltip("The icon for BongoGame compatibility")] private GameObject bongoGameIcon;
    [SerializeField, Tooltip("The icon for telling the player that they have not yet played the song")] private GameObject bangIcon;
    [SerializeField, Tooltip("The icon for telling the player that they cannot play the song yet")] private GameObject lockIcon;

    private void Start()
    {
        if (songData != null)
        {
            albumArt.sprite = songData.AlbumCover;
            songTitle.text = songData.SongName;
            //songArtist.text = songData.ArtistName;
            //songTitleHover.text = songData.SongName;
            circleGameIcon.SetActive(false);
            highwayGameIcon.SetActive(false);
            marchingGameIcon.SetActive(false);
            bongoGameIcon.SetActive(false);
            //bangIcon.SetActive(false);
            lockIcon.SetActive(false);
            foreach (GameMode gameMode in songData.GameModes)
            {
                switch (gameMode.GameType)
                {
                    case GameType.CircleGame:
                        circleGameIcon.SetActive(true);
                        break;
                    case GameType.HighwayGame:
                        highwayGameIcon.SetActive(true);
                        break;
                    case GameType.MarchingGame:
                        marchingGameIcon.SetActive(true);
                        break;
                    case GameType.BongoGame:
                        bongoGameIcon.SetActive(true);
                        break;
                }
            }
        }
    }
}
