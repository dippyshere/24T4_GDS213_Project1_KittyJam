using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighwayScoreManager : MonoBehaviour
{
    public static HighwayScoreManager Instance;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public TextMeshProUGUI scoreText;
    static int comboScore;
    void Start()
    {
        Instance = this;
        comboScore = 0;
    }
    public static void Hit()
    {
        comboScore += 1;
        Instance.hitSFX.Play();
    }
    public static void Miss()
    {
        comboScore = 0;
        Instance.missSFX.Play();    
    }
    private void Update()
    {
        scoreText.text = comboScore.ToString();
    }
}
