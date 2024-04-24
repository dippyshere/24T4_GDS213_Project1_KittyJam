using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the scrolling background based on the parent RectTransform's position
/// </summary>
public class UpdateScrollingBackground : MonoBehaviour
{
    [Tooltip("Reference to the parent rect transform component")] private RectTransform parentTransform;
    [Tooltip("Reference to the raw image component")] private RawImage rawImage;
    [SerializeField, Tooltip("The selected background scrolling behaviour to use")] private BackgroundBehaviour behaviour;
    [Tooltip("Stores the previous frame time to smooth time since startup")] private double previousFrameTime;
    [Tooltip("Stores the current frame time to smooth time since startup")] private double currentFrameTime;

#if UNITY_EDITOR
    void OnValidate()
    {
        Start();
        previousFrameTime = Time.realtimeSinceStartup;
        UpdateMaterial();
    }
#endif

    private void Start()
    {
        parentTransform = transform.parent.GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
    }

    void LateUpdate()
    {
        previousFrameTime = Time.realtimeSinceStartup;
        UpdateMaterial();
    }

    /// <summary>
    /// Updates the material of the raw image based on the parent RectTransform's position
    /// </summary>
    void UpdateMaterial()
    {
        if (rawImage != null && parentTransform != null && rawImage.texture != null)
        {
            float imageScaleFactor = 1f;
            if (rawImage.texture.width != 0)
            {
                imageScaleFactor = 2048f / rawImage.texture.width;
            }

            float scale = 13f / imageScaleFactor;

            currentFrameTime = 0.2f * previousFrameTime + 0.8f * previousFrameTime;

            switch (behaviour)
            {
                case BackgroundBehaviour.Scroll:
                    rawImage.uvRect = new Rect((float)(currentFrameTime * 0.05), (float)(currentFrameTime * 0.05), parentTransform.rect.width / rawImage.texture.width * scale, parentTransform.rect.height / rawImage.texture.height * scale);
                    break;
                case BackgroundBehaviour.Parallax:
                    rawImage.uvRect = new Rect(parentTransform.anchoredPosition.x / 157.56f, parentTransform.anchoredPosition.y / 157.56f, parentTransform.rect.width / rawImage.texture.width * scale, parentTransform.rect.height / rawImage.texture.height * scale);
                    break;
                case BackgroundBehaviour.Both:
                    rawImage.uvRect = new Rect((float)(currentFrameTime * 0.05) + parentTransform.anchoredPosition.x / 157.56f, (float)(currentFrameTime * 0.05) + parentTransform.anchoredPosition.y / 157.56f, parentTransform.rect.width / rawImage.texture.width * scale, parentTransform.rect.height / rawImage.texture.height * scale);
                    break;
            }
        }
    }
}

/// <summary>
/// The selected background scrolling behaviour to use
/// </summary>
public enum BackgroundBehaviour
{
    Scroll,
    Parallax,
    Both
}
