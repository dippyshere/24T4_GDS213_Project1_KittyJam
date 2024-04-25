using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary>
/// The bootstrapper is responsible for downloading the base scene and any additional scenes before launching the game.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [SerializeField, Tooltip("The scene loading behaviour for the bootstrapper to use")] private SceneLoadInfo bootstrapSceneLoadInfo;
    [Tooltip("List of async operations for downloading scenes")] private List<AsyncOperationHandle<SceneInstance>> downloadOperations = new List<AsyncOperationHandle<SceneInstance>>();
    [Tooltip("Reference to the canvas group controlling the bootstrap canvas")] private CanvasGroup bootstrapCanvasGroup;

    private IEnumerator Start()
    {
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = Mathf.CeilToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }
        else
        {
            Application.targetFrameRate = -1;
        }

        InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
        InputSystem.settings.SetInternalFeatureFlag("USE_READ_VALUE_CACHING", true);

        bootstrapCanvasGroup = GetComponent<CanvasGroup>();
        bootstrapCanvasGroup.alpha = 1;

        foreach (SceneAssetReference scene in bootstrapSceneLoadInfo.scenesToLoad)
        {
            downloadOperations.Add(Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }

        if (bootstrapSceneLoadInfo.markFirstSceneAsActive)
        {
            downloadOperations[0].Completed += operation =>
            {
                SceneInstance sceneInstance = operation.Result;
                if (sceneInstance.Scene.IsValid())
                {
                    SceneManager.SetActiveScene(sceneInstance.Scene);
                }
            };
        }

        yield return new WaitUntil(() => downloadOperations.TrueForAll(operation => operation.IsDone));

        if (DownloadManager.Instance != null)
        {
            Dictionary<SceneAssetReference, AsyncOperationHandle<SceneInstance>> sceneInstances = new Dictionary<SceneAssetReference, AsyncOperationHandle<SceneInstance>> ();
            for (int i = 0; i < bootstrapSceneLoadInfo.scenesToLoad.Count; i++)
            {
                sceneInstances.Add(bootstrapSceneLoadInfo.scenesToLoad[i], downloadOperations[i]);
            }
            DownloadManager.Instance.AddSceneInstances(sceneInstances);
        }

        if (bootstrapSceneLoadInfo.useTransition)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }

        if (bootstrapSceneLoadInfo.scenesToUnload.Count > 0)
        {
            DownloadManager.Instance.ReleaseScenes(bootstrapSceneLoadInfo.scenesToUnload);
        }
    }

    /// <summary>
    /// Fades out the bootstrap canvas group before unloading the bootstrap scene
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator FadeOut()
    {
        bootstrapCanvasGroup.alpha = 1;
        while (bootstrapCanvasGroup.alpha > 0)
        {
            bootstrapCanvasGroup.alpha -= Time.unscaledDeltaTime * 2;
            yield return null;
        }
        SceneManager.UnloadSceneAsync(gameObject.scene);
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

    /// <summary>
    /// Returns the GUID of the scene asset reference
    /// </summary>
    /// <param name="sceneAssetReference">The scene asset reference to get the GUID of</param>
    public static implicit operator string(SceneAssetReference sceneAssetReference)
    {
        return sceneAssetReference.AssetGUID;
    }

    /// <summary>
    /// Returns the scene asset reference from a GUID
    /// </summary>
    /// <param name="guid">The GUID to get the scene asset reference from</param>
    public static implicit operator SceneAssetReference(string guid)
    {
        return new SceneAssetReference(guid);
    }

    /// <summary>
    /// Handles the equality comparison between two SceneAssetReference objects
    /// </summary>
    /// <param name="obj">The object to compare against</param>
    /// <returns>The result of the equality comparison</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SceneAssetReference other = (SceneAssetReference)obj;
        return AssetGUID == other.AssetGUID;
    }

    /// <summary>
    /// Handles the hash code generation for the SceneAssetReference object
    /// </summary>
    /// <returns>The hash code for the SceneAssetReference object</returns>
    public override int GetHashCode()
    {
        return AssetGUID.GetHashCode();
    }
}
