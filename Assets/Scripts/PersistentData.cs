using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// Handes the storage of persistent data between scenes
/// </summary>
public class PersistentData : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton instance of the persistent data")] public static PersistentData Instance;
    [Tooltip("The selected song to be played")] public IResourceLocation SelectedSongAssetLocation;
    [Tooltip("The selected song to be played")] public string songName;
    [Tooltip("The selected game type to be played")] public GameType SelectedGameType;

    public void Awake()
    {
        Instance = this;
    }

    public void SetSelectedSong(IResourceLocation selectedSongAssetLocation)
    {
        SelectedSongAssetLocation = selectedSongAssetLocation;
        songName = selectedSongAssetLocation.PrimaryKey;
    }
}
