using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BongoCatController : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the bongo cat controller")] public static BongoCatController Instance;
    [HideInInspector, Tooltip("The animator for the bongo cat")] public Animator bongoCatAnimator => GetComponent<Animator>();
    [SerializeField, Tooltip("Reference to the render object that controls the cat fur material")] public Renderer renderObject;

    private void Awake()
    {
        Instance = this;
    }

    public void EnableArm(BongoCatArm arm)
    {
        bongoCatAnimator.SetBool(arm.ToString(), true);
    }

    public void DisableArm(BongoCatArm arm)
    {
        bongoCatAnimator.SetBool(arm.ToString(), false);
    }
}

public enum BongoCatArm
{
    FarLeft,
    Left,
    Right,
    FarRight
}
