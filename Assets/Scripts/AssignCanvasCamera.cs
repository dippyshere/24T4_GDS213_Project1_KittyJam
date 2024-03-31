using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCanvasCamera : MonoBehaviour
{
    [Tooltip("Reference to the canvas to assign the camera to.")] private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        canvas.worldCamera = Camera.main;
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        canvas.worldCamera = Camera.main;
    }
}
