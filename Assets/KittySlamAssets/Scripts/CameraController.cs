using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // From previous project
    public IEnumerator ShakeCamera(float intensity, float duration)
    {
        Vector3 originalPosition = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float x = originalPosition.x + Random.Range(-intensity, intensity);
            float y = originalPosition.y + Random.Range(-intensity, intensity);
            transform.position = new Vector3(x, y, originalPosition.z);
            // Gradually decrease the intensity of the shake over time
            float t = Mathf.Clamp01(elapsedTime / duration);
            intensity = Mathf.Lerp(intensity, 0f, t / 4);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
