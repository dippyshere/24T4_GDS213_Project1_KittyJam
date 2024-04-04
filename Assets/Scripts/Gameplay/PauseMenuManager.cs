using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public delegate void PauseGameplay(bool pause);

public class PauseMenuManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the pause menu manager")] public static PauseMenuManager Instance;
    [Tooltip("Reference to the pause menu")] private GameObject pauseMenu;
    [HideInInspector, Tooltip("Reference to the global post processing volume")] private Volume postProcessVolume;
    [SerializeField, Tooltip("The post processing profile to use when unpaused")] private VolumeProfile defaultPostProcess;
    [SerializeField, Tooltip("The post processing profile to use when paused (To enable effects like DOF blur when paused)")] private VolumeProfile upgradePostProcess;
    [HideInInspector, Tooltip("Callback to call when the game is paused or unpaused via the pause menu")] public PauseGameplay OnPauseGameplay;
    [SerializeField, Tooltip("The level data to use when transitioning to the menu")] private SceneLoadInfo menuLevelData;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        pauseMenu = transform.GetChild(0).gameObject;
        yield return new WaitUntil(() => PostProcessManager.Instance != null);
        postProcessVolume = PostProcessManager.Instance.postProcessVolume;
    }

    public void TogglePauseState(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Time.timeScale == 0f)
            {
                DisablePause();
            }
            else
            {
                EnablePause();
            }
        }
    }

    public void EnablePause()
    {
        CursorController.Instance.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        OnPauseGameplay?.Invoke(true);
        pauseMenu.SetActive(true);
    }

    public void DisablePause()
    {
        CursorController.Instance.LockCursor();
        Time.timeScale = 1f;
        postProcessVolume.profile = defaultPostProcess;
        OnPauseGameplay?.Invoke(false);
        pauseMenu.SetActive(false);
    }

    public void EnablePauseEffect()
    {
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
    }

    public void DisablePauseEffect()
    {
        postProcessVolume.profile = defaultPostProcess;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: menuLevelData);
    }
}
