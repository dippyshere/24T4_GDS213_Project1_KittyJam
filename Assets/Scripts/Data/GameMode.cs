using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMode
{
    [SerializeField, Tooltip("The type of game mode")] private GameType gameType;
    [SerializeField, Tooltip("The supported instruments for the game mode")] private List<Instrument> supportedInstruments;

    public GameType GameType { get => gameType; }
    public List<Instrument> SupportedInstruments { get => supportedInstruments; }
    public GameMode(GameType gameType, List<Instrument> supportedInstruments)
    {
        this.gameType = gameType;
        this.supportedInstruments = supportedInstruments;
    }
}

public enum GameType
{
    CircleGame,
    HighwayGame,
    MarchingGame,
    BongoGame
}
