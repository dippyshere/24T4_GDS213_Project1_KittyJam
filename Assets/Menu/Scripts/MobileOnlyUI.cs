using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MobileOnlyUI : MonoBehaviour
{
    [SerializeField, Tooltip("The image to enable when the game is running on mobile")] private Image mobileOnlyImage;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            mobileOnlyImage.enabled = true;
        }
        else
        {
            mobileOnlyImage.enabled = false;
        }
    }
}
