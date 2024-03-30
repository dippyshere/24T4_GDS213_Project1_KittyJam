using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary>
/// The bootstrapper is responsible for downloading the base scene and any additional scenes before launching the game.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [SerializeField, Tooltip("The base scene to download and launch into")] private SceneAssetReference baseSceneToDownload;
    [SerializeField, Tooltip("Any additional scenes to download and wait for")] private SceneAssetReference[] scenesToDownload;
    [Tooltip("List of async operations for downloading scenes")] private List<AsyncOperationHandle> downloadOperations = new List<AsyncOperationHandle>();

    private IEnumerator Start()
    {
        downloadOperations.Add(Addressables.LoadSceneAsync(baseSceneToDownload, LoadSceneMode.Additive));
        foreach (SceneAssetReference scene in scenesToDownload)
        {
            downloadOperations.Add(Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }
        yield return new WaitUntil(() => downloadOperations.TrueForAll(operation => operation.IsDone));

        SceneInstance sceneInstance = downloadOperations[0].Convert<SceneInstance>().Result;
        if (sceneInstance.Scene.IsValid())
        {
            SceneManager.SetActiveScene(sceneInstance.Scene);
        }

        SceneManager.UnloadSceneAsync(gameObject.scene);
    }

    private void OnDestroy()
    {
        foreach (var operation in downloadOperations)
        {
            Addressables.Release(operation);
        }
    }
}

/// <summary>
/// This class is used to reference a scene asset in the project.
/// </summary>
[System.Serializable]
public class SceneAssetReference : AssetReference
{
    public SceneAssetReference(string guid) : base(guid) { }

#if UNITY_EDITOR
    public override bool ValidateAsset(string path)
    {
        return path.EndsWith(".unity");
    }
#endif
}