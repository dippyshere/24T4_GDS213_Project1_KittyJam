using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeatAnimation : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton instance of the beat animation script")] public static BeatAnimation instance;
    [SerializeField, Tooltip("Default bpm of the song"), Min(1)] private float bpm = 128;
    [Tooltip("Current song data")] private SongData songData;
    [Tooltip("Current audio source to sync the beat to")] private AudioSource audioSource;
    [Tooltip("List of beat tweeners that animate on every beat")] private List<Tween> tweens1 = new List<Tween>();
    [Tooltip("List of beat tweeners that animate on every second beat")] private List<Tween> tweens2 = new List<Tween>();
    [Tooltip("List of beat tweeners that animate on every fourth beat")] private List<Tween> tweens4 = new List<Tween>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        DOTween.useSmoothDeltaTime = true;
        RefreshBeatObjects();
    }

    public void RefreshBeatObjects()
    {
        tweens1 = DOTween.TweensById("beat");
        tweens1 ??= new List<Tween>();
        tweens2 = DOTween.TweensById("beat2");
        tweens2 ??= new List<Tween>();
        tweens4 = DOTween.TweensById("beat4");
        tweens4 ??= new List<Tween>();
        StopAllCoroutines();
        StartCoroutine(Beat());
        StartCoroutine(Beat2());
        StartCoroutine(Beat4());
    }

    public void ChangeBeatDrivingSong(SongData songData, AudioSource audioSource)
    {
        this.songData = songData;
        this.audioSource = audioSource;
        bpm = songData.Bpm;
    }

    private IEnumerator Beat()
    {
        while (true)
        {
            if (audioSource != null)
            {
                if (audioSource.isPlaying)
                {
                    foreach (Tween tween in tweens1)
                    {
                        tween.timeScale = 60 / bpm;
                    }
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    foreach (Tween tween in tweens1)
                    {
                        tween.Restart();
                    }
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                // yield return null;
                foreach (Tween tween in tweens1)
                {
                    tween.timeScale = 60 / bpm;
                }
                yield return new WaitForSeconds(60 / bpm);
                foreach (Tween tween in tweens1)
                {
                    tween.Restart();
                }
            }
        }
    }

    private IEnumerator Beat2()
    {
        while (true)
        {
            if (audioSource != null)
            {
                if (audioSource.isPlaying)
                {
                    foreach (Tween tween in tweens2)
                    {
                        tween.timeScale = 60 / bpm;
                    }
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    foreach (Tween tween in tweens2)
                    {
                        tween.Restart();
                    }
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                // yield return null;
                foreach (Tween tween in tweens2)
                {
                    tween.timeScale = 60 / bpm;
                }
                yield return new WaitForSeconds(60 / bpm);
                yield return new WaitForSeconds(60 / bpm);
                foreach (Tween tween in tweens2)
                {
                    tween.Restart();
                }
            }
        }
    }

    private IEnumerator Beat4()
    {
        while (true)
        {
            if (audioSource != null)
            {
                if (audioSource.isPlaying)
                {
                    foreach (Tween tween in tweens4)
                    {
                        tween.timeScale = 60 / bpm;
                    }
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    yield return new WaitForSecondsRealtime((60 / bpm) - (audioSource.time % (60 / bpm)));
                    foreach (Tween tween in tweens4)
                    {
                        tween.Restart();
                    }
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                // yield return null;
                foreach (Tween tween in tweens4)
                {
                    tween.timeScale = 60 / bpm;
                }
                yield return new WaitForSeconds(60 / bpm);
                yield return new WaitForSeconds(60 / bpm);
                yield return new WaitForSeconds(60 / bpm);
                yield return new WaitForSeconds(60 / bpm);
                foreach (Tween tween in tweens4)
                {
                    tween.Restart();
                }
            }
        }
    }
}
