using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileOnlyUI : MonoBehaviour
{
    [SerializeField, Tooltip("The image to enable when the game is running on mobile")] private Image mobileOnlyImage;
    [SerializeField, Tooltip("The sprite to enable when the game is running on mobile")] private SpriteRenderer mobileOnlySprite;

    void Start()
    {
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop)
        {
            if (mobileOnlyImage != null)
            mobileOnlyImage.enabled = true;
            if (mobileOnlySprite != null)
            mobileOnlySprite.enabled = true;
        }
        else
        {
            if (mobileOnlyImage != null)
            mobileOnlyImage.enabled = false;
            if (mobileOnlySprite != null)
            mobileOnlySprite.enabled = false;
        }
    }
}
