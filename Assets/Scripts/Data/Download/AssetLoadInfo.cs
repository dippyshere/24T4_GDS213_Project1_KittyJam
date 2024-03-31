using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// A scriptable object that contains a list of addressable asset keys/labels to load
/// </summary>
[
    CreateAssetMenu(fileName = "AssetLoadInfo", menuName = "Kitty Jam/Download/Asset Load Info"),
    System.Serializable
]
public class AssetLoadInfo : ScriptableObject
{
    [Header("Asset Load Info")]
    [Tooltip("A list of addressable assets that should be loaded by the download manager")] public List<AssetReferenceLoadInfo> assetsToLoad = new List<AssetReferenceLoadInfo>();
    [Tooltip("A list of addressable asset labels that should be loaded by the download manager")] public List<AssetLabelReferenceLoadInfo> assetLabelsToLoad = new List<AssetLabelReferenceLoadInfo>();
}

/// <summary>
/// An entry in the assets to load list that allows for specifying an asset label to load, as well as the option to release the assets after downloading.
/// </summary>
[System.Serializable]
public class AssetReferenceLoadInfo
{
    [Tooltip("The label to load assets from")] public AssetReference assetReference;
    [Tooltip("Whether the asset should be released after downloading.")] public bool releaseAssetsAfterDownload = false;
}


/// <summary>
/// An entry in the asset labels to load list that allows for specifying an asset label to load, as well as the option to release the assets after downloading.
/// </summary>
[System.Serializable]
public class AssetLabelReferenceLoadInfo
{
    [Tooltip("The label to load assets from")] public AssetLabelReference assetLabel;
    [Tooltip("Whether the assets should be released after downloading.")] public bool releaseAssetsAfterDownload = false;
}
