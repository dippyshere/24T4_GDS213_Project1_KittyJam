using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public RectTransform topLeftUI;
    private Vector3 originalPosition;
    private Vector3 originalUIPosition;

    private void Start()
    {
        originalPosition = transform.position;
        originalUIPosition = topLeftUI.position;
    }

    public IEnumerator ShakeCamera(float intensity, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float x = originalPosition.x + Random.Range(-intensity, intensity);
            float y = originalPosition.y + Random.Range(-intensity, intensity);
            float uiX = originalUIPosition.x + Random.Range(-intensity / 8, intensity / 8);
            float uiY = originalUIPosition.y + Random.Range(-intensity / 8, intensity / 8);
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

    private void RestorePositions()
    {
        transform.position = originalPosition;
        topLeftUI.position = originalUIPosition;
    }
}
