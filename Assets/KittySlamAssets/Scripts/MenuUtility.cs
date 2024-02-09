using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUtility : MonoBehaviour
{
    public void OpenStory()
    {
        SceneManager.LoadScene("Story");
    }

    public void OpenGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
