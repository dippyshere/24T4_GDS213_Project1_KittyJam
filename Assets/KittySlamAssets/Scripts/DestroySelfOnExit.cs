// Jacob wrote this for our last project in 23T2 Project 2

using UnityEngine;

/// <summary>
/// Destroys the parent GameObject of the animator when the animation is complete
/// </summary>
public class DestroySelfOnExit : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject, stateInfo.length * stateInfo.speed);
    }
}