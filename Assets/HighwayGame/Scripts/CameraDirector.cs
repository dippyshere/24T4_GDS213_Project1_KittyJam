using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the camera animations
/// </summary>
public class CameraDirector : MonoBehaviour
{
    [Tooltip("Reference to the animator component")] private Animator animator => GetComponent<Animator>();
    [Tooltip("Index of the previous animation that was played")] private int previousAnimation = 1;

    private void Start()
    {
        StartCoroutine(PickRandomAnimation());
    }

    /// <summary>
    /// Picks a random camera animation to play every 15 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator PickRandomAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(15);
            int newAnimation = Random.Range(1, 6);
            while (newAnimation == previousAnimation)
            {
                newAnimation = Random.Range(1, 6);
            }
            previousAnimation = newAnimation;
            animator.SetBool("Cam " + newAnimation, true);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
