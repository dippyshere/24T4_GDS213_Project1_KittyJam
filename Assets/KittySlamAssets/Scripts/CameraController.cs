using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the camera shake effect
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField, Tooltip("The transform controlling the top left UI position")] private RectTransform topLeftUI;
    [Tooltip("The original position of the camera")] private Vector3 originalPosition;
    [Tooltip("The original position of the UI")] private Vector3 originalUIPosition;

    private void Start()
    {
        originalPosition = transform.position;
        originalUIPosition = topLeftUI.position;
    }

    /// <summary>
    /// Shakes the camera for a given duration with a given intensity
    /// </summary>
    /// <param name="intensity">The intensity of the shake</param>
    /// <returns>The IEnumerator for the coroutine</returns>
    public IEnumerator ShakeCamera(float intensity, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float x = originalPosition.x + Random.Range(-intensity, intensity);
            float y = originalPosition.y + Random.Range(-intensity, intensity);
            float uiX = originalUIPosition.x + Random.Range(-intensity / 12, intensity / 12);
            float uiY = originalUIPosition.y + Random.Range(-intensity / 12, intensity / 12);
            transform.position = new Vector3(x, y, originalPosition.z);
            topLeftUI.position = new Vector3(uiX, uiY, originalUIPosition.z);
            // Gradually decrease the intensity of the shake over time
            float t = Mathf.Clamp01(elapsedTime / duration);
            intensity = Mathf.Lerp(intensity, 0f, t / 4);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
        topLeftUI.position = originalUIPosition;
        Invoke(nameof(RestorePositions), 0.1f);
    }

    /// <summary>
    /// Restores the camera and UI to their original positions
    /// </summary>
    private void RestorePositions()
    {
        transform.position = originalPosition;
        topLeftUI.position = originalUIPosition;
    }
}
