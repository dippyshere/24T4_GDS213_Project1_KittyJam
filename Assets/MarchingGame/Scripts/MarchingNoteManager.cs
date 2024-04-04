using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarchingNoteManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the marching note manager")] public static MarchingNoteManager Instance;
    [Header("Game Configuration")]
    [Tooltip("The Y coordinate that beat notes spawn at")] public float beatNoteSpawnY = 300f;
    [Tooltip("The Y coordinate that beat notes should be tapped at")] public float beatNoteTapY = 0;
    [Header("References")]
    [SerializeField, Tooltip("Reference to the beat managers")] private MarchingBeat[] marchingBeats;
    [SerializeField, Tooltip("Beat notes canvas group")] private CanvasGroup beatNotesCanvasGroup;
    [SerializeField, Tooltip("The prefab to spawn for beat note feedback")] private GameObject beatNoteFeedbackPrefab;
    [SerializeField, Tooltip("The prefab to spawn for wand note feedback")] private GameObject wandNoteFeedbackPrefab;
    [HideInInspector, Tooltip("The duration that a marching beat note will be on screen for")] public float beatNoteTime = 0.5f;
    [HideInInspector, Tooltip("The calculated Y coordinate beat notes despawn at")]
    public float beatNoteDespawnY
    {
        get
        {
            return beatNoteTapY - (beatNoteSpawnY - beatNoteTapY);
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

        yield return new WaitUntil(() => MarchingPlayer.Instance != null);

        foreach (MarchingLane lane in MarchingPlayer.Instance.marchingLanes) lane.SetTimeStamps(SongManager.Instance.noteTimestamps);

        float beatTime = 60f / SongManager.Instance.bpm;
        float time = SongManager.Instance.songDelayInSeconds;

        List<double> upBeatTimestamps = new List<double>();
        List<double> downBeatTimestamps = new List<double>();
        while (time < SongManager.midiFile.GetDuration<MetricTimeSpan>().TotalSeconds)
        {
            upBeatTimestamps.Add(time);
            time += beatTime;
            downBeatTimestamps.Add(time);
            time += beatTime;
        }
        upBeatTimestamps.RemoveRange(0, 8);
        downBeatTimestamps.RemoveRange(0, 8);
        marchingBeats[0].SetTimeStamps(upBeatTimestamps);
        marchingBeats[1].SetTimeStamps(downBeatTimestamps);

        yield return new WaitUntil(() => ScoreManager.Instance != null);
        ScoreManager.Instance.noteScoreEvent += ShowNoteFeedback;
        ScoreManager.Instance.alternateNoteScoreEvent += ShowAlternateNoteFeedback;
        ScoreManager.Instance.comboEvent += UpdateComboTransparency;

        yield return new WaitUntil(() => CursorController.Instance != null);
        CursorController.Instance.LockCursor();

        beatNoteTime = 60f / SongManager.Instance.bpm * 2 * SongManager.Instance.trackSpeed;
        MarchingPlayer.Instance.catAnimator.SetFloat("Speed", SongManager.Instance.bpm / 109.0909090909f);
        yield return new WaitForSecondsRealtime((0.2833333333f * (SongManager.Instance.bpm / 109.0909090909f)) + SongManager.Instance.songDelayInSeconds);
        MarchingPlayer.Instance.catAnimator.SetTrigger("Marching");
        MarchingPlayer.Instance.SetSplineMovementSpeed(SongManager.Instance.bpm / 36.6666666667f);
    }

    /// <summary>
    /// Hit the up beat lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane1(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        if (context.started)
        {
            marchingBeats[0].Hit();
        }
    }

    /// <summary>
    /// Hit the down beat lane
    /// </summary>
    /// <param name="context">The input context</param>
    public void HitLane2(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        if (context.started)
        {
            marchingBeats[1].Hit();
        }
    }

    /// <summary>
    /// Handles the mouse input for the wand movement on the player controller
    /// </summary>
    /// <param name="context">The input context</param>
    public void WandMovement(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        if (MarchingPlayer.Instance != null)
        MarchingPlayer.Instance.HandleMouseInput(context);
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
            case NoteFeedback.Perfect:
                GameObject noteFeedbackObject = Instantiate(beatNoteFeedbackPrefab, position, beatNotesCanvasGroup.transform.rotation, beatNotesCanvasGroup.transform);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
            case NoteFeedback.Good:
                noteFeedbackObject = Instantiate(beatNoteFeedbackPrefab, position, beatNotesCanvasGroup.transform.rotation, beatNotesCanvasGroup.transform);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
            case NoteFeedback.Miss:
                noteFeedbackObject = Instantiate(beatNoteFeedbackPrefab, position, beatNotesCanvasGroup.transform.rotation, beatNotesCanvasGroup.transform);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
            case NoteFeedback.TooEarly:
                noteFeedbackObject = Instantiate(beatNoteFeedbackPrefab, position, beatNotesCanvasGroup.transform.rotation, beatNotesCanvasGroup.transform);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
        }
    }

    /// <summary>
    /// Show feedback for the player's alernate note timing at a specific position (wand notes in marching game)
    /// </summary>
    /// <param name="feedback">The type of feedback to display</param>
    /// <param name="position">The position to display the feedback at</param>
    public void ShowAlternateNoteFeedback(NoteFeedback feedback, Vector3 position)
    {
        switch (feedback)
        {
            case NoteFeedback.Perfect:
                GameObject noteFeedbackObject = Instantiate(wandNoteFeedbackPrefab, position, Camera.main.transform.rotation);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
            case NoteFeedback.Good:
                noteFeedbackObject = Instantiate(wandNoteFeedbackPrefab, position, Camera.main.transform.rotation);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
            case NoteFeedback.Miss:
                noteFeedbackObject = Instantiate(wandNoteFeedbackPrefab, position, Camera.main.transform.rotation);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
            case NoteFeedback.TooEarly:
                noteFeedbackObject = Instantiate(wandNoteFeedbackPrefab, position, Camera.main.transform.rotation);
                noteFeedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
                break;
        }
    }

    public void UpdateComboTransparency(long combo)
    {
        beatNotesCanvasGroup.alpha = Mathf.Lerp(0.75f, 0.25f, Mathf.Clamp01(combo / 8f));
    }
}
