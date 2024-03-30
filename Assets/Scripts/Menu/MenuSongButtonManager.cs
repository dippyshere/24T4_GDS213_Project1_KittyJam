using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSongButtonManager : MonoBehaviour
{
    [SerializeField, Tooltip("The song data to load the level with")] private SongData songData;
    [SerializeField, Tooltip("The game type to load the level with")] private TempGameType gameType;

    public void LoadSong()
    {
        GlobalVariables.Set("activeSong", songData);
        switch (gameType)
        {
            case TempGameType.CircleGame:
                TransitionManager.Instance.StartLoadingSceneMusicStop("GameType1CircleGame");
                break;
            case TempGameType.HighwayGame:
                TransitionManager.Instance.StartLoadingSceneMusicStop("GameType2HighwayGame");
                break;
            case TempGameType.MarchingGame:
                TransitionManager.Instance.StartLoadingSceneMusicStop("GameType3MarchingGame");
                break;
            case TempGameType.BongoGame:
                TransitionManager.Instance.StartLoadingSceneMusicStop("GameType4DDRGame");
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
