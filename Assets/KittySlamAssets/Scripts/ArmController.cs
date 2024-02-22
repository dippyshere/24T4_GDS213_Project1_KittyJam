using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player arm controller
/// </summary>
public class ArmController : MonoBehaviour
{
    [SerializeField, Tooltip("The arm speed upgrade level")] private float armSpeed = 5f;
    [SerializeField, Tooltip("Adjusts the arm sensitivity multiplier")] private float armSensitivityMultiplier = 1f;
    [SerializeField, Tooltip("The slam speed upgrade level")] private float slamSpeedMultiplier = 1f;
    // [SerializeField, Tooltip("The layer that items exist on")] private LayerMask itemLayer;
    [SerializeField, Tooltip("Reference to the camera controller")] private CameraController cameraController;
    [SerializeField, Tooltip("Reference to the arm animator")] private Animator animator;
    [SerializeField, Tooltip("Game object that is spawned when slamming occurs")] private GameObject slamEffect;
    [SerializeField, Tooltip("Transform that the slam effect occurs at")] private Transform slamPosition;
    [SerializeField, Tooltip("Reference to the pause menu")] private GameObject pauseMenu;
    [HideInInspector, Tooltip("Whether the player is allowed to slam")] public bool canSlam = true;
    [HideInInspector, Tooltip("Whether the player is allowed to move")] public bool canMove = true;
    [SerializeField, Tooltip("List of trail renderers to enable/disable while slamming")] private List<TrailRenderer> trails = new List<TrailRenderer>();
    [Tooltip("Whether the player is currently slamming")] private bool isSlamming = false;
    [Tooltip("If a note has already been hit since the last slam")] private bool noteHit = false;
    [Tooltip("The position of the mouse")] private Vector3 mousePosition;

    private void Update()
    {
        if (canSlam)
        {
            // Check for player input
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetFloat("SlamSpeedMult", slamSpeedMultiplier);
                animator.SetTrigger("Slam");
                isSlamming = true;
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].enabled = true;
                }
                //canMove = false;
                //if (!true)
                //{
                //    ChatGPT (unused)
                //    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, itemLayer);
                //    if (hit.collider != null)
                //    {
                //        targetPosition = hit.collider.transform.position;
                //    }
                //}
            }
            // 2nd condition is a bug workaround
            else if (!isSlamming && !Input.GetMouseButton(0) || animator.GetBool("Slam") && !Input.GetMouseButton(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
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

        if (canMove && Cursor.lockState == CursorLockMode.Locked)
        {
            mousePosition += new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0) * armSensitivityMultiplier;
            mousePosition.x = Mathf.Clamp(mousePosition.x, -8 * 1.3f, 8 * 1.3f);
            mousePosition.y = Mathf.Clamp(mousePosition.y, -4 * 1.3f, 4 * 1.3f);
            transform.position = Vector3.Lerp(transform.position, mousePosition, armSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Pause))
        {
            // Toggle the pause menu
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }

    /// <summary>
    /// Called when the slam animation hits to trigger the screen shake
    /// </summary>
    public void OnSlam()
    {
        cameraController.StartCoroutine(cameraController.ShakeCamera(0.3f, 0.4f));
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
 