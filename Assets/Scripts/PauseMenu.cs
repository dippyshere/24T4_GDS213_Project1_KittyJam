using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Manages the pause menu
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the global post processing volume")] private Volume postProcessVolume;
    [SerializeField, Tooltip("The post processing profile to use when unpaused")] private VolumeProfile defaultPostProcess;
    [SerializeField, Tooltip("The post processing profile to use when paused (To enable effects like DOF blur when paused)")] private VolumeProfile upgradePostProcess;
    public delegate void PauseGameplay(bool pause);
    public static PauseGameplay OnPauseGameplay;

    /// <summary>
    /// When the pause menu is enabled, unlock the cursor and pause the game
    /// </summary>
    private void OnEnable()
    {
        CursorController.Instance.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        OnPauseGameplay?.Invoke(true);
    }

    /// <summary>
    /// When the pause menu is disabled, lock the cursor and unpause the game
    /// </summary>
    private void OnDisable()
    {
        CursorController.Instance.LockCursor();
        Time.timeScale = 1f;
        postProcessVolume.profile = defaultPostProcess;
        OnPauseGameplay?.Invoke(false);
    }
}
