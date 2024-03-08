using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the player's score and combo system
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the score manager")] public static ScoreManager Instance;

    [SerializeField, Tooltip("The sound to play when successfully hitting a note")] private AudioSource hitSFX;
    [SerializeField, Tooltip("The sound to play when missing a note")] private AudioSource missSFX;
    [SerializeField, Tooltip("Reference to the text displaying the current multiplier")] private TextMeshProUGUI multiplierText;
    [SerializeField, Tooltip("Reference to the text displaying the current score")] private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Reference to the text displaying the current combo")] private TextMeshProUGUI comboText;

    [Tooltip("The current combo")] private int comboScore;
    [HideInInspector, Tooltip("The current score")] public long score;
    [Tooltip("The current combo multiplier")] private int multiplier;
    [SerializeField, Tooltip("The current perfect hit bonus multipler")] private float perfectBonus = 1;
    [Tooltip("The currently displayed score. This is used to gradually change the score over time")] private long displayedScore;
    [HideInInspector, Tooltip("The count of good hits")] public int hitCount;
    [HideInInspector, Tooltip("The count of perfect hits")] public int perfectCount;
    [HideInInspector, Tooltip("The count of missed notes")] public int missCount;

    void Start()
    {
        // Setup variables
        Instance = this;
        comboScore = 0;
        score = 0;
        multiplier = 1;
    }

    private void FixedUpdate()
    {
        // Gradually update the displayed score to the actual score at a fixed rate
        displayedScore = (long)Mathf.Ceil(Mathf.Lerp(displayedScore, score, 0.06f));
        UpdateScoreText();
    }

    /// <summary>
    /// Called when the player successfully hits a note to update the score and combo, play a sound, and display feedback
    /// </summary>
    /// <param name="hitTime">The time the note was hit. The closer to 0, the better</param>
    /// <param name="position">The position of the note that was hit, to display feedback at</param>
    public void Hit(double hitTime, Vector3 position)
    {
        // Debug.Log("Hit time: " + hitTime);
        hitTime *= -1;
        if (hitTime >= -SongManager.Instance.perfectRange && hitTime <= SongManager.Instance.perfectRange)
        {
            ShowNoteFeedback(NoteFeedback.Perfect, position);
            HitPerfect();
            return;
        }
        else if (hitTime >= -SongManager.Instance.goodRange && hitTime <= SongManager.Instance.goodRange)
        {
            ShowNoteFeedback(NoteFeedback.Good, position);
            HitGood();
            return;
        }
        else if (hitTime > SongManager.Instance.goodRange)
        {
            Miss(position, NoteFeedback.TooEarly);
            return;
        }
        else
        {
            // Fall back to a good hit
            ShowNoteFeedback(NoteFeedback.Good, position);
            HitGood();
            return;
        }
    }

    /// <summary>
    /// Called when the player misses a note to reset the combo, play a sound, and display feedback
    /// </summary>
    /// <param name="position">The position of the note that was missed, to display feedback at</param>
    /// <param name="noteFeedback">The type of feedback to display</param>
    public void Miss(Vector3 position, NoteFeedback noteFeedback = NoteFeedback.Miss)
    {
        ShowNoteFeedback(noteFeedback, position);
        comboScore = 0;
        multiplier = 1;
        missCount++;
        UpdateScoreText();
        Instance.missSFX.Play();
    }

    /// <summary>
    /// Called when the player successfully hits a note within the good range to update the score and combo
    /// </summary>
    private void HitGood()
    {
        score += 50 * multiplier;
        hitCount++;
        comboScore++;
        if (comboScore % 3 == 0 && comboScore <= 21)
        {
            multiplier += 1;
        }
        Instance.hitSFX.Play();
        UpdateScoreText();
    }

    /// <summary>
    /// Called when the player successfully hits a note within the perfect range to update the score and combo
    /// </summary>
    private void HitPerfect()
    {
        score += Mathf.CeilToInt(50 * multiplier * perfectBonus);
        perfectCount++;
        comboScore++;
        if (comboScore % 3 == 0 && comboScore <= 21)
        {
            multiplier += 1;
        }
        Instance.hitSFX.Play();
        UpdateScoreText();
    }

    /// <summary>
    /// Show feedback for the player's note timing at a specific position
    /// </summary>
    /// <param name="feedback">The type of feedback to display</param>
    /// <param name="position">The position to display the feedback at</param>
    public void ShowNoteFeedback(NoteFeedback feedback, Vector3 position)
    {
        GameObject feedbackObject = Instantiate(SongManager.Instance.noteFeedbackPrefab, position, Quaternion.identity);
        feedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
    }

    /// <summary>
    /// Update the score text to display the current score, combo, and multiplier
    /// </summary>
    private void UpdateScoreText()
    {
        multiplierText.text = "Multiplier: x" + multiplier.ToString();
        scoreText.text = "" + displayedScore.ToString("N0", CultureInfo.InvariantCulture);
        comboText.text = "x" + comboScore.ToString("N0", CultureInfo.InvariantCulture);
    }
}
