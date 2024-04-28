using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpecificSprite : MonoBehaviour
{
    [SerializeField, Tooltip("The sprite to use when the game is running on mobile")] private SpriteRenderer mobileSprite;
    [SerializeField, Tooltip("The sprite to use when the game is running on desktop")] private SpriteRenderer desktopSprite;

    void Start()
    {
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop || Application.isMobilePlatform)
        {
            if (mobileSprite != null)
            {
                mobileSprite.enabled = true;
                mobileSprite.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            }
            if (desktopSprite != null)
            {
                desktopSprite.enabled = false;
                desktopSprite.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            }
        }
        else
        {
            if (mobileSprite != null)
            {
                mobileSprite.enabled = false;
                mobileSprite.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            }
            if (desktopSprite != null)
            {
                desktopSprite.enabled = true;
                desktopSprite.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            }
        }
    }
}
