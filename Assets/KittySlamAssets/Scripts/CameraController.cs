using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Callback for the camera shake effect
/// </summary>
/// <param name="intensity">The intensity of the shake</param>
/// <param name="duration">The duration of the shake</param>
public delegate void CameraCallback(float intensity, float duration);

/// <summary>
/// Controls the camera shake effect
/// </summary>
public class CameraController : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton instance for the camera controller")] public static CameraController Instance;
    [SerializeField, Tooltip("An additional multiplier to apply to the screen shake effect as an option")] private float shakeMultiplier = 1f;
    [Tooltip("The original position of the camera")] private Vector3 originalPosition;
    [HideInInspector, Tooltip("The callback to call when camera shake begins")] public CameraCallback cameraShakeBeginCallback;
    [SerializeField, Tooltip("Whether to disable the intro animation after it completes")] private bool disableIntroAnimation = false;

    private void Awake()
    {
        Instance = this;
        originalPosition = transform.position;
    }

    private IEnumerator Start()
    {
#if UNITY_IOS || UNITY_ANDROID
        Vibration.Init();
#endif
        if (disableIntroAnimation)
        {
            yield return new WaitForSeconds(3);
            gameObject.GetComponent<Animator>().enabled = false;
        }
    }

    /// <summary>
    /// Shakes the camera for a given duration with a given intensity
    /// </summary>
    /// <param name="intensity">The intensity of the shake</param>
    /// <param name="duration">The duration of the shake</param>
    /// <returns>The IEnumerator for the coroutine</returns>
    public IEnumerator ShakeCamera(float intensity, float duration)
    {
#if UNITY_IOS
        try
        {
            Vibration.VibrateIOS(ImpactFeedbackStyle.Soft);
        }
        catch (Exception)
        {

        }
#elif UNITY_ANDROID
        Vibration.VibratePeek();
#endif
        cameraShakeBeginCallback?.Invoke(intensity * shakeMultiplier * 0.9f, duration);
        float timer = 0;
        while (timer < duration)
        {
            intensity = Mathf.Lerp(intensity, 0, timer / duration);
            transform.position = originalPosition + intensity * shakeMultiplier * UnityEngine.Random.insideUnitSphere;
            timer += Time.smoothDeltaTime;
            yield return null;
        }
        yield return null;
        transform.position = originalPosition;
        yield return new WaitForSecondsRealtime(0.1f);
        transform.position = originalPosition;
    }
}
