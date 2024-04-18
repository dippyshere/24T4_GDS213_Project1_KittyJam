using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the resolution of the highway to match the displayed resolution
/// </summary>
public class UpdateHighwayResolution : MonoBehaviour
{
    [Tooltip("Reference to the raw image used to display the highway on the canvas")] private RawImage highwayRawImage;
    [Tooltip("Reference to the previous screen height to detect changes in resolution")] private int previousScreenHeight;

    private void Start()
    {
        highwayRawImage = GetComponent<RawImage>();
        StartCoroutine(UpdateResolution());
        StartCoroutine(WaitUntilCanvasCameraExists());
    }

    private IEnumerator WaitUntilCanvasCameraExists()
    {
        highwayRawImage.enabled = false;
        while (true)
        {
            if (highwayRawImage.canvas.worldCamera != null)
            {
                break;
            }
            yield return null;
        }
        highwayRawImage.enabled = true;
    }

    private IEnumerator UpdateResolution()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            if (Screen.height != previousScreenHeight)
            {
                previousScreenHeight = Screen.height;
                RectTransform parentRectTransform = highwayRawImage.transform.parent.GetComponent<RectTransform>();
                RenderTexture currentRT = highwayRawImage.texture as RenderTexture;
                currentRT.Release();
                currentRT.height = (int)(Screen.height * 0.75f * 1.2f * (parentRectTransform.anchorMax.y - parentRectTransform.anchorMin.y));
                currentRT.width = (int)(Screen.height * 1.2f * (parentRectTransform.anchorMax.y - parentRectTransform.anchorMin.y));
                currentRT.Create();
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
