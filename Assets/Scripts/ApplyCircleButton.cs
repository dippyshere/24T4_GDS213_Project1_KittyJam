using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyCircleButton : MonoBehaviour
{
    [SerializeField] private Image image;

    private void Start()
    {
        if (image != null)
        {
            image.alphaHitTestMinimumThreshold = 0.5f;
        }
    }
}
