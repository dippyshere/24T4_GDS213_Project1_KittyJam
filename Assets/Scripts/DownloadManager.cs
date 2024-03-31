using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;

public class DownloadManager : MonoBehaviour
{
    [HideInInspector, Tooltip("The singleton instance for the download manager")] public static DownloadManager Instance;
    [SerializeField, Tooltip("Scenes to load on startup when the title screen is not yet loaded.")] private SceneAssetReference[] scenesToLoad;
    [SerializeField, Tooltip("Reference to the UI text element containing the download text.")] private TextMeshProUGUI downloadText;
    [SerializeField, Tooltip("Reference to the UI slider element containing the download progress.")] private Slider downloadProgress;
    [SerializeField, Tooltip("Reference to the UI image element containing the cat animation.")] private Image catImage;
    private bool shownText = false;

    private void Start()
    {
        Instance = this;
        downloadText.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(false);
        catImage.gameObject.SetActive(false);
        if (SceneManager.GetSceneByName("TitleScreen").isLoaded == false)
        {
            StartCoroutine(DownloadDependencies(new List<string> { "initialdownload", "fonts" }, true));
        }
    }

    /// <summary>
    /// Downloads the dependencies for the given list of addresses and/or labels, showing progress + animation for the player.
    /// </summary>
    /// <param name="addresses">The address or label to download dependencies for.</param>
    /// <param name="isInitialLoad">Whether this is the initial load of the game, if so then the title screen is loaded and activated.</param>
    /// <returns>The coroutine for downloading the dependencies.</returns>
    public IEnumerator DownloadDependencies(List<string> addresses, bool isInitialLoad = false)
    {
        if (!shownText)
        {
            downloadText.gameObject.SetActive(true);
            shownText = true;
        }
        downloadProgress.gameObject.SetActive(true);
        catImage.gameObject.SetActive(true);

        int totalDependencies = addresses.Count;

        float individualProgress;
        if (isInitialLoad)
        {
            totalDependencies += 2;
            individualProgress = 1f / totalDependencies;
        }
        else
        {
            individualProgress = 1f / totalDependencies;
        }

        List<AsyncOperationHandle> downloadOperations = new List<AsyncOperationHandle>();
        foreach (string address in addresses)
        {
            downloadOperations.Add(Addressables.DownloadDependenciesAsync(address));
        }

        while (true)
        {
            float totalProgress = 0f;
            foreach (var operation in downloadOperations)
            {
                totalProgress += operation.PercentComplete * individualProgress;
            }

            downloadProgress.value = Mathf.Min(totalProgress, 1f);

            bool allDone = downloadOperations.TrueForAll(operation => operation.IsDone);
            if (allDone)
            {
                break;
            }

            yield return null;
        }

        foreach (var operation in downloadOperations)
        {
            Addressables.Release(operation);
        }

        if (isInitialLoad)
        {
            List<AsyncOperationHandle> mainSceneDownloadOperations = new List<AsyncOperationHandle>();
            foreach (SceneAssetReference scene in scenesToLoad)
            {
                mainSceneDownloadOperations.Add(Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive));
            }

            while (true)
            {
                float totalProgress = 0f;
                foreach (var operation in mainSceneDownloadOperations)
                {
                    totalProgress += operation.PercentComplete * individualProgress;
                }

                downloadProgress.value = Mathf.Min(totalProgress, 1f);

                bool allDone = mainSceneDownloadOperations.TrueForAll(operation => operation.IsDone);
                if (allDone)
                {
                    break;
                }

                yield return null;
            }

            //foreach (var operation in mainSceneDownloadOperations)
            //{
            //    Addressables.Release(operation);
            //}

        }

        downloadText.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(false);
        catImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Downloads & optionally loads a list of scenes asynchronously, showing progress + animation for the player. Runs the callback when complete.
    /// </summary>
    /// <param name="scenesToLoad">The list of scenes to load in the format "SceneName".</param>
    /// <param name="callback">The callback to run when the scenes are loaded.</param>
    /// <returns>The coroutine for loading the scenes.</returns>
    public IEnumerator DownloadScenes(List<string> scenesToLoad, System.Action<List<string>> callback = null, List<string> callbackData = null)
    {
        float timeStarted = Time.realtimeSinceStartup;
        downloadProgress.gameObject.SetActive(true);
        catImage.gameObject.SetActive(true);

        float individualProgress = 1 / scenesToLoad.Count;

        List<AsyncOperationHandle> downloadOperations = new List<AsyncOperationHandle>();
        foreach (string address in scenesToLoad)
        {
            downloadOperations.Add(Addressables.LoadSceneAsync(address, LoadSceneMode.Additive, true, -100));
        }

        while (true)
        {
            float totalProgress = 0f;
            foreach (var operation in downloadOperations)
            {
                totalProgress += operation.PercentComplete * individualProgress;
            }

            downloadProgress.value = Mathf.Min(totalProgress, 1f);

            bool allDone = downloadOperations.TrueForAll(operation => operation.IsDone);
            if (allDone)
            {
                break;
            }

            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenesToLoad[0]));

        downloadProgress.gameObject.SetActive(false);
        catImage.gameObject.SetActive(false);

        if (callback != null)
        {
            while (Time.realtimeSinceStartup - timeStarted < 1)
            {
                yield return null;
            }
            callback(callbackData);
        }
    }
}
