using Melanchall.DryWetMidi.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
        //if (sceneToLoad == "GameType3MarchingGame")
        //{
        //    if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        //    {
        //        StartCoroutine(LoadAssetBundleFromWeb("marchinggamescene"));
        //        StartCoroutine(LoadAssetBundleFromWeb("marchinggame"));
        //        StartCoroutine(LoadAssetBundleFromWeb("marchinggamelighting"));
        //    }
        //    else
        //    {
        //        StartCoroutine(LoadAssetBundleFromDisk("marchinggamescene"));
        //        StartCoroutine(LoadAssetBundleFromDisk("marchinggame"));
        //        StartCoroutine(LoadAssetBundleFromDisk("marchinggamelighting"));
        //    }
        //}
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
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator LoadAssetBundleFromWeb(string fileLocation)
    {
        using UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation);
        yield return www.SendWebRequest();

        // If there was an error, log it
        if (UnityWebRequest.Result.ConnectionError.Equals(www.result) || UnityWebRequest.Result.ProtocolError.Equals(www.result))
        {
            Debug.LogError(www.error);
        }
        else
        {
            AssetBundle.LoadFromMemory(www.downloadHandler.data);
        }
    }

    private IEnumerator LoadAssetBundleFromDisk(string fileLocation)
    {
        AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, fileLocation));
        yield return null;
    }
}
