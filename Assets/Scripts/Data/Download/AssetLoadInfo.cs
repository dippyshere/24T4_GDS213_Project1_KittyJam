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

    /// <summary>
    /// Handles the equality comparison for the asset load info
    /// </summary>
    /// <param name="obj">The object to compare to</param>
    /// <returns>The result of the comparison</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AssetLoadInfo other = (AssetLoadInfo)obj;
        return assetsToLoad.Count == other.assetsToLoad.Count && assetLabelsToLoad.Count == other.assetLabelsToLoad.Count &&
            assetsToLoad.TrueForAll(asset => other.assetsToLoad.Contains(asset)) && assetLabelsToLoad.TrueForAll(label => other.assetLabelsToLoad.Contains(label));
    }

    /// <summary>
    /// Handles the hash code generation for the asset load info
    /// </summary>
    /// <returns>The hash code for the asset load info</returns>
    public override int GetHashCode()
    {
        return assetsToLoad.GetHashCode() ^ assetLabelsToLoad.GetHashCode();
    }
}

/// <summary>
/// An entry in the assets to load list that allows for specifying an asset label to load, as well as the option to release the assets after downloading.
/// </summary>
[System.Serializable]
public class AssetReferenceLoadInfo
{
    [Tooltip("The label to load assets from")] public AssetReference assetReference;
    [Tooltip("Whether the asset should be released after downloading.")] public bool releaseAssetsAfterDownload = true;

    /// <summary>
    /// Handles the equality comparison for the asset reference load info
    /// </summary>
    /// <param name="obj">The object to compare to</param>
    /// <returns>The result of the comparison</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AssetReferenceLoadInfo other = (AssetReferenceLoadInfo)obj;
        return assetReference == other.assetReference && releaseAssetsAfterDownload == other.releaseAssetsAfterDownload;
    }

    /// <summary>
    /// Handles the hash code generation for the asset reference load info
    /// </summary>
    /// <returns>The hash code for the asset reference load info</returns>
    public override int GetHashCode()
    {
        return assetReference.GetHashCode() ^ releaseAssetsAfterDownload.GetHashCode();
    }
}


/// <summary>
/// An entry in the asset labels to load list that allows for specifying an asset label to load, as well as the option to release the assets after downloading.
/// </summary>
[System.Serializable]
public class AssetLabelReferenceLoadInfo
{
    [Tooltip("The label to load assets from")] public AssetLabelReference assetLabel;
    [Tooltip("Whether the assets should be released after downloading.")] public bool releaseAssetsAfterDownload = true;

    /// <summary>
    /// Handles the equality comparison for the asset label reference load info
    /// </summary>
    /// <param name="obj">The object to compare to</param>
    /// <returns>The result of the comparison</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AssetLabelReferenceLoadInfo other = (AssetLabelReferenceLoadInfo)obj;
        return assetLabel == other.assetLabel && releaseAssetsAfterDownload == other.releaseAssetsAfterDownload;
    }

    /// <summary>
    /// Handles the hash code generation for the asset label reference load info
    /// </summary>
    /// <returns>The hash code for the asset label reference load info</returns>
    public override int GetHashCode()
    {
        return assetLabel.GetHashCode() ^ releaseAssetsAfterDownload.GetHashCode();
    }
}
