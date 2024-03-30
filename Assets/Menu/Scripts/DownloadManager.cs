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
            StartCoroutine(DownloadDependencies(new List<string> { "initialdownload", "fonts", "Assets/Scenes/Frontend/TitleScreen.unity" }, LoadMainMenu));
        }
    }

    /// <summary>
    /// Downloads the dependencies for the given list of addresses and/or labels, showing progress + animation for the player. Runs the callback when complete.
    /// </summary>
    /// <param name="address">The address or label to download dependencies for.</param>
    /// <param name="callback">The callback to run when the download is complete.</param>
    /// <returns>The coroutine for downloading the dependencies.</returns>
    public IEnumerator DownloadDependencies(List<string> addresses, System.Action<List<AsyncOperationHandle>> callback = null)
    {
        if (!shownText)
        {
            downloadText.gameObject.SetActive(true);
            shownText = true;
        }
        downloadProgress.gameObject.SetActive(true);
        catImage.gameObject.SetActive(true);

        float totalProgress = 0f;
        float individualProgress = 1f / addresses.Count;

        List<AsyncOperationHandle> downloadOperations = new List<AsyncOperationHandle>();
        foreach (string address in addresses)
        {
            downloadOperations.Add(Addressables.DownloadDependenciesAsync(address));
        }

        while (totalProgress < 1f)
        {
            totalProgress = 0f;
            foreach (var operation in downloadOperations)
            {
                totalProgress += operation.PercentComplete * individualProgress;
            }
            downloadProgress.value = totalProgress;
            yield return null;
        }

        downloadProgress.value = 1f;
        downloadText.gameObject.SetActive(false);
        downloadProgress.gameObject.SetActive(false);
        catImage.gameObject.SetActive(false);

        if (callback != null)
        {
            callback(downloadOperations);
        }
    }

    private void LoadMainMenu(List<AsyncOperationHandle> operations)
    {
        Addressables.LoadSceneAsync("Assets/Scenes/Frontend/TitleScreen.unity", LoadSceneMode.Additive);
    }
}
