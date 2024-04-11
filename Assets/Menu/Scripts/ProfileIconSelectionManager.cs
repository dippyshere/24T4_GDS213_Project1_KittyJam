using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;

public class ProfileIconSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Scene load info to use once the profile icon is selected")] private SceneLoadInfo sceneLoadInfo;
    private bool isSet = false;

    public async void SelectProfileIcon(int profileIconIndex = 1)
    {
        if (isSet) return;
        isSet = true;
        Dictionary<string, object> data = new() { { "profileIndex", profileIconIndex } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));

        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
    }
}
