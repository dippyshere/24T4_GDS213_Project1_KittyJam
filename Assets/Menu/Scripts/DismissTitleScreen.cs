using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;

/// <summary>
/// Dismisses the title screen when any button is pressed or tapped
/// </summary>
public class DismissTitleScreen : MonoBehaviour
{
    [Tooltip("The event listener for any button press")] private IDisposable m_EventListener;
    [Header("Asset and scene configuration")]
    [SerializeField, Tooltip("The assets to download before allowing the player to dismiss the title screen")] private AssetLoadInfo titlescreenAssetLoadInfo;
    [SerializeField, Tooltip("The scene load configuration to use when dismissing the title screen when already logged in")] private SceneLoadInfo songShelfSceneLoadInfo;
    [SerializeField, Tooltip("The scene load configuration to use when dismissing the title screen when not logged in")] private SceneLoadInfo onboardSceneLoadInfo;
    [Header("References")]
    [SerializeField, Tooltip("Reference to the dismissal text prompt")] private GameObject titleDismissalText;
    [Tooltip("Whether the FTUE will be shown or not")] private bool showFTUE = true;

    private IEnumerator Start()
    {
        titleDismissalText.SetActive(false);
        if (DownloadManager.Instance == null)
        {
            Debug.LogError("DownloadManager is not present in the scene. Dismissing the title screen will not work.");
            yield break;
        }
        while (DownloadManager.Instance.isBusy)
        {
            yield return null;
        }
        DownloadManager.Instance.OnTransitionComplete += AssetLoadCompleted;
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(titlescreenAssetLoadInfo);
    }

    /// <summary>
    /// Called when the title screen assets have been loaded
    /// </summary>
    public async void AssetLoadCompleted()
    {
        DownloadManager.Instance.OnTransitionComplete -= AssetLoadCompleted;
        GlobalVariables.GetAll().Clear();
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        try
        {
            Dictionary<string, Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "profileIndex" }, new LoadOptions(new PublicReadAccessClassOptions()));
            if (playerData.TryGetValue("profileIndex", out Item keyName))
            {
                if (keyName.Value.GetAs<int>() > 0)
                {
                    showFTUE = false;
#if UNITY_IOS
                    try
                    {
                        Vibration.VibrateIOS(NotificationFeedbackStyle.Success);
                    }
                    catch (Exception)
                    {

                    }
#elif UNITY_ANDROID
                    Vibration.VibratePeek();
#endif
                }
                else
                {
                    Debug.Log("Showing FTUE because profile index is 0");
                    showFTUE = true;
                }
            }
            else
            {
                Debug.Log("Showing FTUE because profile index is not present");
                showFTUE = true;
            }
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError(ex);
            showFTUE = true;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError(ex);
            showFTUE = true;
        }
        if (showFTUE)
        {
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to sign in: " + e.Message);
            }
        }
        titleDismissalText.SetActive(true);
        m_EventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
    }

    /// <summary>
    /// Handles the button press event
    /// </summary>
    /// <param name="button">The button that was pressed</param>
    void OnButtonPressed(InputControl button)
    {
        //var device = button.device;

        m_EventListener.Dispose();

        if (showFTUE)
        {
            DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: onboardSceneLoadInfo);
        }
        else
        {
            DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: songShelfSceneLoadInfo);
        }
    }
}
