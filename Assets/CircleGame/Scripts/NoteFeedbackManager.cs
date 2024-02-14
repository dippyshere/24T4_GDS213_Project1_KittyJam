using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteFeedbackManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    public TMP_ColorGradient[] tMP_ColorGradients;
    public Animator animator;

    public void SetFeedbackType(NoteFeedback feedback)
    {
        switch (feedback)
        {
            case NoteFeedback.Miss:
                feedbackText.text = "Miss";
                feedbackText.colorGradientPreset = tMP_ColorGradients[0];
                break;
            case NoteFeedback.TooEarly:
                feedbackText.text = "Too Early";
                feedbackText.colorGradientPreset = tMP_ColorGradients[1];
                break;
            case NoteFeedback.Good:
                feedbackText.text = "Good";
                feedbackText.colorGradientPreset = tMP_ColorGradients[1];
                break;
            case NoteFeedback.Perfect:
                feedbackText.text = "Perfect";
                feedbackText.colorGradientPreset = tMP_ColorGradients[2];
                break;
        }
        animator.SetBool("StartAnim", true);
    }
}

public enum NoteFeedback
{ 
    Miss,
    TooEarly,
    Good,
    Perfect
}
