using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    private Animator animator => GetComponent<Animator>();
    private int previousAnimation = 1;

    private void Start()
    {
        StartCoroutine(PickRandomAnimation());
    }

    private IEnumerator PickRandomAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
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
