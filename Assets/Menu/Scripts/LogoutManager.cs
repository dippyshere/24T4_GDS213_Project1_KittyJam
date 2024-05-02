using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;

public class LogoutManager : MonoBehaviour
{
    [SerializeField, Tooltip("Scene info to use when going back")] private SceneLoadInfo backSceneLoadInfo;
    [SerializeField, Tooltip("Scene info to use when logging out")] private SceneLoadInfo logoutSceneLoadInfo;

    public void Logout()
    {
        try
        {
            AuthenticationService.Instance.SignOut(true);
            AuthenticationService.Instance.ClearSessionToken();
            GlobalVariables.GetAll().Clear();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to sign out: " + e.Message);
        }
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: logoutSceneLoadInfo);
    }

    public void Back()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: backSceneLoadInfo);
    }
}
