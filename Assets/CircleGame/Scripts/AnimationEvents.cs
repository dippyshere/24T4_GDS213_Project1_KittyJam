using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the animation events for the player
/// </summary>
public class AnimationEvents : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the arm controller")] private ArmController armController;

    /// <summary>
    /// Trigger screen shake when the slam occurs
    /// </summary>
    public void triggerSlam()
    {
        armController.OnSlam();
    }

    /// <summary>
    /// Trigger resetting note hit flags when the slam ends
    /// </summary>
    public void triggerEndSlam()
    {
        armController.OnSlamEnd();
    }
}
