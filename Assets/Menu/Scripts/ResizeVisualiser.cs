using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeVisualiser : MonoBehaviour
{
    [SerializeField, Tooltip("List of spectrum visualisers to resize")] private SimpleSpectrum[] spectrums;
    [Tooltip("Reference to the previous screen width to detect changes in resolution")] private int previousScreenWidth;
    [Tooltip("Reference to the previous screen height to detect changes in resolution")] private int previousScreenHeight;

    private void Start()
    {
        StartCoroutine(UpdateResolution());
    }

    private IEnumerator UpdateResolution()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            if (Screen.width != previousScreenWidth || Screen.height != previousScreenHeight)
            {
                previousScreenWidth = Screen.width;
                previousScreenHeight = Screen.height;
                // set the barAmount to 64 for a 16:9 aspect ratio, scaling up/down by the aspect ratio width, then call RebuildSpectrum method
                // this should be purely based on aspect ratio, not resolution
                float aspectRatio = (float)Screen.width / Screen.height;
                foreach (SimpleSpectrum spectrum in spectrums)
                {
                    spectrum.barAmount = Mathf.RoundToInt(35.9999999996f * aspectRatio);
                    spectrum.RebuildSpectrum();
                }
            }
            yield return new WaitForSecondsRealtime(1.5f);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
