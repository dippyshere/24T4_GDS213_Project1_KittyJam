using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    [SerializeField] private SpriteRenderer visualSprite;

    void Start()
    {
        timeInstantiated = SongManager.Instance.GetAudioSourceTime();
        transform.localPosition = Vector3.forward * HighwayNoteManager.Instance.noteSpawnZ;
        Invoke(nameof(OnMiss), (float)(SongManager.Instance.noteTime + ScoreManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = SongManager.Instance.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(Vector3.forward * (HighwayNoteManager.Instance.noteDespawnZ - HighwayNoteManager.Instance.noteSpawnZ) * Time.deltaTime / (SongManager.Instance.noteTime * 2));
        }
    }

    public void OnMiss()
    {
        ScoreManager.Instance.Miss(transform.position);
        visualSprite.color = new Color(0.8f, 0.45f, 0.45f, 0.45f);
    }
}
