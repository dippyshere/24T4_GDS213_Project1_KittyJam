using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using TMPro;
using System.Globalization;
using Unity.Services.Leaderboards.Models;
using Cysharp.Threading.Tasks;

/// <summary>
/// Handles the win menu UI
/// </summary>
public class WinMenuManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the win menu manager instance")] public static WinMenuManager Instance;
    [Tooltip("Reference to the win menu")] private GameObject winMenu;
    [SerializeField, Tooltip("Reference to the final score display")] private TextMeshProUGUI finalScoreDisplay;
    [SerializeField, Tooltip("Reference to the score tally display")] private TextMeshProUGUI scoreTallyDisplay;
    [SerializeField, Tooltip("Reference to the leaderborad score submission status")] private TextMeshProUGUI leaderboardSubmissionStatus;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        winMenu = transform.GetChild(0).gameObject;
        winMenu.SetActive(false);
    }

    /// <summary>
    /// Enables the win menu
    /// </summary>
    public async void EnableWinMenu()
    {
        CursorController.Instance.UnlockCursor();
        winMenu.SetActive(true);
        PauseMenuManager.Instance.OnPauseGameplay?.Invoke(true);
        PauseMenuManager.Instance.EnablePauseEffect();
        finalScoreDisplay.text = "Final Score: " + ScoreManager.Instance.score.ToString("N0", CultureInfo.InvariantCulture);
        scoreTallyDisplay.text = "Perfect: " + ScoreManager.Instance.perfectCount.ToString("N0", CultureInfo.InvariantCulture) + "\nGood: " + ScoreManager.Instance.hitCount.ToString("N0", CultureInfo.InvariantCulture) + "\nMiss: " + ScoreManager.Instance.missCount.ToString("N0", CultureInfo.InvariantCulture);
        await SubmitLeaderboardScore();
    }

    /// <summary>
    /// Disables the win menu
    /// </summary>
    public void DisableWinMenu()
    {
        CursorController.Instance.LockCursor();
        winMenu.SetActive(false);
        PauseMenuManager.Instance.OnPauseGameplay?.Invoke(false);
        PauseMenuManager.Instance.DisablePauseEffect();
    }

    /// <summary>
    /// Returns to the main menu
    /// </summary>
    public void ReturnToMenu()
    {
        PauseMenuManager.Instance.ReturnToMenu();
    }

    public async UniTask SubmitLeaderboardScore()
    {
        leaderboardSubmissionStatus.text = "Submitting...";
        SongData songData = SongManager.Instance.songData;
        if (songData == null)
        {
            leaderboardSubmissionStatus.text = "No song data found";
            return;
        }
        LeaderboardEntry playerEntry = null;
        try
        {
            switch (GlobalVariables.Get<int>("activeGameType"))
            {
                case 1:
                    foreach (GameMode gameMode in songData.GameModes)
                    {
                        if (gameMode.GameType == GameType.CircleGame)
                        {
                            if (gameMode.LeaderboardID == null || gameMode.LeaderboardID == "")
                            {
                                leaderboardSubmissionStatus.text = "No leaderboard ID found";
                                return;
                            }
                            playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(gameMode.LeaderboardID, ScoreManager.Instance.score);
                            Debug.Log(JsonConvert.SerializeObject(playerEntry));
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (GameMode gameMode in songData.GameModes)
                    {
                        if (gameMode.GameType == GameType.HighwayGame)
                        {
                            if (gameMode.LeaderboardID == null || gameMode.LeaderboardID == "")
                            {
                                leaderboardSubmissionStatus.text = "No leaderboard ID found";
                                return;
                            }
                            playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(gameMode.LeaderboardID, ScoreManager.Instance.score);
                            Debug.Log(JsonConvert.SerializeObject(playerEntry));
                            break;
                        }
                    }
                    break;
                case 3:
                    foreach (GameMode gameMode in songData.GameModes)
                    {
                        if (gameMode.GameType == GameType.MarchingGame)
                        {
                            if (gameMode.LeaderboardID == null || gameMode.LeaderboardID == "")
                            {
                                leaderboardSubmissionStatus.text = "No leaderboard ID found";
                                return;
                            }
                            playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(gameMode.LeaderboardID, ScoreManager.Instance.score);
                            Debug.Log(JsonConvert.SerializeObject(playerEntry));
                            break;
                        }
                    }
                    break;
                case 4:
                    foreach (GameMode gameMode in songData.GameModes)
                    {
                        if (gameMode.GameType == GameType.BongoGame)
                        {
                            if (gameMode.LeaderboardID == null || gameMode.LeaderboardID == "")
                            {
                                leaderboardSubmissionStatus.text = "No leaderboard ID found";
                                return;
                            }
                            playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(gameMode.LeaderboardID, ScoreManager.Instance.score);
                            Debug.Log(JsonConvert.SerializeObject(playerEntry));
                            break;
                        }
                    }
                    break;
                default:
                    leaderboardSubmissionStatus.text = "No game type found";
                    return;
            }
        }
        catch (Exception e)
        {
            leaderboardSubmissionStatus.text = "Failed to submit score";
            Debug.LogError(e);
            return;
        }
        if (playerEntry == null)
        {
            leaderboardSubmissionStatus.text = "Failed to submit score";
            return;
        }
        leaderboardSubmissionStatus.text = "Score submitted";
    }
}
