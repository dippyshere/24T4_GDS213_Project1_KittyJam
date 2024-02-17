using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player arm controller
/// </summary>
public class ArmController : MonoBehaviour
{
    [SerializeField, Tooltip("The arm speed upgrade level")] private float armSpeed = 5f;
    [SerializeField, Tooltip("The slam speed upgrade level")] private float slamSpeedMultiplier = 1f;
    // [SerializeField, Tooltip("The layer that items exist on")] private LayerMask itemLayer;
    [SerializeField, Tooltip("Reference to the camera controller")] private CameraController cameraController;
    [SerializeField, Tooltip("Reference to the arm animator")] private Animator animator;
    [SerializeField, Tooltip("Game object that is spawned when slamming occurs")] private GameObject slamEffect;
    [SerializeField, Tooltip("Transform that the slam effect occurs at")] private Transform slamPosition;
    [SerializeField, Tooltip("Reference to the pause menu")] private GameObject pauseMenu;
    [HideInInspector, Tooltip("Whether the player is allowed to slam")] public bool canSlam = true;
    [Tooltip("Whether the player is allowed to move")] private bool canMove = true;
    [SerializeField, Tooltip("List of trail renderers to enable/disable while slamming")] private List<TrailRenderer> trails = new List<TrailRenderer>();
    [Tooltip("Should the resolution be periodically checked for changes to update the safe zone")] private bool checkResolution = true;
    [Tooltip("Whether the player is currently slamming")] private bool isSlamming = false;

    [Tooltip("The minimum X position of the safe zone")] private float minX;
    [Tooltip("The maximum X position of the safe zone")] private float maxX;
    [Tooltip("The minimum Y position of the safe zone")] private float minY;
    [Tooltip("The maximum Y position of the safe zone")] private float maxY;

    [Tooltip("The offset of the camera to account for when controlling arm position")] Vector3 cameraOffset;

    private void Start()
    {
        UpdateSafeZone();
        StartCoroutine(CheckForResize());
        Camera camera = Camera.main;
        cameraOffset = camera.transform.forward * 10f;
    }

    /// <summary>
    /// Updates the safe zone boundaries based on the current screen resolution
    /// </summary>
    private void UpdateSafeZone()
    {
        float widthScale = (float)Screen.width / 1920;
        float heightScale = (float)Screen.height / 1080;
        minX = -(100 * widthScale);
        maxX = Screen.width + (100 * widthScale);
        minY = -(100 * heightScale);
        maxY = Screen.height + (100 * heightScale);
        //Debug.Log("minX: " + minX + " maxX: " + maxX + " minY: " + minY + " maxY: " + maxY);
        //Debug.Log("Screen.width: " + Screen.width + " Screen.height: " + Screen.height);
        //Debug.Log("widthScale: " + widthScale + " heightScale: " + heightScale);
    }

    /// <summary>
    /// Coroutine to periodically check for changes in resolution to update the safe zone
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    IEnumerator CheckForResize()
    {
        int lastWidth = Screen.width;
        int lastHeight = Screen.height;

        while (checkResolution)
        {
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                UpdateSafeZone();
                lastWidth = Screen.width;
                lastHeight = Screen.height;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void OnDestroy()
    {
        checkResolution = false;
    }


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

        if (canMove)
        {
            // ChatGPT
            // Get the remapped mouse position within the safe zone
            Vector3 remappedMousePosition = GetRemappedMousePosition();
            // Move the arm with the mouse
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(remappedMousePosition + cameraOffset);
            mousePosition.z = 0f;
            transform.position = Vector3.Lerp(transform.position, mousePosition, armSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Pause))
        {
            // Toggle the pause menu
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            canMove = !pauseMenu.activeSelf;
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
            SongManager.Instance.OnApplicationPause(pauseMenu.activeSelf);
            UpdateSafeZone();
        }
    }

    /// <summary>
    /// Get the remapped mouse position within the safe zone
    /// </summary>
    /// <returns>Vector3 of the remapped mouse position</returns>
    private Vector3 GetRemappedMousePosition()
    {
        // ChatGPT
        Vector3 mousePosition = Input.mousePosition;

        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousePosition.y, minY, maxY);

        // Remap the mouse position values within the safe zone boundaries
        float remappedX = Remap(clampedX, minX, maxX, 0f, Screen.width);
        float remappedY = Remap(clampedY, minY, maxY, 0f, Screen.height);

        return new Vector3(remappedX, remappedY, 0f);
    }

    /// <summary>
    /// Remaps a value from one range to another
    /// </summary>
    /// <param name="value">The value to remap</param>
    /// <param name="fromMin">The minimum value of the original range</param>
    /// <param name="fromMax">The maximum value of the original range</param>
    /// <param name="toMin">The minimum value of the new range</param>
    /// <param name="toMax">The maximum value of the new range</param>
    /// <returns>A float of the remapped value</returns>
    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // ChatGPT
        return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
    }

    // ChatGPT
    private void OnDrawGizmos()
    {
        // Draw the safe zone boundaries as a gizmo
        Camera camera = Camera.main;
        if (camera == null)
            return;
        Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(minX, minY, 10));
        Vector3 bottomRight = camera.ScreenToWorldPoint(new Vector3(maxX, minY, 10));
        Vector3 topLeft = camera.ScreenToWorldPoint(new Vector3(minX, maxY, 10));
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(maxX, maxY, 10));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
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
        Debug.Log("Item");
        if (other.TryGetComponent<CircleGemController>(out var gem))
        {
            gem.OnPickup();
        }
    }

}
 