using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Animator circleTransitionAnimator;
    public Animator logoTransitionAnimator;

    public void StartGame()
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        Invoke("LoadOnboarding", 1.05f);
    }

    public void StartGameType2()
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        Time.timeScale = 1;
        Invoke("LoadGameScene", 1.05f);
    }

    public void StartMenu()
    {
        circleTransitionAnimator.SetTrigger("Start");
        logoTransitionAnimator.SetTrigger("Start");
        Time.timeScale = 1;
        Invoke("LoadMainMenu", 1.05f);
    }

    private void LoadGameScene()
    {
        Time.timeScale = 1;
        AudioManager.instance.StopMusic();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameType2");
    }

    private void LoadOnboarding()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game2Onboarding");
    }

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
