using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handled the trail that followed the mouse cursor
/// </summary>
public class MarchingTrail : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the trail renderer")] private TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer.enabled = false;
    }

    /// <summary>
    /// Starts the trail when the mouse button is pressed
    /// </summary>
    /// <param name="context">The context of the input action</param>
    public void StartTrail(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            trailRenderer.enabled = true;
        }
        else if (context.canceled)
        {
            trailRenderer.enabled = false;
            trailRenderer.Clear();
        }
    }

    /// <summary>
    /// Handles the position of the trail
    /// </summary>
    /// <param name="context">The context of the input action</param>
    public void SetTrailPosition(InputAction.CallbackContext context)
    {
        if (trailRenderer.enabled)
        {
            Vector3 mousePos = context.ReadValue<Vector2>();
            mousePos.z = Camera.main.nearClipPlane;
            trailRenderer.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}
