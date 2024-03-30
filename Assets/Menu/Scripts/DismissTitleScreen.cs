using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Dismisses the title screen when any button is pressed or tapped
/// </summary>
public class DismissTitleScreen : MonoBehaviour
{
    private IDisposable m_EventListener;

    void OnEnable()
    {
        m_EventListener = InputSystem.onAnyButtonPress.Call(OnButtonPressed);
    }

    void OnDisable()
    {
        m_EventListener.Dispose();
    }

    void OnButtonPressed(InputControl button)
    {
        //var device = button.device;

        TransitionManager.Instance.StartTransitionAndLoadScenes(new List<string> { "Assets/Scenes/SongSelection.unity" });

        gameObject.SetActive(false);
    }
}
