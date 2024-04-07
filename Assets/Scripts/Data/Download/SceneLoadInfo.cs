using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object that contains a list of scenes to load, scenes to unload, and options for whether to mark the first scene as the active scene after loading, and whether to use the transition or not.
/// </summary>
[
    CreateAssetMenu(fileName = "SceneLoadInfo", menuName = "Kitty Jam/Download/Scene Load Info"),
    System.Serializable
]
public class SceneLoadInfo : ScriptableObject
{
    [Header("Scene Load Info")]
    [Tooltip("A list of addressable scenes that should be loaded by the download manager")] public List<SceneAssetReference> scenesToLoad = new List<SceneAssetReference>();
    [Tooltip("A list of addressable scenes that should be unloaded before loading the scenes")] public List<SceneAssetReference> scenesToUnload = new List<SceneAssetReference>();
    [Header("Options")]
    [Tooltip("Whether the first scene in the scenes to load list should be marked as the active scene once it finishes loading.")] public bool markFirstSceneAsActive = true;
    [Tooltip("Whether the transition should be used while loading the scenes.")] public bool useTransition = true;

    /// <summary>
    /// Handles the equality comparison between two SceneLoadInfo objects
    /// </summary>
    /// <param name="obj">The object to compare against</param>
    /// <returns>The result of the equality comparison</returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SceneLoadInfo other = (SceneLoadInfo)obj;
        return scenesToLoad.Count == other.scenesToLoad.Count && scenesToUnload.Count == other.scenesToUnload.Count &&
            scenesToLoad.TrueForAll(scene => other.scenesToLoad.Contains(scene)) && scenesToUnload.TrueForAll(scene => other.scenesToUnload.Contains(scene)) &&
            markFirstSceneAsActive == other.markFirstSceneAsActive && useTransition == other.useTransition;
    }

    /// <summary>
    /// Handles the hash code generation for the SceneLoadInfo object
    /// </summary>
    /// <returns>The hash code for the SceneLoadInfo object</returns>
    public override int GetHashCode()
    {
        return scenesToLoad.GetHashCode() ^ scenesToUnload.GetHashCode() ^ markFirstSceneAsActive.GetHashCode() ^ useTransition.GetHashCode();
    }
}
