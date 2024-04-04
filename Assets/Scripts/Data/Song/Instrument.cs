using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instrument
{
    [SerializeField, Tooltip("The supported instrument")] private InstrumentType instrumentType;
    [SerializeField, Tooltip("The difficulty rating for the instrument"), Range(0, 7)] private uint difficulty;

    public InstrumentType InstrumentType { get => instrumentType; }
    public uint Difficulty { get => difficulty; }
    public Instrument(InstrumentType instrumentType, uint difficulty)
    {
        this.instrumentType = instrumentType;
        this.difficulty = difficulty;
    }
}

public enum InstrumentType
{
    Guitar,
    Bass,
    Drums,
    Keyboard,
    Vocals
}
