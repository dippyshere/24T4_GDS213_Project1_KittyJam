using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MarchingTrail : MonoBehaviour
{
    public TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer.enabled = false;
    }

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
