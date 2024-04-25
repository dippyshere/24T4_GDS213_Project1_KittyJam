using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Melanchall.DryWetMidi.Interaction;

/// <summary>
/// Handles the spawning of notes based on the MIDI file for Game Type 2 (HighwayGame), in addition to game specific behaviors and logic
/// </summary>
public class HighwayNoteManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the highway note manager")] public static HighwayNoteManager Instance;
    [Header("Game Configuration")]
    [SerializeField, Tooltip("Reference to the lane managers")] private HighwayLane[] highwayLanes;
    [Tooltip("The prefab to spawn when hitting a note with good timing")] public GameObject goodHitPrefab;
    [Tooltip("The prefab to spawn when hitting a note with perfect timing")] public GameObject perfectHitPrefab;
    [SerializeField, Tooltip("The prefab to use on up beat markers")] private GameObject upBeatPrefab;
    [SerializeField, Tooltip("The prefab to use on down beat markers")] private GameObject downBeatPrefab;
    [Tooltip("Timestamps for up beat markers")] private List<float> upBeatTimestamps = new List<float>();
    [Tooltip("Timestamps for down beat markers")] private List<float> downBeatTimestamps = new List<float>();
    [Tooltip("The current index of the up beat marker")] private int upBeatIndex = 0;
    [Tooltip("The current index of the down beat marker")] private int downBeatIndex = 0;
    [Tooltip("The Z coordinate that notes spawn at")] public float noteSpawnZ = 10f;
    [Tooltip("The Z coordinate that notes should be tapped at")] public float noteTapZ = -5f;
    [HideInInspector, Tooltip("The calculated Z coordinate notes despawn at")]
    public float noteDespawnZ
    {
        get
        {
            return noteTapZ - (noteSpawnZ - noteTapZ);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SongManager.Instance != null);
        yield return new WaitUntil(() => SongManager.Instance.noteTimestamps != null);

        foreach (HighwayLane lane in highwayLanes) lane.SetTimeStamps(SongManager.Instance.noteTimestamps);

        float beatTime = 60f / SongManager.Instance.bpm;
        float time = SongManager.Instance.songDelayInSeconds;

        while (time < SongManager.midiFile.GetDuration<MetricTimeSpan>().TotalSeconds)
        {
            upBeatTimestamps.Add(time);
            time += beatTime;
            for (int i = 0; i < 3; i++)
            {
                downBeatTimestamps.Add(time);
                time += beatTime;
            }
        }
        upBeatTimestamps.RemoveRange(0, 2);
        downBeatTimestamps.RemoveRange(0, 6);
        Invoke(nameof(HighwayAppear), Mathf.Clamp(SongManager.Instance.songDelayInSeconds + SongManager.Instance.firstNoteTime - 2f, 0, float.MaxValue));
        Invoke(nameof(HighwayDissolve), SongManager.Instance.songDelayInSeconds + SongManager.Instance.lastNoteTime + 1f);
        yield return new WaitUntil(() => ScoreManager.Instance != null);
        ScoreManager.Instance.noteScoreEvent += ShowNoteFeedback;
        yield return new WaitUntil(() => CursorController.Instance != null);
        CursorController.Instance.LockCursor();
    }

    private void Update()
    {
        if (upBeatIndex < upBeatTimestamps.Count)
        {
            if (SongManager.Instance.GetAudioSourceTime() >= upBeatTimestamps[upBeatIndex] - SongManager.Instance.noteTime)
            {
                Instantiate(upBeatPrefab, transform);
                upBeatIndex++;
            }
        }

        if (downBeatIndex < downBeatTimestamps.Count)
        {
            if (SongManager.Instance.GetAudioSourceTime() >= downBeatTimestamps[downBeatIndex] - SongManager.Instance.noteTime)
            {
                Instantiate(downBeatPrefab, transform);
                downBeatIndex++;
            }
        }
    }

    /// <summary>
    /// Hit the first lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane1(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        highwayLanes[0].Hit(context);
    }

    /// <summary>
    /// Hit the second lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane2(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        highwayLanes[1].Hit(context);
    }

    /// <summary>
    /// Hit the third lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane3(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        highwayLanes[2].Hit(context);
    }

    /// <summary>
    /// Hit the fourth lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane4(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        highwayLanes[3].Hit(context);
    }

    /// <summary>
    /// Hit the optional fifth lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane5(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0 || highwayLanes.Length < 5)
        {
            return;
        }
        highwayLanes[4].Hit(context);
    }


    /// <summary>
    /// Make the highway appear
    /// </summary>
    private void HighwayAppear()
    {
        HighwayDissolveManager.Instance.Appear();
    }
    /// <summary>
    /// Make the highway disappear
    /// </summary>
    private void HighwayDissolve()
    {
        HighwayDissolveManager.Instance.Dissolve();
    }

    /// <summary>
    /// Show feedback for the player's note timing at a specific position
    /// </summary>
    /// <param name="feedback">The type of feedback to display</param>
    /// <param name="position">The position to display the feedback at</param>
    public void ShowNoteFeedback(NoteFeedback feedback, Vector3 position)
    {
        switch (feedback)
        {
            case NoteFeedback.Good:
                GameObject feedbackObject = Instantiate(goodHitPrefab, position, Quaternion.identity);
                feedbackObject.layer = 7;
                foreach (Transform child in feedbackObject.transform)
                {
                    child.gameObject.layer = 7;
                }
                break;
            case NoteFeedback.Perfect:
                feedbackObject = Instantiate(perfectHitPrefab, position, Quaternion.identity);
                feedbackObject.layer = 7;
                foreach (Transform child in feedbackObject.transform)
                {
                    child.gameObject.layer = 7;
                }
                break;
        }
    }
}
