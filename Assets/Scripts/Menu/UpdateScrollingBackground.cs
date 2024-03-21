using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScrollingBackground : MonoBehaviour
{
    private RectTransform parentTransform;
    private RawImage rawImage;
    [SerializeField] private bool isScrolling = false;

#if UNITY_EDITOR
    void OnValidate()
    {
        Start();
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
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (rawImage != null && parentTransform != null)
        {
            if (isScrolling)
            {
                rawImage.uvRect = new Rect((float)(Time.realtimeSinceStartupAsDouble * 0.05), (float)(Time.realtimeSinceStartupAsDouble * 0.05), parentTransform.rect.width / rawImage.texture.width * 13, parentTransform.rect.height / rawImage.texture.height * 13);
            }
            else
            {
                rawImage.uvRect = new Rect(parentTransform.anchoredPosition.x / 157.56f, parentTransform.anchoredPosition.y / 157.56f, parentTransform.rect.width / rawImage.texture.width * 13, parentTransform.rect.height / rawImage.texture.height * 13);
            }
        }
    }
}
