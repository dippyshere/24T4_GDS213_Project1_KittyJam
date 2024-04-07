using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the dissolve effect of the highway
/// </summary>
public class HighwayDissolveManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the highway dissolve controller")] public static HighwayDissolveManager Instance;
    [SerializeField, Tooltip("The speed at which the dissolve effect occurs at")] private float dissolveSpeed = 1;

    [Tooltip("Reference to the material being used to control the dissolve effect")] private Material _material;

    [Tooltip("Caches the dissolve ammount property id for faster performance")] private int _dissolveAmmount = Shader.PropertyToID("_DissolveAmount");

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<RawImage>().material;
        _material.SetFloat(_dissolveAmmount, 1.1f);
    }

    /// <summary>
    /// Coroutine that makes the highway appear
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator AppearCoroutine()
    {
        float dissolve = 1.1f;
        while (dissolve > 0)
        {
            dissolve -= Time.deltaTime * dissolveSpeed;
            _material.SetFloat(_dissolveAmmount, dissolve);
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine that makes the highway disappear
    /// </summary>
    /// <returns>The IEnumerator for the coroutine</returns>
    private IEnumerator DissolveCoroutine()
    {
        float dissolve = 0;
        while (dissolve < 1.1)
        {
            dissolve += Time.deltaTime * dissolveSpeed;
            _material.SetFloat(_dissolveAmmount, dissolve);
            yield return null;
        }
    }

    /// <summary>
    /// Makes the highway appear
    /// </summary>
    public void Appear()
    {
        StartCoroutine(AppearCoroutine());
    }

    /// <summary>
    /// Makes the highway disappear
    /// </summary>
    public void Dissolve()
    {
        StartCoroutine(DissolveCoroutine());
    }
}
