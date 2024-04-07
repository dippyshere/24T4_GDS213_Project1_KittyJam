using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the bongo cat's animations
/// </summary>
public class BongoCatController : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the bongo cat controller")] public static BongoCatController Instance;
    [HideInInspector, Tooltip("The animator for the bongo cat")] public Animator bongoCatAnimator => GetComponent<Animator>();
    [SerializeField, Tooltip("Reference to the render object that controls the cat fur material")] public Renderer renderObject;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Activates the hit animation for a specific arm
    /// </summary>
    /// <param name="arm">The arm to activate the hit animation for</param>
    public void EnableArm(BongoCatArm arm)
    {
        bongoCatAnimator.SetBool(arm.ToString(), true);
    }

    /// <summary>
    /// Deactivates the hit animation for a specific arm
    /// </summary>
    /// <param name="arm">The arm to deactivate the hit animation for</param>
    public void DisableArm(BongoCatArm arm)
    {
        bongoCatAnimator.SetBool(arm.ToString(), false);
    }
}

/// <summary>
/// The different hit positions for the bongo cat
/// </summary>
public enum BongoCatArm
{
    FarLeft,
    Left,
    Right,
    FarRight
}
