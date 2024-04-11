using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Services.Leaderboards.Models;

/// <summary>
/// Handles the game selection screen
/// </summary>
public class GameSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The game selection tile for CircleGame")] private GameObject circleGameTile;
    [SerializeField, Tooltip("The game selection tile for HighwayGame")] private GameObject highwayGameTile;
    [SerializeField, Tooltip("The game selection tile for MarchingGame")] private GameObject marchingGameTile;
    [SerializeField, Tooltip("The game selection tile for BongoGame")] private GameObject bongoGameTile;
    [SerializeField, Tooltip("The text component for the song title")] private TextMeshProUGUI songTitle;
    [SerializeField, Tooltip("The text component for the song artist")] private TextMeshProUGUI songArtist;
    [SerializeField, Tooltip("The scene load info for game type 1")] private SceneLoadInfo game1;
    [SerializeField, Tooltip("The scene load info for game type 1 onboarding")] private SceneLoadInfo game1Onboarding;
    [SerializeField, Tooltip("The scene load info for game type 2")] private SceneLoadInfo game2;
    [SerializeField, Tooltip("The scene load info for game type 2 onbarding")] private SceneLoadInfo game2Onboarding;
    [SerializeField, Tooltip("The scene load info for game type 3")] private SceneLoadInfo game3;
    [SerializeField, Tooltip("The scene load info for game type 3 onboarding")] private SceneLoadInfo game3Onboarding;
    [SerializeField, Tooltip("The scene load info for game type 4")] private SceneLoadInfo game4;
    [SerializeField, Tooltip("The scene load info for game type 4 onboarding")] private SceneLoadInfo game4Onboarding;
    [Header("Leaderboard References")]
    [SerializeField, Tooltip("The leaderboard entry prefab")] private GameObject leaderboardEntryPrefab;
    [SerializeField, Tooltip("The leaderboard entry parent")] private Transform leaderboardEntryParent;
    [SerializeField, Tooltip("Reference to the text containging the leaderboard title")] private TextMeshProUGUI leaderboardTitle;
    [Tooltip("The cached songData object")] private SongData cachedSongData;

    /// <summary>
    /// Handles the update of the game selection screen
    /// </summary>
    public IEnumerator UpdateGameSelectionScreen()
    {
        if (GlobalVariables.Get<IResourceLocation>("activeSongLocation") != null)
        {
            circleGameTile.SetActive(false);
            highwayGameTile.SetActive(false);
            marchingGameTile.SetActive(false);
            bongoGameTile.SetActive(false);
            bool leaderboardLoaded = false;
            ClearLeaderboard();

            SongData songData = null;
            cachedSongData = null;
            IResourceLocation songDataAssetLocation = GlobalVariables.Get<IResourceLocation>("activeSongLocation");
            AsyncOperationHandle<SongData> opHandle = Addressables.LoadAssetAsync<SongData>(songDataAssetLocation);
            yield return new WaitUntil(() => opHandle.IsDone);

            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                songData = opHandle.Result;
                cachedSongData = songData;
                Addressables.Release(opHandle);
            }
            else
            {
                Debug.LogError("Failed to load song data asset reference: " + songDataAssetLocation);
            }
            songTitle.text = songData.SongName;
            songArtist.text = songData.ArtistName;
            foreach (GameMode gameMode in songData.GameModes)
            {
                switch (gameMode.GameType)
                {
                    case GameType.CircleGame:
                        circleGameTile.SetActive(true);
                        circleGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 1 + songData.name + "played") != null);
                        if (!leaderboardLoaded && gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                        {
                            leaderboardLoaded = true;
                            LoadGame1Leaderboard();
                        }
                        break;
                    case GameType.HighwayGame:
                        highwayGameTile.SetActive(true);
                        highwayGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 2 + songData.name + "played") != null);
                        if (!leaderboardLoaded && gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                        {
                            leaderboardLoaded = true;
                            LoadGame2Leaderboard();
                        }
                        break;
                    case GameType.MarchingGame:
                        marchingGameTile.SetActive(true);
                        marchingGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 3 + songData.name + "played") != null);
                        if (!leaderboardLoaded && gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                        {
                            leaderboardLoaded = true;
                            LoadGame3Leaderboard();
                        }
                        break;
                    case GameType.BongoGame:
                        bongoGameTile.SetActive(true);
                        bongoGameTile.GetComponent<SongTileManager>().UpdateNewBadge(GlobalVariables.Get<string>("GameType" + 4 + songData.name + "played") != null);
                        if (!leaderboardLoaded && gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                        {
                            leaderboardLoaded = true;
                            LoadGame4Leaderboard();
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Clears the leaderboard entries
    /// </summary>
    private void ClearLeaderboard()
    {
        leaderboardTitle.text = "Loading...";
        foreach (Transform child in leaderboardEntryParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Loads the game 1 leaderboard entries
    /// </summary>
    /// <returns>The task</returns>
    public async void LoadGame1Leaderboard()
    {
        ClearLeaderboard();
        if (cachedSongData == null)
        {
            return;
        }
        leaderboardTitle.text = "Paw Percussion";
        foreach (GameMode gameMode in cachedSongData.GameModes)
        {
            if (gameMode.GameType == GameType.CircleGame)
            {
                if (gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                {
                    LeaderboardScoresPage scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(gameMode.LeaderboardID, new GetScoresOptions { Offset = 0, Limit = 8 });
                    if (scoresResponse != null)
                    {
                        foreach (LeaderboardEntry entry in scoresResponse.Results)
                        {
                            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardEntryParent);
                            leaderboardEntry.GetComponent<LeaderboardEntryManager>().SetProperties(entry);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the game 2 leaderboard entries
    /// </summary>
    /// <returns>The task</returns>
    public async void LoadGame2Leaderboard()
    {
        ClearLeaderboard();
        if (cachedSongData == null)
        {
            return;
        }
        leaderboardTitle.text = "Feline Fretboard";
        foreach (GameMode gameMode in cachedSongData.GameModes)
        {
            if (gameMode.GameType == GameType.HighwayGame)
            {
                if (gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                {
                    LeaderboardScoresPage scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(gameMode.LeaderboardID, new GetScoresOptions { Offset = 0, Limit = 8 });
                    if (scoresResponse != null)
                    {
                        foreach (LeaderboardEntry entry in scoresResponse.Results)
                        {
                            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardEntryParent);
                            leaderboardEntry.GetComponent<LeaderboardEntryManager>().SetProperties(entry);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the game 3 leaderboard entries
    /// </summary>
    /// <returns>The task</returns>
    public async void LoadGame3Leaderboard()
    {
        ClearLeaderboard();
        if (cachedSongData == null)
        {
            return;
        }
        leaderboardTitle.text = "Pouncing Parade";
        foreach (GameMode gameMode in cachedSongData.GameModes)
        {
            if (gameMode.GameType == GameType.MarchingGame)
            {
                if (gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                {
                    LeaderboardScoresPage scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(gameMode.LeaderboardID, new GetScoresOptions { Offset = 0, Limit = 8 });
                    if (scoresResponse != null)
                    {
                        foreach (LeaderboardEntry entry in scoresResponse.Results)
                        {
                            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardEntryParent);
                            leaderboardEntry.GetComponent<LeaderboardEntryManager>().SetProperties(entry);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the game 4 leaderboard entries
    /// </summary>
    /// <returns>The task</returns>
    public async void LoadGame4Leaderboard()
    {
        ClearLeaderboard();
        if (cachedSongData == null)
        {
            return;
        }
        leaderboardTitle.text = "Bongo Bash";
        foreach (GameMode gameMode in cachedSongData.GameModes)
        {
            if (gameMode.GameType == GameType.BongoGame)
            {
                if (gameMode.LeaderboardID != null && gameMode.LeaderboardID != "")
                {
                    LeaderboardScoresPage scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(gameMode.LeaderboardID, new GetScoresOptions { Offset = 0, Limit = 8 });
                    if (scoresResponse != null)
                    {
                        foreach (LeaderboardEntry entry in scoresResponse.Results)
                        {
                            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardEntryParent);
                            leaderboardEntry.GetComponent<LeaderboardEntryManager>().SetProperties(entry);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the game onboarding scene, or the game scene if the player has already been onboarded
    /// </summary>
    /// <param name="gameType">The game type to load</param>
    public void LoadGameOnboarding(int gameType)
    {
        StartCoroutine(LoadGameScene(gameType));
    }

    private IEnumerator LoadGameScene(int gameType)
    {
        SongSelectionManager.instance.StartCoroutine(SongSelectionManager.instance.fadeOutAudioSources());

        SongData songData = null;
        IResourceLocation songDataAssetLocation = GlobalVariables.Get<IResourceLocation>("activeSongLocation");
        AsyncOperationHandle<SongData> opHandle = Addressables.LoadAssetAsync<SongData>(songDataAssetLocation);
        yield return new WaitUntil(() => opHandle.IsDone);

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            songData = opHandle.Result;
            Addressables.Release(opHandle);
        }
        else
        {
            Debug.LogError("Failed to load song data asset reference: " + songDataAssetLocation);
        }

        GlobalVariables.Set(songData.name + "New", "0");
        if (GlobalVariables.Get<string>("GameType" + gameType + songData.name + "played") == null)
        {
            GlobalVariables.Set("GameType" + gameType + songData.name + "played", "1");
        }
        switch (gameType)
        {
            case 1:
                GlobalVariables.Set("activeGameType", 1);
                DiscordRPCManager.Instance.UpdateActivity(details: "Paw Percussion", start: DateTimeOffset.Now.ToUnixTimeMilliseconds(), largeImageKey: "circlegame", largeImageText: "Kitty Jam Paw Percussion", smallImageKey: songData.DiscordIconKey, smallImageText: songData.SongName + " - " + songData.ArtistName);
                break;
            case 2:
                GlobalVariables.Set("activeGameType", 2);
                DiscordRPCManager.Instance.UpdateActivity(details: "Feline Fretboard", start: DateTimeOffset.Now.ToUnixTimeMilliseconds(), largeImageKey: "highwaygame", largeImageText: "Kitty Jam Feline Fretboard", smallImageKey: songData.DiscordIconKey, smallImageText: songData.SongName + " - " + songData.ArtistName);
                break;
            case 3:
                GlobalVariables.Set("activeGameType", 3);
                DiscordRPCManager.Instance.UpdateActivity(details: "Pouncing Parade", start: DateTimeOffset.Now.ToUnixTimeMilliseconds(), largeImageKey: "marchinggame", largeImageText: "Kitty Jam Pouncing Parade", smallImageKey: songData.DiscordIconKey, smallImageText: songData.SongName + " - " + songData.ArtistName);
                break;
            case 4:
                GlobalVariables.Set("activeGameType", 4);
                DiscordRPCManager.Instance.UpdateActivity(details: "Bongo Bash", start: DateTimeOffset.Now.ToUnixTimeMilliseconds(), largeImageKey: "bongogame", largeImageText: "Kitty Jam Bongo Bash", smallImageKey: songData.DiscordIconKey, smallImageText: songData.SongName + " - " + songData.ArtistName);
                break;
        }
        if (GlobalVariables.Get<string>("GameType" + gameType + "Onboarded") == null)
        {
            GlobalVariables.Set("GameType" + gameType + "Onboarded", "1");
            DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: gameType == 1 ? game1Onboarding : gameType == 2 ? game2Onboarding : gameType == 3 ? game3Onboarding : game4Onboarding);
        }
        else
        {
            switch (gameType)
            {
                case 1:
                    DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game1);
                    break;
                case 2:
                    DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game2);
                    break;
                case 3:
                    DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game3);
                    break;
                case 4:
                    DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game4);
                    break;
            }
        }
    }
}
