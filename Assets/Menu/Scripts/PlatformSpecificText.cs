using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlatformSpecificText : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the text mesh component containing the text")] private TextMeshProUGUI textMesh;
    [SerializeField, Tooltip("The text to use when the game is running on mobile")] private string mobileText;
    [SerializeField, Tooltip("The text to use when the game is running on desktop")] private string desktopText;

    void Start()
    {
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop || Application.isMobilePlatform)
        {
            textMesh.text = mobileText;
        }
        else
        {
            textMesh.text = desktopText;
        }
    }
}
