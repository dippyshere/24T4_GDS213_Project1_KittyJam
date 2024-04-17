using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

/// <summary>
/// Handles the UI elements in the game
/// </summary>
public class GameUIManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("A multiplier to put on the shake intensity to increase/reduce the UI response intensity")] private float intensityMultiplier = 75f;
    [Header("References")]
    [SerializeField, Tooltip("A list of rect transforms to shake upon screen shake")] private List<RectTransform> uiElements;
    [Tooltip("Original position of all the ui elemnts that will shake")] private Vector3[] originalPositions;
    [SerializeField, Tooltip("Reference to the score text")] private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Reference to the combo text")] private TextMeshProUGUI comboText;
    [SerializeField, Tooltip("Reference to the multiplier text")] private TextMeshProUGUI multiplierText;
    [Tooltip("The target score to display")] private long targetScore;
    [Tooltip("The current score to display")] private long currentScore;

    private void Start()
    {
        originalPositions = new Vector3[uiElements.Count];
        for (int i = 0; i < uiElements.Count; i++)
        {
            originalPositions[i] = uiElements[i].localPosition;
        }
        UpdateScoreText(0);
        UpdateComboText(0);
        UpdateMultiplierText(1);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForCameraControllerInstance());
        StartCoroutine(WaitForScoreManagerInstance());
    }

    /// <summary>
    /// Waits for the camera controller instance to be created before subscribing to the camera shake event
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForCameraControllerInstance()
    {
        yield return new WaitUntil(() => CameraController.Instance != null);
        CameraController.Instance.cameraShakeBeginCallback += ShakeUI;
    }

    /// <summary>
    /// Waits for the score manager instance to be created before subscribing to the score events
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForScoreManagerInstance()
    {
        yield return new WaitUntil(() => ScoreManager.Instance != null);
        ScoreManager.Instance.scoreEvent += UpdateScoreText;
        ScoreManager.Instance.comboEvent += UpdateComboText;
        ScoreManager.Instance.multiplierEvent += UpdateMultiplierText;
    }

    /// <summary>
    /// Sets the original positions of the UI elements
    /// </summary>
    public void SetOriginalPositions()
    {
        originalPositions = new Vector3[uiElements.Count];
        for (int i = 0; i < uiElements.Count; i++)
        {
            originalPositions[i] = uiElements[i].localPosition;
        }
    }

    /// <summary>
    /// Shakes the UI elements
    /// </summary>
    /// <param name="intensity">The intensity of the shake</param>
    /// <param name="duration">The duration of the shake</param>
    public void ShakeUI(float intensity, float duration)
    {
        StartCoroutine(ShakeUIRoutine(intensity, duration));
    }

    /// <summary>
    /// Handles the shaking of the UI elements
    /// </summary>
    /// <param name="intensity">The intensity of the shake</param>
    /// <param name="duration">The duration of the shake</param>
    /// <returns>The coroutine</returns>
    private IEnumerator ShakeUIRoutine(float intensity, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            intensity = Mathf.Lerp(intensity, 0, timer / duration);
            for (int i = 0; i < uiElements.Count; i++)
            {
                uiElements[i].localPosition = originalPositions[i] + intensity * intensityMultiplier * Random.insideUnitSphere;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return null;
        for (int i = 0; i < uiElements.Count; i++)
        {
            uiElements[i].localPosition = originalPositions[i];
        }
        yield return new WaitForSecondsRealtime(0.1f);
        for (int i = 0; i < uiElements.Count; i++)
        {
            uiElements[i].localPosition = originalPositions[i];
        }
    }

    /// <summary>
    /// Handles updating the score text
    /// </summary>
    /// <param name="score">The score to update to</param>
    public void UpdateScoreText(long score)
    {
        targetScore = score;
    }

    /// <summary>
    /// Handles updating the combo text
    /// </summary>
    /// <param name="combo">The combo to update to</param>
    public void UpdateComboText(long combo)
    {
        comboText.text = "x" + combo.ToString("N0", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Handles updating the multiplier text
    /// </summary>
    /// <param name="multiplier">The multiplier to update to</param>
    public void UpdateMultiplierText(long multiplier)
    {
        multiplierText.text = "Multiplier: x" + multiplier.ToString("N0", CultureInfo.InvariantCulture);
    }

    private void FixedUpdate()
    {
        currentScore = (long)Mathf.Ceil(Mathf.Lerp(currentScore, targetScore, 0.06f));
        scoreText.text = currentScore.ToString("N0", CultureInfo.InvariantCulture);
    }
}
