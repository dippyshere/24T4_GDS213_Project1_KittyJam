using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public float armSpeed = 5f;
    public float slamSpeedMultiplier = 1f;
    public LayerMask itemLayer;
    public CameraController cameraController;
    public Animator animator;
    public GameObject slamEffect;
    public GameObject scoreFeedback;
    public Transform slamPosition;
    public ScoreManager scoreManager;
    public GameObject pauseMenu;
    public bool canSlam = true;
    public List<TrailRenderer> trails = new List<TrailRenderer>();

    private bool canMove = true;
    private bool checkResolution = true;
    private bool isSlamming = false;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private Vector3 cameraOffset;

    private void Start()
    {
        UpdateSafeZone();
        StartCoroutine(CheckForResize());
        Camera camera = Camera.main;
        cameraOffset = camera.transform.forward * 10f;
    }

    private void UpdateSafeZone()
    {
        float widthScale = (float)Screen.width / 1920;
        float heightScale = (float)Screen.height / 1080;
        minX = -(100 * widthScale);
        maxX = Screen.width + (100 * widthScale);
        minY = -(100 * heightScale);
        maxY = Screen.height + (100 * heightScale);
        Debug.Log("minX: " + minX + " maxX: " + maxX + " minY: " + minY + " maxY: " + maxY);
        Debug.Log("Screen.width: " + Screen.width + " Screen.height: " + Screen.height);
        Debug.Log("widthScale: " + widthScale + " heightScale: " + heightScale);
    }

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
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            canMove = !pauseMenu.activeSelf;
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
            SongManager.Instance.OnApplicationPause(pauseMenu.activeSelf);
            UpdateSafeZone();
        }
    }

    // ChatGPT
    private Vector3 GetRemappedMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousePosition.y, minY, maxY);

        // Remap the mouse position values within the safe zone boundaries
        float remappedX = Remap(clampedX, minX, maxX, 0f, Screen.width);
        float remappedY = Remap(clampedY, minY, maxY, 0f, Screen.height);

        return new Vector3(remappedX, remappedY, 0f);
    }

    // ChatGPT
    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
    }

    // ChatGPT
    private void OnDrawGizmos()
    {
        // Get the camera component
        Camera camera = Camera.main;
        if (camera == null)
            return;

        // Calculate the world coordinates of the safe zone corners
        Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(minX, minY, 10));
        Vector3 bottomRight = camera.ScreenToWorldPoint(new Vector3(maxX, minY, 10));
        Vector3 topLeft = camera.ScreenToWorldPoint(new Vector3(minX, maxY, 10));
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(maxX, maxY, 10));

        // Draw the safe zone boundaries as a gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

    public void onSlam()
    {
        cameraController.StartCoroutine(cameraController.ShakeCamera(0.3f, 0.4f));
        isSlamming = false;
        Instantiate(slamEffect, slamPosition.position, Quaternion.identity);
    }

    public void PickUpItem(GameObject other)
    {
        Debug.Log("Item");
        if (other.TryGetComponent<CircleGemController>(out var gem))
        {
            // Debug.Log("Scored: " + item.currentCollectionBonus);
            scoreManager.Hit();
            gem.OnPickup();
            //GameObject scoreFeedbackInstance = Instantiate(scoreFeedback, item.transform.position, Quaternion.identity);
            //scoreFeedbackInstance.GetComponent<ScoreFeedbackManager>().scoreValue = Mathf.CeilToInt(item.currentCollectionBonus);
        }
    }

}
 