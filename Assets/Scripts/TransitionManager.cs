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
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        StartCoroutine(LoadSceneCoroutine(sceneToLoad, 0));
    }

    /// <summary>
    /// Transitions to a scene, starting the music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicStart(string sceneToLoad)
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        StartCoroutine(LoadSceneCoroutine(sceneToLoad, 1));
    }

    /// <summary>
    /// Transitions to a scene, stopping the music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicStop(string sceneToLoad)
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        StartCoroutine(LoadSceneCoroutine(sceneToLoad, 2));
    }

    /// <summary>
    /// Loads the scene with the desired music behaviour
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    /// <param name="musicBehaviour">Continue, start, or stop the music</param>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator LoadSceneCoroutine(string sceneToLoad, int musicBehaviour)
    {
        if (AudioManager.instance != null)
        {
            switch (musicBehaviour)
            {
                case 0:
                    yield return new WaitForSecondsRealtime(1.05f);
                    break;
                case 1:
                    AudioManager.instance.PlayMusic();
                    yield return new WaitForSecondsRealtime(1.05f);
                    break;
                case 2:
                    yield return new WaitForSecondsRealtime(1.05f);
                    AudioManager.instance.StopMusic();
                    break;
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(1.05f);
        }
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync("Assets/Scenes/" + sceneToLoad + ".unity");
        while (!handle.IsDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath("Assets/Scenes/" + sceneToLoad + ".unity"));
        circleTransitionAnimator.SetTrigger("End");
        logoTransitionAnimator.SetTrigger("End");
    }

    public void StartTransitionAndLoadScenes(List<string> scenesToLoad)
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");

        StartCoroutine(DownloadAndLoadScenes(scenesToLoad));
    }

    private IEnumerator DownloadAndLoadScenes(List<string> scenesToLoad)
    {
        yield return new WaitForSecondsRealtime(1.05f);

        yield return StartCoroutine(DownloadManager.Instance.DownloadScenes(scenesToLoad, EndTransition));
    }

    public void EndTransition()
    {
        circleTransitionAnimator.SetTrigger("End");
        logoTransitionAnimator.SetTrigger("End");
    }
}
