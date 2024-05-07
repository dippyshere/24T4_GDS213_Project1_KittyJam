using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpriteLightProbes : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    private void Start()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        }
    }
}
