using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the feedback text for the player's note timing
/// </summary>
public class NoteFeedbackManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text object to display feedback on")] private TextMeshProUGUI feedbackText;
    [SerializeField, Tooltip("List of color gradients for different feedback types")] private TMP_ColorGradient[] tMP_ColorGradients;
    [SerializeField, Tooltip("Animator for the feedback text")] private Animator animator;

    /// <summary>
    /// Set the feedback text and color gradient based on the feedback type
    /// </summary>
    /// <param name="feedback"></param>
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
                feedbackText.text = "Purrfect";
                feedbackText.colorGradientPreset = tMP_ColorGradients[2];
                break;
        }
        animator.SetBool("StartAnim", true);
    }
}

/// <summary>
/// Different types of feedback for the player's note timing
/// </summary>
public enum NoteFeedback
{ 
    Miss,
    TooEarly,
    Good,
    Perfect
}
