using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

/// <summary>
/// Handles the win menu UI
/// </summary>
public class WinMenuManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the win menu manager instance")] public static WinMenuManager Instance;
    [Tooltip("Reference to the win menu")] private GameObject winMenu;
    [SerializeField, Tooltip("Reference to the final score display")] private TextMeshProUGUI finalScoreDisplay;
    [SerializeField, Tooltip("Reference to the score tally display")] private TextMeshProUGUI scoreTallyDisplay;

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
    public void EnableWinMenu()
    {
        CursorController.Instance.UnlockCursor();
        winMenu.SetActive(true);
        PauseMenuManager.Instance.OnPauseGameplay?.Invoke(true);
        PauseMenuManager.Instance.EnablePauseEffect();
        finalScoreDisplay.text = "Final Score: " + ScoreManager.Instance.score.ToString("N0", CultureInfo.InvariantCulture);
        scoreTallyDisplay.text = "Perfect: " + ScoreManager.Instance.perfectCount.ToString("N0", CultureInfo.InvariantCulture) + "\nGood: " + ScoreManager.Instance.hitCount.ToString("N0", CultureInfo.InvariantCulture) + "\nMiss: " + ScoreManager.Instance.missCount.ToString("N0", CultureInfo.InvariantCulture);
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
}
