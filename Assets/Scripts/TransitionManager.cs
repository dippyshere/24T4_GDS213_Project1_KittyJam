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
        return;
    }

    /// <summary>
    /// Transitions to a scene, starting the music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicStart(string sceneToLoad)
    {
        return;
    }

    /// <summary>
    /// Transitions to a scene, stopping the music
    /// </summary>
    /// <param name="sceneToLoad">The scene to load</param>
    public void StartLoadingSceneMusicStop(string sceneToLoad)
    {
        return;
    }

    public void StartTransition()
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
    }

    public void EndTransition()
    {
        circleTransitionAnimator.SetTrigger("End");
        logoTransitionAnimator.SetTrigger("End");
    }
}
