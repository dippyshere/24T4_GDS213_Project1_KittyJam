using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarchingPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("Animator for the player cat")] private Animator catAnimator;
    [SerializeField, Tooltip("The speed of the player cat")] private float wandSensitivityMultiplier = 1f;
    [HideInInspector, Tooltip("Whether the player is allowed to move")] public bool canMove = true;
    [Tooltip("The position of the mouse")] private Vector3 mousePosition;
    [HideInInspector, Tooltip("Whether the input should be reversed")] public bool ReverseInput = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
