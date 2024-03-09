using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Tooltip("Reference to the pause menu")] private GameObject pauseMenu;

    void Start()
    {
        pauseMenu = transform.GetChild(0).gameObject;
    }

    public void TogglePauseState(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }
}
