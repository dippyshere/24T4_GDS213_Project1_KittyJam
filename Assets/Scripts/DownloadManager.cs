using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;
using UnityEngine.ResourceManagement.ResourceProviders;

public delegate void DownloadCallback();

public class DownloadManager : MonoBehaviour
{
    [HideInInspector, Tooltip("The singleton instance for the download manager")] public static DownloadManager Instance;
    [Header("Startup Data")]
    [SerializeField, Tooltip("Scenes to load on startup when the title screen is not yet loaded.")] private SceneLoadInfo startupSceneLoadInfo;
    [SerializeField, Tooltip("Assets to load on startup when the title screen is not yet loaded.")] private AssetLoadInfo startupAssetsToLoad;
    [Header("UI Elements")]
    [SerializeField, Tooltip("Reference to the UI text element containing the download text.")] private TextMeshProUGUI downloadText;
    [SerializeField, Tooltip("Reference to the UI slider element containing the download progress.")] private Slider downloadProgress;
    [SerializeField, Tooltip("Reference to the UI image element containing the cat animation.")] private Image catImage;
    [Tooltip("Reference to the canvas group component to control alpha for all downloader ui elements")] private CanvasGroup canvasGroup;
    [Tooltip("Store whether or not we've already shown the player the downloader message. If we have then we don't need to show them again")] private bool shownText = false;
    [Tooltip("List of scene instances that have been downloaded by the download manager.\n" +
        "Stored as a dictionary with the key being the SceneAssetReference, and the value being the SceneInstance")] private Dictionary<SceneAssetReference, AsyncOperationHandle<SceneInstance>> sceneInstances = new Dictionary<SceneAssetReference, AsyncOperationHandle<SceneInstance>>();
    [Tooltip("List of asset instances that have been downloaded by the download manager.\n" +
               "Stored as a dictionary with the key being the AssetReference, and the value being the AsyncOperationHandle")] private Dictionary<AssetReferenceLoadInfo, AsyncOperationHandle> assetInstances = new Dictionary<AssetReferenceLoadInfo, AsyncOperationHandle>();
    [Tooltip("List of asset instances that have been downloaded by the download manager.\n" +
                      "Stored as a dictionary with the key being the AssetLabelReference, and the value being the AsyncOperationHandle")] private Dictionary<AssetLabelReferenceLoadInfo, AsyncOperationHandle> assetLabelInstances = new Dictionary<AssetLabelReferenceLoadInfo, AsyncOperationHandle>();
    [HideInInspector, Tooltip("The callback to invoke when downloading begins.")] public event DownloadCallback OnDownloadBegin;
    [HideInInspector, Tooltip("The callback to invoke when downloading completes.")] public event DownloadCallback OnDownloadComplete;
    [HideInInspector, Tooltip("The callback to invoke when scene loading begins.")] public event DownloadCallback OnSceneLoadBegin;
    [HideInInspector, Tooltip("The callback to invoke when all loading is complete.")] public event DownloadCallback OnLoadingComplete;
    [HideInInspector, Tooltip("The callback to invoke when the transition completes.")] public event DownloadCallback OnTransitionComplete;
    [HideInInspector, Tooltip("Whether the download manager is currently busy")] public bool isBusy = false;

