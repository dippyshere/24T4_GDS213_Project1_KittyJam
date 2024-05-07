using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the behavior of the marching notes
/// </summary>
public class MarchingNote : MonoBehaviour
{
    [HideInInspector, Tooltip("The time that the note was instantiated at")] public double timeInstantiated;
    [HideInInspector, Tooltip("The time that the note needs to be hit")] public float assignedTime;

    void Start()
    {
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        Invoke(nameof(OnMiss), (float)(SongManager.Instance.noteTime + ScoreManager.Instance.goodRange + SongManager.Instance.inputOffset));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            NoteFeedback feedbackType = ScoreManager.Instance.Hit(0, transform.position, true);
            if (feedbackType == NoteFeedback.Perfect || feedbackType == NoteFeedback.Good)
            {
#if UNITY_IOS
                try
                {
                    Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
                }
                catch (Exception)
                {

                }
#elif UNITY_ANDROID
                Vibration.VibratePop();
#endif
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the note is missed
    /// </summary>
    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.parent.transform.position, isAlternateNote: true);
        Destroy(gameObject);
    }
}
