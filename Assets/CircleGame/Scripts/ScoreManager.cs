using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource missSFX;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    int comboScore;
    long score;
    int multiplier;
    public float perfectBonus;
    long displayedScore;
    int hitCount;
    int perfectCount;
    int missCount;

    void Start()
    {
        Instance = this;
        comboScore = 0;
        score = 0;
        multiplier = 1;
    }

    private void FixedUpdate()
    {
        displayedScore = (long)Mathf.Ceil(Mathf.Lerp(displayedScore, score, 0.06f));
        UpdateScoreText();
    }

    public void Hit(float hitTime, Vector3 position)
    {
        Debug.Log("Hit time: " + hitTime);
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
            Miss(position);
            return;
        }
    }

    public void Miss(Vector3 position, NoteFeedback noteFeedback = NoteFeedback.Miss)
    {
        ShowNoteFeedback(noteFeedback, position);
        comboScore = 1;
        multiplier = 1;
        missCount++;
        UpdateScoreText();
        Instance.missSFX.Play();
    }

    private void HitGood()
    {
        comboScore += 1;
        score += comboScore * multiplier;
        hitCount++;
        if (comboScore % 3 == 0 && comboScore < 8)
        {
            multiplier += 1;
        }
        Instance.hitSFX.Play();
        UpdateScoreText();
    }

    private void HitPerfect()
    {
        comboScore += 1;
        score += (long)Mathf.Ceil(comboScore * multiplier * (long)perfectBonus);
        perfectCount++;
        if (comboScore % 3 == 0 && comboScore < 8)
        {
            multiplier += 1;
        }
        Instance.hitSFX.Play();
        UpdateScoreText();
    }

    public void ShowNoteFeedback(NoteFeedback feedback, Vector3 position)
    {
        GameObject feedbackObject = Instantiate(SongManager.Instance.noteFeedbackPrefab, position, Quaternion.identity);
        feedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
        Debug.Log("Feedback: " + feedback);
    }

    private void UpdateScoreText()
    {
        multiplierText.text = "Multiplier: x" + multiplier.ToString();
        scoreText.text = "Score: " + displayedScore.ToString("N0", CultureInfo.InvariantCulture);
        comboText.text = "Combo: " + comboScore.ToString("N0", CultureInfo.InvariantCulture);
    }
}
