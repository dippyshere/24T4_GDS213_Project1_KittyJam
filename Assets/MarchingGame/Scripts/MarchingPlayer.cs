using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class MarchingPlayer : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the marching player")] public static MarchingPlayer Instance;
    [Header("Configuration")]
    [SerializeField, Tooltip("The speed of the player cat")] private float wandSensitivityMultiplier = 1f;
    [Header("References")]
    [SerializeField, Tooltip("Animator for the player cat")] public Animator catAnimator;
    [SerializeField, Tooltip("List of wand note spawn managers")] public MarchingLane[] marchingLanes;
    [HideInInspector, Tooltip("Whether the player is allowed to move")] public bool canMove = true;
    [Tooltip("The position of the mouse")] private Vector3 mousePosition;
    [HideInInspector, Tooltip("Whether the input should be reversed")] public bool ReverseInput = false;

    private void Awake()
    {
        Instance = this;
    }

    public void HandleMouseInput(InputAction.CallbackContext context)
    {
        if (canMove && Cursor.lockState == CursorLockMode.Locked)
        {
            if (ReverseInput)
            {
                mousePosition += new Vector3(-context.ReadValue<Vector2>().x * 0.5f * 0.1f, context.ReadValue<Vector2>().y * 0.5f * 0.1f, 0) * wandSensitivityMultiplier;
            }
            else
            {
                mousePosition += new Vector3(context.ReadValue<Vector2>().x * 0.5f * 0.1f, context.ReadValue<Vector2>().y * 0.5f * 0.1f, 0) * wandSensitivityMultiplier;
            }
            mousePosition.x = Mathf.Clamp(mousePosition.x, -1, 1);
            mousePosition.y = Mathf.Clamp(mousePosition.y, -1, 1);
            // Update the UpDown and LeftRight parameters of the animator
            catAnimator.SetFloat("UpDown", mousePosition.y);
            catAnimator.SetFloat("LeftRight", mousePosition.x);
        }
    }

    public void SetSplineMovementSpeed(float speed)
    {
        SplineAnimate splineAnimate = GetComponent<SplineAnimate>();
        if (splineAnimate != null)
        {
            splineAnimate.MaxSpeed = speed;
        }
    }
}
