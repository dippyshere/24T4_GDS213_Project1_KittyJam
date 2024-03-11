using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the cursor state
/// </summary>
public class CursorController : MonoBehaviour
{
    [SerializeField] private bool lockCursorOnStart = true;

    private void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    /// <summary>
    /// Locks the cursor to the game window and hides it
    /// </summary>
    public void LockCursor()
    {
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Unlocks the cursor and makes it visible
    /// </summary>
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// When the object is destroyed, unlock the cursor
    /// </summary>
    private void OnDestroy()
    {
        UnlockCursor();
    }
}
