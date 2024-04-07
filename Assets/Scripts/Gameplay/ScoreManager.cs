using UnityEngine;

/// <summary>
/// Callbacks for when the player hits or misses a note
/// </summary>
/// <param name="score">The new score, combo, or multiplier value</param>
public delegate void ScoreEvent(long score);
/// <summary>
/// Callbacks for when the player hits or misses a note
/// </summary>
/// <param name="noteFeedback">The type of feedback to display</param>
/// <param name="position">The position of the note that was hit, to display feedback at</param>
public delegate void NoteEvent(NoteFeedback noteFeedback, Vector3 position);

/// <summary>
/// Manages the player's score and combo system
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the score manager")] public static ScoreManager Instance;

    [Header("Configuration")]
    [SerializeField, Tooltip("How many note hits in a row to increase the multiplier")] private int comboThreshold = 3;
    [SerializeField, Tooltip("The amount to increase the multiplier by when the combo threshold is reached")] private int comboMultiplierIncrease = 1;
    [SerializeField, Tooltip("The maximum multiplier the player can achieve")] private int maxMultiplier = 4;
    [SerializeField, Tooltip("The amount to increase the score by when a note is hit within the good range")] private int goodHitScore = 50;
    [SerializeField, Tooltip("The amount to multiply the good hit score by when a note is hit within the perfect range")] private float perfectBonus = 1.5f;
    [SerializeField, Tooltip("The range in seconds that a note can be hit early or late to still be considered a good hit")] public float goodRange = 0.1f;
    [SerializeField, Tooltip("The range in seconds that a note can be hit early or late to still be considered a perfect hit")] public float perfectRange = 0.05f;

    [Header("References")]
    [SerializeField, Tooltip("The sound to play when successfully hitting a good note")] private AudioClip goodHitSFX;
    [SerializeField, Tooltip("The sound to play when successfully hitting a perfect note")] private AudioClip perfectHitSFX;
    [SerializeField, Tooltip("The sound to play when missing a note")] private AudioClip missSFX;
    [Tooltip("Reference to the audio source to use for playing sound effects")] private AudioSource audioSource;

    [Tooltip("The current combo")] private int comboScore;
    [HideInInspector, Tooltip("The current score")] public long score;
    [Tooltip("The current combo multiplier")] private int multiplier;
    [HideInInspector, Tooltip("The count of good hits")] public int hitCount;
    [HideInInspector, Tooltip("The count of perfect hits")] public int perfectCount;
    [HideInInspector, Tooltip("The count of missed notes")] public int missCount;
    [Header("Callbacks")]
    [HideInInspector, Tooltip("The callback to call when a note is hit or missed")] public NoteEvent noteScoreEvent;
    [HideInInspector, Tooltip("The callback to call when an alternate note is hit or missed")] public NoteEvent alternateNoteScoreEvent;
    [HideInInspector, Tooltip("The callback to call when the score is updated")] public ScoreEvent scoreEvent;
    [HideInInspector, Tooltip("The callback to call when the combo is updated")] public ScoreEvent comboEvent;
    [HideInInspector, Tooltip("The callback to call when the multiplier is updated")] public ScoreEvent multiplierEvent;
    [HideInInspector, Tooltip("The callback to call when the maximum multiplier is reached")] public ScoreEvent maxMultiplierEvent;
    [HideInInspector, Tooltip("The callback to call when the player loses their maximum multiplier")] public ScoreEvent loseMaxMultiplierEvent;

    void Awake()
    {
        // Setup variables
        Instance = this;
        comboScore = 0;
        score = 0;
        multiplier = 1;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        scoreEvent?.Invoke(score);
        comboEvent?.Invoke(comboScore);
        multiplierEvent?.Invoke(multiplier);
    }

    /// <summary>
    /// Called when the player successfully hits a note to update the score and combo, play a sound, and display feedback
    /// </summary>
    /// <param name="hitTime">The time the note was hit. The closer to 0, the better</param>
    /// <param name="position">The position of the note that was hit, to display feedback at</param>
    public NoteFeedback Hit(double hitTime, Vector3 position, bool isAlternateNote = false)
    {
        // Debug.Log("Hit time: " + hitTime);
        hitTime *= -1;
        if (hitTime >= -perfectRange && hitTime <= perfectRange)
        {
            HitPerfect(position, isAlternateNote);
            return NoteFeedback.Perfect;
        }
        else if (hitTime >= -goodRange && hitTime <= goodRange)
        {
            HitGood(position, isAlternateNote);
            return NoteFeedback.Good;
        }
        else if (hitTime > goodRange)
        {
            Miss(position, NoteFeedback.TooEarly);
            return NoteFeedback.TooEarly;
        }
        else
        {
            Miss(position, NoteFeedback.Miss);
            return NoteFeedback.Miss;
        }
    }

    /// <summary>
    /// Called when the player misses a note to reset the combo, play a sound, and display feedback
    /// </summary>
    /// <param name="position">The position of the note that was missed, to display feedback at</param>
    /// <param name="noteFeedback">The type of feedback to display</param>
    public void Miss(Vector3 position, NoteFeedback noteFeedback = NoteFeedback.Miss, bool isAlternateNote = false)
    {
        if (isAlternateNote)
        {
            alternateNoteScoreEvent?.Invoke(noteFeedback, position);
        }
        else
        {
            noteScoreEvent?.Invoke(noteFeedback, position);
        }
        if (multiplier == maxMultiplier)
        {
            loseMaxMultiplierEvent?.Invoke(multiplier);
        }
        comboScore = 0;
        multiplier = 1;
        missCount++;
        multiplierEvent?.Invoke(multiplier);
        comboEvent?.Invoke(comboScore);
        try
        {
            audioSource.PlayOneShot(missSFX);
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// Called when the player successfully hits a note within the good range to update the score and combo
    /// </summary>
    private void HitGood(Vector3 position, bool isAlternateNote)
    {
        if (isAlternateNote)
        {
            alternateNoteScoreEvent?.Invoke(NoteFeedback.Good, position);
        }
        else
        {
            noteScoreEvent?.Invoke(NoteFeedback.Good, position);
        }
        score += goodHitScore * multiplier;
        hitCount++;
        comboScore++;
        IncreaseMultiplier();
        try
        {
            audioSource.PlayOneShot(goodHitSFX);
        }
        catch (System.Exception)
        {

        }
        scoreEvent?.Invoke(score);
        comboEvent?.Invoke(comboScore);
    }

    /// <summary>
    /// Called when the player successfully hits a note within the perfect range to update the score and combo
    /// </summary>
    private void HitPerfect(Vector3 position, bool isAlternateNote = false)
    {
        if (isAlternateNote)
        {
            alternateNoteScoreEvent?.Invoke(NoteFeedback.Perfect, position);
        }
        else
        {
            noteScoreEvent?.Invoke(NoteFeedback.Perfect, position);
        }
        score += Mathf.CeilToInt(goodHitScore * multiplier * perfectBonus);
        perfectCount++;
        comboScore++;
        IncreaseMultiplier();
        try
        {
            audioSource.PlayOneShot(perfectHitSFX);
        }
        catch (System.Exception)
        {

        }
        scoreEvent?.Invoke(score);
        comboEvent?.Invoke(comboScore);
    }

    /// <summary>
    /// Called when the player successfully hits a note to increase the multiplier if the combo threshold is reached
    /// </summary>
    private void IncreaseMultiplier()
    {
        if (comboScore % comboThreshold == 0 && comboScore < maxMultiplier * comboThreshold)
        {
            multiplier += comboMultiplierIncrease;
            multiplierEvent?.Invoke(multiplier);
        }
        if (multiplier == maxMultiplier)
        {
            maxMultiplierEvent?.Invoke(multiplier);
        }
    }
}
