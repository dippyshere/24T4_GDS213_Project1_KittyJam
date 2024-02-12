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
    int score;
    int multiplier;

    void Start()
    {
        Instance = this;
        comboScore = 0;
        score = 0;
        multiplier = 1;
    }

    public void Hit()
    {
        comboScore += 1;
        score += comboScore * multiplier;
        if (comboScore % 3 == 0 && comboScore < 8)
        {
            multiplier += 1;
        }
        UpdateScoreText();
        Instance.hitSFX.Play();
    }

    public void Miss()
    {
        comboScore = 0;
        multiplier = 1;
        UpdateScoreText();
        Instance.missSFX.Play();
    }

    private void UpdateScoreText()
    {
        multiplierText.text = "Multiplier: x" + multiplier.ToString();
        scoreText.text = "Score: " + score.ToString("N0", CultureInfo.InvariantCulture);
        comboText.text = "Combo: " + comboScore.ToString("N0", CultureInfo.InvariantCulture);
    }
}
