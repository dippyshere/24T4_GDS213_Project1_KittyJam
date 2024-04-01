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
    [SerializeField, Tooltip("The scene loading behaviour for the bootstrapper to use")] private SceneLoadInfo bootstrapSceneLoadInfo;
    [Tooltip("List of async operations for downloading scenes")] private List<AsyncOperationHandle<SceneInstance>> downloadOperations = new List<AsyncOperationHandle<SceneInstance>>();
    [Tooltip("Reference to the canvas group controlling the bootstrap canvas")] private CanvasGroup bootstrapCanvasGroup;

    private IEnumerator Start()
    {
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

    private IEnumerator FadeOut()
    {
        bootstrapCanvasGroup.alpha = 1;
        while (bootstrapCanvasGroup.alpha > 0)
        {
            bootstrapCanvasGroup.alpha -= Time.deltaTime * 2;
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

    public static implicit operator string(SceneAssetReference sceneAssetReference)
    {
        return sceneAssetReference.AssetGUID;
    }

    public static implicit operator SceneAssetReference(string guid)
    {
        return new SceneAssetReference(guid);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SceneAssetReference other = (SceneAssetReference)obj;
        return AssetGUID == other.AssetGUID;
    }

    public override int GetHashCode()
    {
        return AssetGUID.GetHashCode();
    }
}
