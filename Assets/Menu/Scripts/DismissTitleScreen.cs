using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField, Tooltip("The scene load configuration to use when dismissing the title screen")] private SceneLoadInfo titlescreenSceneLoadInfo;
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

    public void AssetLoadCompleted()
    {
        titleDismissalText.SetActive(true);
        m_EventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
        DownloadManager.Instance.OnTransitionComplete -= AssetLoadCompleted;
    }

    void OnButtonPressed(InputControl button)
    {
        //var device = button.device;

        m_EventListener.Dispose();

        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: titlescreenSceneLoadInfo);
    }
}
