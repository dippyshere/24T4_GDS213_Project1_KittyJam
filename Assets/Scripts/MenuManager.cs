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

    private void Start()
    {
        circleTransitionAnimator.SetTrigger("End");
        logoTransitionAnimator.SetTrigger("End");
    }

    /// <summary>
    /// Starts the game and transitions to the onboarding scene
    /// </summary>
    public void StartGame()
    {
        // Start the circle and logo transition animations
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        // Load the onboarding scene after the transition animation finishes
        Invoke("LoadOnboarding", 1.05f);
    }

    /// <summary>
    /// Starts the game and transitions to the game scene
    /// </summary>
    public void StartGameType1()
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        // Unpause the game to allow the scene transition to occur
        Time.timeScale = 1;
        Invoke("LoadGameScene1", 1.05f);
    }

    /// <summary>
    /// Transitions to the main menu scene
    /// </summary>
    public void StartMenu()
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        Time.timeScale = 1;
        Invoke("LoadMainMenu", 1.05f);
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    private void LoadGameScene1()
    {
        Time.timeScale = 1;
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameType1CircleGame");
    }

    /// <summary>
    /// Loads the onboarding scene
    /// </summary>
    private void LoadOnboarding()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameType1Onboarding");
    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    private void LoadMainMenu()
    {
        Time.timeScale = 1;
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
