using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapBackground : MonoBehaviour
{
    private RectTransform parentTransform;
    private RawImage rawImage;
    [SerializeField] private bool isScrolling = false;
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetY = 0;

#if UNITY_EDITOR
    void OnValidate()
    {
        Start();
    }
#endif

    private void Start()
    {
        parentTransform = transform.parent.GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (rawImage != null && parentTransform != null)
        {
            if (isScrolling)
            {
                rawImage.uvRect = new Rect((float)(Time.realtimeSinceStartupAsDouble * 0.05), (float)(Time.realtimeSinceStartupAsDouble * 0.05), parentTransform.rect.width / rawImage.texture.width * 3.25f, parentTransform.rect.height / rawImage.texture.height * 3.25f);
            }
            else
            {
                rawImage.uvRect = new Rect(parentTransform.anchoredPosition.x + offsetX, parentTransform.anchoredPosition.y + offsetY, parentTransform.rect.width / rawImage.texture.width * 3.25f, parentTransform.rect.height / rawImage.texture.height * 3.25f);
            }
        }
    }
}
