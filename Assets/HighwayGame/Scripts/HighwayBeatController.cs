using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        transform.Translate(Vector3.forward * (HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * Time.deltaTime / (SongManager.Instance.noteTime * 2));
    }

    private void DeleteBeat()
    {
        Destroy(gameObject);
    }
}
