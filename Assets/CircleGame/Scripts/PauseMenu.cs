using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Manages the pause menu
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the cursor controller object to control the cursor when pausing/unpausing")] private CursorController cursorController;
    [SerializeField, Tooltip("Reference to the global post processing volume")] private Volume postProcessVolume;
    [SerializeField, Tooltip("The post processing profile to use when unpaused")] private VolumeProfile defaultPostProcess;
    [SerializeField, Tooltip("The post processing profile to use when paused (To enable effects like DOF blur when paused)")] private VolumeProfile upgradePostProcess;
    [SerializeField, Tooltip("Reference to the player/arm controller to disable movement while paused")] private ArmController armController;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = FindObjectOfType<GameManager>();
        cursorController.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        armController.canSlam = false;
    }

    /// <summary>
    /// When the pause menu is enabled, unlock the cursor and pause the game
    /// </summary>
    private void OnEnable()
    {
        cursorController.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        armController.canSlam = false;
        armController.canMove = false;
        if (SongManager.Instance != null)
        {
            SongManager.Instance.PauseMusic(true);
        }
    }

    /// <summary>
    /// When the pause menu is disabled, lock the cursor and unpause the game
    /// </summary>
    private void OnDisable()
    {
        cursorController.LockCursor();
        Time.timeScale = 1f;
        postProcessVolume.profile = defaultPostProcess;
        armController.canSlam = true;
        armController.canMove = true;
        if (SongManager.Instance != null)
        {
            SongManager.Instance.PauseMusic(false);
        }
    }

    /// <summary>
    /// Pause or unpause the game
    /// </summary>
    /// <param name="pause">True to pause the game, false to unpause the game</param>
    public void PauseAction(bool pause)
    {
        if (pause)
        {
            cursorController.UnlockCursor();
            Time.timeScale = 0f;
            postProcessVolume.profile = upgradePostProcess;
            armController.canSlam = false;
            armController.canMove = false;
        }
        else
        {
            cursorController.LockCursor();
            Time.timeScale = 1f;
            postProcessVolume.profile = defaultPostProcess;
            armController.canSlam = true;
            armController.canMove = true;
        }
    }
}
