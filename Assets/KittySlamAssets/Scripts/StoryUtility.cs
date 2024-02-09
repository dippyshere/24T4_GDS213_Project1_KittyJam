using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryUtility : MonoBehaviour
{

    public GameObject scene1;
    public GameObject scene2;
    public GameObject scene3;

    public TextMeshProUGUI nextButton;

    private int sceneIndex = 1;

    public void NextScene()
    {
        scene1.SetActive(false);
        scene2.SetActive(false);
        scene3.SetActive(false);

        sceneIndex++;

        if (sceneIndex == 1)
        {
            scene1.SetActive(true);
        }
        else if (sceneIndex == 2)
        {
            scene2.SetActive(true);
        }
        else if (sceneIndex == 3)
        {
            scene3.SetActive(true);
            nextButton.text = "PLAY";
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
