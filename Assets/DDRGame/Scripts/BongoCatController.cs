using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BongoCatController : MonoBehaviour
{
    [Tooltip("The animator for the bongo cat")] private Animator bongoCatAnimator => GetComponent<Animator>();

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
