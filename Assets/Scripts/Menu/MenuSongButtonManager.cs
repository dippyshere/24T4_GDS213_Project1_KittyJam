using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class MenuSongButtonManager : MonoBehaviour
{
    [SerializeField, Tooltip("The song data to load the level with")] private SongData songData;
    [SerializeField, Tooltip("The game type to load the level with")] private TempGameType gameType;
    [SerializeField, Tooltip("Reference to the menu manager")] private MenuManager menuManager;

    public void LoadSong()
    {
        GlobalVariables.Set("activeSong", songData);
        switch (gameType)
        {
            case TempGameType.CircleGame:
                menuManager.StartLoadingSceneMusicStop("GameType1CircleGame");
                break;
            case TempGameType.HighwayGame:
                menuManager.StartLoadingSceneMusicStop("GameType2HighwayGame");
                break;
            case TempGameType.MarchingGame:
                menuManager.StartLoadingSceneMusicStop("GameType3MarchingGame");
                break;
            case TempGameType.BongoGame:
                menuManager.StartLoadingSceneMusicStop("GameType4DDRGame");
                break;
        }
    }

    private enum TempGameType
    {
        CircleGame,
        HighwayGame,
        MarchingGame,
        BongoGame
    }
}
