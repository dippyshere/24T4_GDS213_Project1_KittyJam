using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHighwayResolution : MonoBehaviour
{
    private RawImage highwayRawImage;
    private int previousScreenHeight;

    private void Start()
    {
        highwayRawImage = GetComponent<RawImage>();
        StartCoroutine(UpdateResolution());
    }

    private IEnumerator UpdateResolution()
    {
        while (true)
        {
            if (Screen.height != previousScreenHeight)
            {
                previousScreenHeight = Screen.height;
                RenderTexture currentRT = highwayRawImage.texture as RenderTexture;
                currentRT.Release();
                currentRT.height = Screen.height;
                currentRT.width = Screen.height;
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
