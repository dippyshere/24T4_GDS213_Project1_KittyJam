using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the transitions between scenes
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField, Tooltip("The animator that controls the circle wipe")] private Animator circleTransitionAnimator;
    [SerializeField, Tooltip("The animator that controls the logo")] private Animator logoTransitionAnimator;

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();
        circleTransitionAnimator.SetTrigger("End");
        logoTransitionAnimator.SetTrigger("End");
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
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad);
    }
}
