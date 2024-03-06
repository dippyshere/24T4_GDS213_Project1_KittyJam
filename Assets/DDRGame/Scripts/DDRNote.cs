using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDRNote : MonoBehaviour
{
    double timeInstantiated;
    public float assignedTime;
    private RectTransform rectTransform => GetComponent<RectTransform>();

    void Start()
    {
        timeInstantiated = DDRSongManager.GetAudioSourceTime();
        rectTransform.localPosition = Vector3.down * DDRSongManager.Instance.noteSpawnY;
        Invoke(nameof(OnMiss), (float)(DDRSongManager.Instance.noteTime + DDRSongManager.Instance.goodRange));
    }

    // Update is called once per frame
    void Update()
    {
        double timeSinceInstantiated = DDRSongManager.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (DDRSongManager.Instance.noteTime * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            rectTransform.localPosition += Vector3.down * (DDRSongManager.Instance.noteDespawnY - DDRSongManager.Instance.noteSpawnY) * Time.deltaTime / (DDRSongManager.Instance.noteTime * 2);
        }
    }

    public void OnMiss()
    {
        DDRScoreManager.Instance.Miss(gameObject.transform.position);
    }
}
