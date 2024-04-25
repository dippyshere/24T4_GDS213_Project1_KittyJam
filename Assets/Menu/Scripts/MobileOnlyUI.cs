using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileOnlyUI : MonoBehaviour
{
    [SerializeField, Tooltip("The image to enable when the game is running on mobile")] private Image mobileOnlyImage;
    [SerializeField, Tooltip("The image to disable when the game is running on mobile")] private Image desktopOnlyImage;
    [SerializeField, Tooltip("The sprite to disable when the game is running on mobile")] private SpriteRenderer mobileOnlySprite;

    void Start()
    {
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop || Application.isMobilePlatform)
        {
            if (mobileOnlyImage != null)
            {
                mobileOnlyImage.enabled = true;
            }
            if (mobileOnlySprite != null)
            {
                mobileOnlySprite.enabled = false;
            }
            if (desktopOnlyImage != null)
            {
                desktopOnlyImage.enabled = false;
            }
        }
        else
        {
            if (mobileOnlyImage != null)
            {
                mobileOnlyImage.enabled = false;
            }
            if (mobileOnlySprite != null)
            {
                mobileOnlySprite.enabled = true;
            }
            if (desktopOnlyImage != null)
            {
                desktopOnlyImage.enabled = true;
            }
        }
    }
}
