using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeatAnimation : MonoBehaviour
{
    [SerializeField, Tooltip("Default bpm of the song"), Min(1)] private float bpm = 128;
    [Tooltip("List of beat tweeners that animate on every beat")] private List<Tween> tweens1 = new List<Tween>();
    [Tooltip("List of beat tweeners that animate on every second beat")] private List<Tween> tweens2 = new List<Tween>();
    [Tooltip("List of beat tweeners that animate on every fourth beat")] private List<Tween> tweens4 = new List<Tween>();

    private void Start()
    {
        tweens1 = DOTween.TweensById("beat");
        tweens2 = DOTween.TweensById("beat2");
        tweens4 = DOTween.TweensById("beat4");
        StartCoroutine(Beat());
        StartCoroutine(Beat2());
        StartCoroutine(Beat4());
    }

    private IEnumerator Beat()
    {
        while (true)
        {
            foreach (var tween in tweens1)
            {
                tween.timeScale = 60 / bpm;
            }
            yield return new WaitForSeconds(60 / bpm);
            foreach (var tween in tweens1)
            {
                tween.Restart();
            }
        }
    }

    private IEnumerator Beat2()
    {
        while (true)
        {
            foreach (var tween in tweens2)
            {
                tween.timeScale = 60 / bpm;
            }
            yield return new WaitForSeconds(60 / bpm);
            yield return new WaitForSeconds(60 / bpm);
            foreach (var tween in tweens2)
            {
                tween.Restart();
            }
        }
    }

    private IEnumerator Beat4()
    {
        while (true)
        {
            foreach (var tween in tweens4)
            {
                tween.timeScale = 60 / bpm;
            }
            yield return new WaitForSeconds(60 / bpm);
            yield return new WaitForSeconds(60 / bpm);
            yield return new WaitForSeconds(60 / bpm);
            yield return new WaitForSeconds(60 / bpm);
            foreach (var tween in tweens4)
            {
                tween.Restart();
            }
        }
    }
}
