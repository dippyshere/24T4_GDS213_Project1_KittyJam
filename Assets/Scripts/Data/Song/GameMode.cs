using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class GameMode
{
    [SerializeField, Tooltip("The type of game mode")] private GameType gameType;
    [SerializeField, Tooltip("The supported instruments for the game mode")] private List<Instrument> supportedInstruments;
    [SerializeField, Tooltip("The ID that corresponds to the leaderboard for this song & game mode")] private string leaderboardID;

    public GameType GameType { get => gameType; }
    public List<Instrument> SupportedInstruments { get => supportedInstruments; }
    public GameMode(GameType gameType, List<Instrument> supportedInstruments)
    {
        this.gameType = gameType;
        this.supportedInstruments = supportedInstruments;
    }
    public string LeaderboardID { get => leaderboardID; }
}

public enum GameType
{
    CircleGame,
    HighwayGame,
    MarchingGame,
    BongoGame
}
