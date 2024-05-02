using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenMobileButton : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the DOTween animator component for the button")] private DOTweenAnimation DOTweenAnimation;
    [Tooltip("Time that the button was clicked last")] private double timeSinceClick;
    [Tooltip("Time that the button was pressed down last")] private double timeSinceDown;

    public void ButtonPointerExit() 
    {
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop || Application.isMobilePlatform)
        {
            if (Time.timeAsDouble - timeSinceClick > 0.25)
            {
                DOTweenAnimation.DOPlayBackwards();
            }
        }
        else
        {
            DOTweenAnimation.DOPlayBackwards();
        }
    }

    public void ButtonPointerDown()
    {
        timeSinceDown = Time.timeAsDouble;
    }

    public void ButtonPointerClick()
    {
        timeSinceClick = Time.timeAsDouble;
        if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop || Application.isMobilePlatform)
        {
            DOTweenAnimation.DOComplete();
            if (Time.timeAsDouble - timeSinceDown > 0.25)
            {
                DOTweenAnimation.DORestart();
                DOTweenAnimation.DOPlayBackwards();
            }
            else
            {
                DOTweenAnimation.DOPlayBackwards();
            }
        }
    }
}
