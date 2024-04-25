using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of the beat lines on the highway
/// </summary>
public class HighwayBeatController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = Vector3.forward * HighwayNoteManager.Instance.noteSpawnZ;
        Invoke(nameof(DeleteBeat), SongManager.Instance.noteTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * Time.smoothDeltaTime * Vector3.forward / (SongManager.Instance.noteTime * 2));
    }

    /// <summary>
    /// Deletes the beat line when it reaches the end of the highway
    /// </summary>
    private void DeleteBeat()
    {
        Destroy(gameObject);
    }
}
