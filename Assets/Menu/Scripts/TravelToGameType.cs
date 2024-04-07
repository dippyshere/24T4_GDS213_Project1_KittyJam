using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the travel to game type
/// </summary>
public class TravelToGameType : MonoBehaviour
{
    [SerializeField] private SceneLoadInfo game1;
    [SerializeField] private SceneLoadInfo game2;
    [SerializeField] private SceneLoadInfo game3;
    [SerializeField] private SceneLoadInfo game4;

    /// <summary>
    /// Load game 1
    /// </summary>
    public void LoadGame1()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game1);
    }

    /// <summary>
    /// Load game 2
    /// </summary>
    public void LoadGame2()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game2);
    }

    /// <summary>
    /// Load game 3
    /// </summary>
    public void LoadGame3()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game3);
    }

    /// <summary>
    /// Load game 4
    /// </summary>
    public void LoadGame4()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game4);
    }
}
