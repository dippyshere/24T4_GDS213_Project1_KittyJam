using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

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
    public void AssetLoadCompleted()
    {
        titleDismissalText.SetActive(true);
        m_EventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
        DownloadManager.Instance.OnTransitionComplete -= AssetLoadCompleted;
    }

    /// <summary>
    /// Handles the button press event
    /// </summary>
    /// <param name="button">The button that was pressed</param>
    async void OnButtonPressed(InputControl button)
    {
        //var device = button.device;

        m_EventListener.Dispose();

        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: onboardSceneLoadInfo);
        }
        else
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
                Debug.Log("Sign in anonymously succeeded!");

                // Shows how to get the playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
                DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: songShelfSceneLoadInfo);
            }
            catch (AuthenticationException ex)
            {
#if UNITY_IOS
                try
                {
                    Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
                }
                catch (Exception)
                {

                }
#elif UNITY_ANDROID
                Vibration.VibrateNope();
#endif
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogError(ex);
            }
            catch (RequestFailedException ex)
            {
#if UNITY_IOS
                try
                {
                    Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
                }
                catch (Exception)
                {

                }
#elif UNITY_ANDROID
                Vibration.VibrateNope();
#endif
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogError(ex);
            }

            try
            {
                GlobalVariables.GetAll().Clear();
                AuthenticationService.Instance.ClearSessionToken();
                AuthenticationService.Instance.SignOut(true);
            }
            catch (Exception)
            {

            }

            DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: onboardSceneLoadInfo);
        }
    }
}
