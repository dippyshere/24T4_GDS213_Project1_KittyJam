using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelToGameType : MonoBehaviour
{
    [SerializeField] private SceneLoadInfo game1;
    [SerializeField] private SceneLoadInfo game2;
    [SerializeField] private SceneLoadInfo game3;
    [SerializeField] private SceneLoadInfo game4;

    public void LoadGame1()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game1);
    }

    public void LoadGame2()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game2);
    }

    public void LoadGame3()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game3);
    }

    public void LoadGame4()
    {
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: game4);
    }
}
