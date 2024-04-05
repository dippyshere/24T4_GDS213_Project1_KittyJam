using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the post process manager instance")] public static PostProcessManager Instance;
    [Header("References")]
    [HideInInspector, Tooltip("Reference to the global post processing volume")] public Volume postProcessVolume;

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
        Instance = this;
    }
}
