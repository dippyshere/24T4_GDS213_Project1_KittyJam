using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using TMPro;

public class DownloadManager : MonoBehaviour
{
    [HideInInspector] public static DownloadManager Instance;
    [SerializeField, Tooltip("Reference to the UI text element containing the download text.")] private TextMeshProUGUI downloadText;
    [SerializeField, Tooltip("Reference to the UI slider element containing the download progress.")] private Slider downloadProgress;
    [SerializeField, Tooltip("Reference to the UI image element containing the cat animation.")] private Image catImage;

    private void Awake()
    {
        Instance = this;
    }
}
