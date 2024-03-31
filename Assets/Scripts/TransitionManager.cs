using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

/// <summary>
/// Manages the transitions between scenes
/// </summary>
public class TransitionManager : MonoBehaviour
{
    [HideInInspector, Tooltip("The singleton instance for the menu manager")] public static TransitionManager Instance;
    [SerializeField, Tooltip("The animator that controls the circle wipe")] private Animator circleTransitionAnimator;
    [SerializeField, Tooltip("The animator that controls the logo")] private Animator logoTransitionAnimator;
    [Tooltip("Timestamp that the transition began at")] private float transitionStartTime;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Transitions to a scene, without playing/stopping music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicContinue(string sceneToLoad)
    {
        StartTransitionAndLoadScenes(new List<string> { "Assets/Scenes/" + sceneToLoad + ".unity" }, true, new List<string> { "Assets/Scenes/Frontend/SongShelf.unity" });
    }

    /// <summary>
    /// Transitions to a scene, starting the music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicStart(string sceneToLoad)
    {
        StartTransitionAndLoadScenes(new List<string> { "Assets/Scenes/" + sceneToLoad + ".unity" }, true, new List<string> { "Assets/Scenes/GameType1CircleGame.unity" });
    }

    /// <summary>
    /// Transitions to a scene, stopping the music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicStop(string sceneToLoad)
    {
        StartTransitionAndLoadScenes(new List<string> { "Assets/Scenes/" + sceneToLoad + ".unity" }, true, new List<string> { "Assets/Scenes/GameType1Onboarding.unity"});
    }

    public void LoadScenes(List<string> scenesToLoad)
    {
        StartCoroutine(DownloadAndLoadScenes(scenesToLoad, false));
    }

    public void StartTransitionAndLoadScenes(List<string> scenesToLoad, bool isTransitioning = true, List<string> scenesToUnload = null)
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");

        StartCoroutine(DownloadAndLoadScenes(scenesToLoad, isTransitioning, scenesToUnload));
    }

    private IEnumerator DownloadAndLoadScenes(List<string> scenesToLoad, bool isTransitioning = true, List<string> scenesToUnload = null)
    {
        yield return new WaitForSecondsRealtime(1.05f);

        if (isTransitioning)
        {
            yield return StartCoroutine(DownloadManager.Instance.DownloadScenes(scenesToLoad, EndTransition, scenesToUnload));
        }
        else
        {
            yield return StartCoroutine(DownloadManager.Instance.DownloadScenes(scenesToLoad, null, scenesToUnload));
        }
    }

    public void EndTransition(List<string> scenesToUnload = null)
    {
        circleTransitionAnimator.SetTrigger("End");
        logoTransitionAnimator.SetTrigger("End");
    }

    public void UnloadScenesAndEndTransition(List<string> scenesToUnload = null)
    {
        if (scenesToUnload != null && scenesToUnload.Count > 0)
        {
            UnloadScenes(scenesToUnload);
        }
        else
        {
            EndTransition(null);
        }
    }

    private IEnumerator UnloadScenes(List<string> scenesToUnload)
    {
        List<AsyncOperation> unloadOperations = new List<AsyncOperation>();
        foreach (string address in scenesToUnload)
        {
            unloadOperations.Add(SceneManager.UnloadSceneAsync(address));
        }
        while (true)
        {
            bool allDone = unloadOperations.TrueForAll(operation => operation.isDone);
            if (allDone)
            {
                break;
            }
            yield return null;
        }
        EndTransition(null);
    }
}
