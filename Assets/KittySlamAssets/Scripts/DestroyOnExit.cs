// Jacob wrote this for our last project

using UnityEngine;

public class DestroyOnExit : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject.transform.parent.gameObject, stateInfo.length * stateInfo.speed); // Destroys GameObject after animation is complete
    }
}