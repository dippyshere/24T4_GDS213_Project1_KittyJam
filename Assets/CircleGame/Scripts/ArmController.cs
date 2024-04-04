using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The player arm controller
/// </summary>
public class ArmController : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the arm controller")] public static ArmController Instance;
    [Header("Configuration")]
    [SerializeField, Tooltip("Adjusts the arm sensitivity multiplier")] private float armSensitivityMultiplier = 1f;
    [SerializeField, Tooltip("The slam speed upgrade level")] private float slamSpeedMultiplier = 1f;
    [Header("References")]
    [SerializeField, Tooltip("Reference to the arm animator")] private Animator animator;
    [SerializeField, Tooltip("Game object that is spawned when slamming occurs")] private GameObject slamEffect;
    [SerializeField, Tooltip("Transform that the slam effect occurs at")] private Transform slamPosition;
    [HideInInspector, Tooltip("Whether the player is allowed to slam")] public bool canSlam = true;
    [HideInInspector, Tooltip("Whether the player is allowed to move")] public bool canMove = true;
    [SerializeField, Tooltip("List of trail renderers to enable/disable while slamming")] private List<TrailRenderer> trails = new List<TrailRenderer>();
    [Tooltip("Whether the player is currently slamming")] private bool isSlamming = false;
    [Tooltip("If a note has already been hit since the last slam")] private bool noteHit = false;
    [Tooltip("The position of the mouse")] private Vector3 mousePosition;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForCursorControllerInstance());
        StartCoroutine(WaitForPauseMenuInstance());
    }

    private IEnumerator WaitForCursorControllerInstance()
    {
        yield return new WaitUntil(() => CursorController.Instance != null);
        CursorController.Instance.LockCursor();
    }

    private IEnumerator WaitForPauseMenuInstance()
    {
        yield return new WaitUntil(() => PauseMenu.Instance != null);
        PauseMenu.Instance.OnPauseGameplay += PauseGameplay;
    }

    /// <summary>
    /// Moves the arm
    /// </summary>
    /// <param name="context">The input context</param>
    public void MoveArm(InputAction.CallbackContext context)
    {
        if (canMove && Cursor.lockState == CursorLockMode.Locked)
        {
            mousePosition += new Vector3(context.ReadValue<Vector2>().x * 0.5f * 0.1f, context.ReadValue<Vector2>().y * 0.5f * 0.1f, 0) * armSensitivityMultiplier;
            mousePosition.x = Mathf.Clamp(mousePosition.x, -8 * 1.3f, 8 * 1.3f);
            mousePosition.y = Mathf.Clamp(mousePosition.y, -4 * 1.3f, 4 * 1.3f);
            transform.position = mousePosition;
        }
    }

    /// <summary>
    /// Slams the arm down
    /// </summary>
    /// <param name="context">The input context</param>
    public void SlamArm(InputAction.CallbackContext context)
    {
        if (canSlam)
        {
            // Check for player input
            if (context.started)
            {
                animator.SetFloat("SlamSpeedMult", slamSpeedMultiplier);
                animator.SetTrigger("Slam");
                isSlamming = true;
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].enabled = true;
                }
            }
            // 2nd condition is a bug workaround
            else if (!isSlamming && !context.performed || animator.GetBool("Slam") && !context.performed && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                animator.ResetTrigger("Slam");
                isSlamming = false;
                canMove = true;
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Called when the slam animation hits to trigger the screen shake
    /// </summary>
    public void OnSlam()
    {
        CameraController.Instance.StartCoroutine(CameraController.Instance.ShakeCamera(0.15f, 5f));
        StartCoroutine(SpawnSlamEffect());
    }

    /// <summary>
    /// Called when the slam animation ends to reset the note hit flag
    /// </summary>
    public void OnSlamEnd()
    {
        noteHit = false;
    }

    /// <summary>
    /// Disables the player's ability to move and slam when the game is paused
    /// </summary>
    /// <param name="pause">Whether the game is paused</param>
    public void PauseGameplay(bool pause)
    {
        if (pause)
        {
            canMove = false;
            canSlam = false;
        }
        else
        {
            canMove = true;
            canSlam = true;
        }
    }

    /// <summary>
    /// Spawns the slam effect at the slam position
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator SpawnSlamEffect()
    {
        yield return new WaitForEndOfFrame();
        isSlamming = false;
        Instantiate(slamEffect, slamPosition.position, Quaternion.identity);
    }

    /// <summary>
    /// Plays a note
    /// </summary>
    /// <param name="other">The game object that was picked up</param>
    public void PickUpItem(GameObject other)
    {
        // get the circle gem controller on all of the objects that are in the capsule collider on the parent of the slam position object
        CapsuleCollider capsuleCollider = slamPosition.GetComponentInParent<CapsuleCollider>();
        Collider[] colliders = Physics.OverlapCapsule(capsuleCollider.transform.TransformPoint(capsuleCollider.center + new Vector3(0, capsuleCollider.height / 2, 0)), capsuleCollider.transform.TransformPoint(capsuleCollider.center - new Vector3(0, capsuleCollider.height / 2, 0)), capsuleCollider.radius / 1.65f);
        // run the pickup function on the circle gem controller with the lowest assigned time
        float lowestAssignedTime = float.MaxValue;
        CircleGemController lowestAssignedTimeController = null;
        foreach (Collider collider in colliders)
        {
            CircleGemController circleGemController = collider.GetComponent<CircleGemController>();
            if (circleGemController != null && circleGemController.assignedTime < lowestAssignedTime)
            {
                lowestAssignedTime = circleGemController.assignedTime;
                lowestAssignedTimeController = circleGemController;
            }
        }
        if (lowestAssignedTimeController != null && !noteHit)
        {
            lowestAssignedTimeController.OnPickup();
            noteHit = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // draw the capsule collider check as gizmos
        CapsuleCollider capsuleCollider = slamPosition.GetComponentInParent<CapsuleCollider>();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(capsuleCollider.transform.TransformPoint(capsuleCollider.center + new Vector3(0, capsuleCollider.height / 2, 0)), capsuleCollider.radius / 1.65f);
        Gizmos.DrawWireSphere(capsuleCollider.transform.TransformPoint(capsuleCollider.center - new Vector3(0, capsuleCollider.height / 2, 0)), capsuleCollider.radius / 1.65f);
        Gizmos.DrawLine(capsuleCollider.transform.TransformPoint(capsuleCollider.center + new Vector3(0, capsuleCollider.height / 2, 0)) + new Vector3(capsuleCollider.radius / 1.65f, 0, 0), capsuleCollider.transform.TransformPoint(capsuleCollider.center - new Vector3(0, capsuleCollider.height / 2, 0)) + new Vector3(capsuleCollider.radius / 1.65f, 0, 0));
        Gizmos.DrawLine(capsuleCollider.transform.TransformPoint(capsuleCollider.center + new Vector3(0, capsuleCollider.height / 2, 0)) - new Vector3(capsuleCollider.radius / 1.65f, 0, 0), capsuleCollider.transform.TransformPoint(capsuleCollider.center - new Vector3(0, capsuleCollider.height / 2, 0)) - new Vector3(capsuleCollider.radius / 1.65f, 0, 0));
    }

}
 