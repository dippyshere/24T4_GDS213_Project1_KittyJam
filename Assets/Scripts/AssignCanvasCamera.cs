using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCanvasCamera : MonoBehaviour
{
    [Tooltip("Reference to the canvas to assign the camera to.")] private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private IEnumerator Start()
    {
        canvas = GetComponent<Canvas>();
        yield return null;
        canvas.worldCamera = Camera.main;
        yield return null;
    }

    private void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
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
        canvas.worldCamera = Camera.main;
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        canvas.worldCamera = Camera.main;
    }

    private IEnumerator RefreshCamera()
    {
        while (true)
        {
            canvas.worldCamera = Camera.main;
            yield return new WaitForSeconds(1.5f);
        }
    }
}
