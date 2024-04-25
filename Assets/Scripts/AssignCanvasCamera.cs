using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assigns the main camera to the canvas to ensure it is always visible
/// </summary>
public class AssignCanvasCamera : MonoBehaviour
{
    [Tooltip("Reference to the canvas to assign the camera to.")] private Canvas canvas;
    [SerializeField, Tooltip("Name of the camera that should be expected.")] private string cameraName = "MainCamera";

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        StartCoroutine(WaitForCamera());
    }

    /// <summary>
    /// Waits for the camera to be assigned before assigning it to the canvas
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForCamera()
    {
        while (Camera.main == null && Camera.main.name != cameraName)
        {
            yield return null;
        }
        canvas.worldCamera = Camera.main;
    }

    private void OnEnable()
    {
        StartCoroutine(RefreshCamera());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (Camera.main != null && Camera.main.name == cameraName)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (Camera.main != null && Camera.main.name == cameraName)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    /// <summary>
    /// Continuously refreshes the camera on the canvas
    /// </summary>
    /// <returns></returns>
    private IEnumerator RefreshCamera()
    {
        while (true)
        {
            if (Camera.main != null && Camera.main.name == cameraName)
            {
                canvas.worldCamera = Camera.main;
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                yield return null;
            }
        }
    }
}