    private void Start()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        downloadText.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(false);
        catImage.gameObject.SetActive(false);
        canvasGroup.alpha = 0;
        if (SceneManager.GetSceneByName("TitleScreen").isLoaded == false)
        {
            StartCoroutine(DownloadAssets(startupAssetsToLoad, startupSceneLoadInfo));
        }
    }

    public void BeginDownloadAssetsCoroutine(AssetLoadInfo assetLoadInfo = null, SceneLoadInfo sceneLoadInfo = null)
    {
        StartCoroutine(DownloadAssets(assetLoadInfo, sceneLoadInfo));
    }

    /// <summary>
    /// Begins downloading assets and loading scenes based on provided SceneLoadInfo and AssetLoadInfo objects defining the load behaviour.
    /// </summary>
    /// <param name="assetLoadInfo">The list of asset load info objects to download.</param>
    /// <param name="sceneLoadInfo">The list of scene load info objects to load.</param>
    /// <returns>The coroutine for downloading the dependencies.</returns>
    public IEnumerator DownloadAssets(AssetLoadInfo assetLoadInfo = null, SceneLoadInfo sceneLoadInfo = null)
    {
        if (canvasGroup.alpha > 0)
        {
            while (canvasGroup.alpha > 0)
            {
                yield return null;
            }
        }
        while (isBusy)
        {
            yield return null;
        }
        isBusy = true;
        if (!shownText)
        {
            downloadText.gameObject.SetActive(true);
            shownText = true;
        }

        float timeStarted = Time.realtimeSinceStartup;
        downloadProgress.gameObject.SetActive(true);
        downloadProgress.value = 0;
        catImage.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 2;
            yield return null;
        }

        // calculate progress bar values based on number of dependencies and whether we're loading scenes and/or assets
        // if we are just loading scenes or assets, we only need to calculate progress based on the count of assets or scenes we're loading
        // if we're loading both, we need to calculate progress for both using the count of assets for assets and scenes for scenes
        // for loading both, the assets will take up the first 80% of the progress bar, and the scenes will take up the last 20%

        int totalAssetDownloads = 0;
        int totalSceneDownloads = 0;
        float individualAssetProgress = 1;
        float individualSceneProgress = 1;
        if (assetLoadInfo != null)
        {
            totalAssetDownloads = assetLoadInfo.assetsToLoad.Count + assetLoadInfo.assetLabelsToLoad.Count;
            if (sceneLoadInfo != null && sceneLoadInfo.scenesToLoad.Count > 0)
            {
                individualAssetProgress = 0.8f / totalAssetDownloads;
            }
            else
            {
                individualAssetProgress = 1f / totalAssetDownloads;
            }
        }
        if (sceneLoadInfo != null)
        {
            totalSceneDownloads = sceneLoadInfo.scenesToLoad.Count;
            if (assetLoadInfo != null && totalAssetDownloads > 0)
            {
                individualSceneProgress = 0.2f / totalSceneDownloads;
            }
            else
            {
                individualSceneProgress = 1f / totalSceneDownloads;
            }

            if (sceneLoadInfo.useTransition && TransitionManager.Instance != null)
            {
                TransitionManager.Instance.StartTransition();
                timeStarted = Time.realtimeSinceStartup;
            }
        }

        OnDownloadBegin?.Invoke();

        // download assets
        List<AsyncOperationHandle> downloadAssetOperations = new List<AsyncOperationHandle>();
        List<AsyncOperationHandle> downloadAssetLabelOperations = new List<AsyncOperationHandle>();
        if (assetLoadInfo != null)
        {
            foreach (AssetReferenceLoadInfo loadInfo in assetLoadInfo.assetsToLoad)
            {
                AsyncOperationHandle asyncOperationHandle = Addressables.DownloadDependenciesAsync(loadInfo.assetReference);
                downloadAssetOperations.Add(asyncOperationHandle);
                assetInstances.Add(loadInfo, asyncOperationHandle);
            }
            foreach (AssetLabelReferenceLoadInfo loadInfo in assetLoadInfo.assetLabelsToLoad)
            {
                AsyncOperationHandle asyncOperationHandle = Addressables.DownloadDependenciesAsync(loadInfo.assetLabel);
                downloadAssetLabelOperations.Add(asyncOperationHandle);
                assetLabelInstances.Add(loadInfo, asyncOperationHandle);
            }

            while (true)
            {
                float totalProgress = 0f;
                foreach (var operation in downloadAssetOperations)
                {
                    totalProgress += operation.PercentComplete * individualAssetProgress;
                }
                foreach (var operation in downloadAssetLabelOperations)
                {
                    totalProgress += operation.PercentComplete * individualAssetProgress;
                }

                downloadProgress.value = Mathf.Min(totalProgress, 1f);

                bool allDone = downloadAssetOperations.TrueForAll(operation => operation.IsDone) && downloadAssetLabelOperations.TrueForAll(operation => operation.IsDone);
                if (allDone)
                {
                    break;
                }

                yield return null;
            }

            List<AssetReferenceLoadInfo> pendingAssetReferencesToRemove = new List<AssetReferenceLoadInfo>();
            foreach (var operation in downloadAssetOperations)
            {
                foreach (AssetReferenceLoadInfo loadInfo in assetLoadInfo.assetsToLoad)
                {
                    if (operation.Equals(assetInstances[loadInfo]) && operation.IsValid() && loadInfo.releaseAssetsAfterDownload)
                    {
                        Addressables.Release(operation);
                        pendingAssetReferencesToRemove.Add(loadInfo);
                    }
                    else if (operation.Equals(assetInstances[loadInfo]) && !operation.IsValid())
                    {
                        pendingAssetReferencesToRemove.Add(loadInfo);
                    }
                }
            }

            foreach (AssetReferenceLoadInfo loadInfo in pendingAssetReferencesToRemove)
            {
                assetInstances.Remove(loadInfo);
            }

            List<AssetLabelReferenceLoadInfo> pendingAssetLabelReferencesToRemove = new List<AssetLabelReferenceLoadInfo>();
            foreach (var operation in downloadAssetLabelOperations)
            {
                foreach (AssetLabelReferenceLoadInfo loadInfo in assetLoadInfo.assetLabelsToLoad)
                {
                    if (operation.Equals(assetLabelInstances[loadInfo]) && operation.IsValid() && loadInfo.releaseAssetsAfterDownload)
                    {
                        Addressables.Release(operation);
                        pendingAssetLabelReferencesToRemove.Add(loadInfo);
                    }
                    else if (operation.Equals(assetLabelInstances[loadInfo]) && !operation.IsValid())
                    {
                        pendingAssetLabelReferencesToRemove.Add(loadInfo);
                    }
                }
            }

            foreach (AssetLabelReferenceLoadInfo loadInfo in pendingAssetLabelReferencesToRemove)
            {
                assetLabelInstances.Remove(loadInfo);
            }
        }

        OnDownloadComplete?.Invoke();
        OnSceneLoadBegin?.Invoke();

        // download scenes
        List<AsyncOperationHandle> downloadSceneOperations = new List<AsyncOperationHandle>();
        if (sceneLoadInfo != null)
        {
            if (sceneLoadInfo.useTransition && TransitionManager.Instance != null)
            {
                while (Time.realtimeSinceStartup - timeStarted < 1.05f)
                {
                    yield return null;
                }
            }

            foreach (SceneAssetReference scene in sceneLoadInfo.scenesToLoad)
            {
                AsyncOperationHandle<SceneInstance> asyncOperationHandle = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive, true, -100);
                downloadSceneOperations.Add(asyncOperationHandle);
                sceneInstances.Add(scene, asyncOperationHandle);
                if (sceneLoadInfo.markFirstSceneAsActive && scene.Equals(sceneLoadInfo.scenesToLoad[0]))
                {
                    asyncOperationHandle.Completed += operation =>
                    {
                        SceneInstance sceneInstance = operation.Result;
                        if (sceneInstance.Scene.IsValid())
                        {
                            SceneManager.SetActiveScene(sceneInstance.Scene);
                        }
                    };
                }
            }

            while (true)
            {
                float totalProgress = 0f;
                if (totalAssetDownloads > 0)
                {
                    totalProgress = 0.8f;
                }
                foreach (var operation in downloadSceneOperations)
                {
                    totalProgress += operation.PercentComplete * individualSceneProgress;
                }

                downloadProgress.value = Mathf.Min(totalProgress, 1f);

                bool allDone = downloadSceneOperations.TrueForAll(operation => operation.IsDone);
                if (allDone)
                {
                    break;
                }

                yield return null;
            }

            List<AsyncOperationHandle<SceneInstance>> unloadOperationHandles = new List<AsyncOperationHandle<SceneInstance>>();
            List<SceneAssetReference> pendingSceneReferencesToRemove = new List<SceneAssetReference>();
            foreach (SceneAssetReference sceneAsset in sceneLoadInfo.scenesToUnload)
            {
                foreach (var sceneInstance in sceneInstances)
                {
                    if (sceneInstance.Key.Equals(sceneAsset) && sceneInstance.Value.Result.Scene.isLoaded)
                    {
                        Debug.Log("Unloading scene instance: " + sceneInstance.Key);
                        AsyncOperationHandle<SceneInstance> unloadOperationHandle = Addressables.UnloadSceneAsync(sceneInstance.Value, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, false);
                        unloadOperationHandles.Add(unloadOperationHandle);
                        unloadOperationHandle.Completed += operation =>
                        {
                            if (operation.Status == AsyncOperationStatus.Succeeded)
                            {
                                Addressables.Release(operation);
                                pendingSceneReferencesToRemove.Add(sceneAsset);
                            }
                            else
                            {
                                Debug.LogError("Failed to unload scene instance: " + sceneInstance.Key);
                            }
                        };
                    }
                }
            }

            while (true)
            {
                bool allDone = unloadOperationHandles.TrueForAll(operation => operation.IsDone);
                if (allDone)
                {
                    break;
                }
                else
                {
                    Debug.Log("Waiting for scene unloading to complete...");
                    yield return null;
                }
            }

            foreach (SceneAssetReference sceneAsset in pendingSceneReferencesToRemove)
            {
                sceneInstances.Remove(sceneAsset);
            }

            if (sceneLoadInfo.useTransition && TransitionManager.Instance != null)
            {
                while (Time.realtimeSinceStartup - timeStarted < 1.05f)
                {
                    yield return null;
                }
                for (int i = 0; i < 5; i++)
                {
                    yield return null;
                }
                TransitionManager.Instance.EndTransition();
            }

            downloadProgress.value = 1;
        }

        OnLoadingComplete?.Invoke();

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2;
            yield return null;
        }

        downloadText.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(false);
        catImage.gameObject.SetActive(false);

        OnTransitionComplete?.Invoke();
        isBusy = false;
    }

    /// <summary>
    /// Adds a list of scene instances to the download manager's list of scene instances to keep track of scene instances and unload them when necessary.
    /// </summary>
    /// <param name="instances">The list of scene instances to add to the download manager's list of scene instances.</param>
    public void AddSceneInstances(Dictionary<SceneAssetReference, AsyncOperationHandle<SceneInstance>> instances)
    {
        foreach (var instance in instances)
        {
            sceneInstances.Add(instance.Key, instance.Value);
        }
    }

    /// <summary>
    /// Unloads a list of scene instances from the download manager's list of scene instances to reduce asset reference count and free up memory.
    /// </summary>
    /// <param name="scenesToRelease">The list of scene instances to release from the download manager's list of scene instances.</param>
    public void ReleaseScenes(List<SceneAssetReference> scenesToRelease)
    {
        foreach (SceneAssetReference scene in scenesToRelease)
        {
            if (sceneInstances.ContainsKey(scene))
            {
                Addressables.UnloadSceneAsync(sceneInstances[scene]);
                sceneInstances.Remove(scene);
            }
        }
    }
}
