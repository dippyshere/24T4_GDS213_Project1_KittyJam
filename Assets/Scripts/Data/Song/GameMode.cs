using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// The different types of games that can be played, and the instruments that are supported for each game mode, as well as the leaderboard ID for each game mode and song difficulty
/// </summary>
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

/// <summary>
/// The different types of games that can be played
/// </summary>
public enum GameType
{
    CircleGame,
    HighwayGame,
    MarchingGame,
    BongoGame
}
