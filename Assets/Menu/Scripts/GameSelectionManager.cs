using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using TMPro;

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

    /// <summary>
    /// Handles the update of the game selection screen
    /// </summary>
    public IEnumerator UpdateGameSelectionScreen()
    {
        if (PersistentData.Instance.SelectedSongAssetLocation != null)
        {
            SongData songData = null;
            IResourceLocation songDataAssetLocation = PersistentData.Instance.SelectedSongAssetLocation;
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
        IResourceLocation songDataAssetLocation = PersistentData.Instance.SelectedSongAssetLocation;
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
