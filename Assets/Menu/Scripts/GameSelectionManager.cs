using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The game selection tile for CircleGame")] private GameObject circleGameTile;
    [SerializeField, Tooltip("The game selection tile for HighwayGame")] private GameObject highwayGameTile;
    [SerializeField, Tooltip("The game selection tile for MarchingGame")] private GameObject marchingGameTile;
    [SerializeField, Tooltip("The game selection tile for BongoGame")] private GameObject bongoGameTile;
    [SerializeField, Tooltip("The text component for the song title")] private TextMeshProUGUI songTitle;
    [SerializeField, Tooltip("The text component for the song artist")] private TextMeshProUGUI songArtist;
    [SerializeField, Tooltip("Reference to the menu magager")] private MenuManager menuManager;

    public void UpdateGameSelectionScreen()
    {
        if (GlobalVariables.Get<SongData>("activeSong") != null)
        {
            SongData songData = GlobalVariables.Get<SongData>("activeSong");
            songTitle.text = songData.SongName;
            songArtist.text = songData.ArtistName;
            circleGameTile.SetActive(false);
            highwayGameTile.SetActive(false);
            marchingGameTile.SetActive(false);
            bongoGameTile.SetActive(false);
            foreach (GameMode gameMode in songData.GameModes)
            {
                switch (gameMode.GameType)
                {
                    case GameType.CircleGame:
                        circleGameTile.SetActive(true);
                        circleGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 1 + songData.name + "played") != null);
                        break;
                    case GameType.HighwayGame:
                        highwayGameTile.SetActive(true);
                        highwayGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 2 + songData.name + "played") != null);
                        break;
                    case GameType.MarchingGame:
                        marchingGameTile.SetActive(true);
                        marchingGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 3 + songData.name + "played") != null);
                        break;
                    case GameType.BongoGame:
                        bongoGameTile.SetActive(true);
                        bongoGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 4 + songData.name + "played") != null);
                        break;
                }
            }
        }
    }

    public void LoadGameOnboarding(int gameType)
    {
        SongSelectionManager.instance.StartCoroutine(SongSelectionManager.instance.fadeOutAudioSources());
        SongData songData = GlobalVariables.Get<SongData>("activeSong");
        GlobalVariables.Set(songData.name + "New", "0");
        if (GlobalVariables.Get<string>("GameType" + gameType + songData.name + "played") == null)
        {
            GlobalVariables.Set("GameType" + gameType + songData.name + "played", "1");
        }
        if (GlobalVariables.Get<string>("GameType" + gameType + "Onboarded") == null)
        {
            GlobalVariables.Set("GameType" + gameType + "Onboarded", "1");
            menuManager.StartLoadingSceneMusicStop("GameType" + gameType + "Onboarding");
        }
        else
        {
            switch (gameType)
            {
                case 1:
                    menuManager.StartLoadingSceneMusicStop("GameType1CircleGame");
                    break;
                case 2:
                    menuManager.StartLoadingSceneMusicStop("GameType2HighwayGame");
                    break;
                case 3:
                    menuManager.StartLoadingSceneMusicStop("GameType3MarchingGame");
                    break;
                case 4:
                    menuManager.StartLoadingSceneMusicStop("GameType4DDRGame");
                    break;
            }
        }
    }
}
