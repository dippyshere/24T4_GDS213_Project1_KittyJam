using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the spawning and despawning of notes in the DDR game (game type 4/bongo game), as well as game specific behaviours and logic.
/// </summary>
public class DDRNoteManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the bongo note manager")] public static DDRNoteManager Instance;
    [Header("Game Configuration")]
    [SerializeField, Tooltip("Reference to the bongo lane managers")] private DDRLane[] DDRLanes;
    [SerializeField, Tooltip("The note feedback prefab to use")] private GameObject noteFeedbackPrefab;
    [Tooltip("The Y coordinate that notes spawn at")] public float noteSpawnZ = 10f;
    [Tooltip("The Y coordinate that notes should be tapped at")] public float noteTapY = 0f;
    [HideInInspector, Tooltip("The calculated Z coordinate notes despawn at")]
    public float noteDespawnZ
    {
        get
        {
            return noteTapY - (noteSpawnZ - noteTapY);
        }
    }
    [SerializeField] float seconds = 4;
    float timer = 0.0f;
    bool blueToGreen = true;
    bool greenToRed = false;
    bool redToBlue = false;
    int comboScore = 0;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SongManager.Instance != null);
        yield return new WaitUntil(() => SongManager.Instance.noteTimestamps != null);

        foreach (DDRLane lane in DDRLanes) lane.SetTimeStamps(SongManager.Instance.noteTimestamps);

        float beatTime = 60f / SongManager.Instance.bpm;
        float time = SongManager.Instance.songDelayInSeconds;

        yield return new WaitUntil(() => BongoCatController.Instance != null);
        float bopSpeed = SongManager.Instance.bpm / 60f;
        BongoCatController.Instance.bongoCatAnimator.SetFloat("BPMMultiplier", bopSpeed);
        yield return new WaitForSecondsRealtime(60f / SongManager.Instance.bpm);
        BongoCatController.Instance.bongoCatAnimator.SetTrigger("StartBop");

        yield return new WaitUntil(() => ScoreManager.Instance != null);
        ScoreManager.Instance.noteScoreEvent += ShowNoteFeedback;
        ScoreManager.Instance.comboEvent += UpdateComboCount;
        yield return new WaitUntil(() => CursorController.Instance != null);
        CursorController.Instance.LockCursor();
    }

    /// <summary>
    /// Update the combo count
    /// </summary>
    /// <param name="score"></param>
    public void UpdateComboCount(long score)
    {
        comboScore = (int)score;
    }

    private void Update()
    {
        if (BongoCatController.Instance == null || BongoCatController.Instance.renderObject == null || BongoCatController.Instance.renderObject.material)
        {
            return;
        }
        // bongo cat colour changing
        if (comboScore >= 50)
        {
            timer += Time.deltaTime / seconds;

            if (blueToGreen == true && greenToRed == false && redToBlue == false)
            {
                BongoCatController.Instance.renderObject.material.color = Color.Lerp(Color.blue, Color.green, timer);

                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    blueToGreen = false;
                    greenToRed = true;
                }
            }

            if (greenToRed == true && blueToGreen == false && redToBlue == false)
            {
                BongoCatController.Instance.renderObject.material.color = Color.Lerp(Color.green, Color.red, timer);

                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    greenToRed = false;
                    redToBlue = true;
                }
            }

            if (redToBlue == true && greenToRed == false && blueToGreen == false)
            {
                BongoCatController.Instance.renderObject.material.color = Color.Lerp(Color.red, Color.blue, timer);

                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    redToBlue = false;
                    blueToGreen = true;
                }
            }
        }
        if (comboScore >= 100)
        {
            timer += Time.deltaTime / seconds;

            if (blueToGreen == true && greenToRed == false && redToBlue == false)
            {
                BongoCatController.Instance.renderObject.material.color = Color.Lerp(Color.blue, Color.green, timer);
                BongoCatController.Instance.renderObject.material.SetColor("_EmissionColor", BongoCatController.Instance.renderObject.material.color * 1);
                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    blueToGreen = false;
                    greenToRed = true;
                }
            }

            if (greenToRed == true && blueToGreen == false && redToBlue == false)
            {
                BongoCatController.Instance.renderObject.material.color = Color.Lerp(Color.green, Color.red, timer);
                BongoCatController.Instance.renderObject.material.SetColor("_EmissionColor", BongoCatController.Instance.renderObject.material.color * 1);
                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    greenToRed = false;
                    redToBlue = true;
                }
            }

            if (redToBlue == true && greenToRed == false && blueToGreen == false)
            {
                BongoCatController.Instance.renderObject.material.color = Color.Lerp(Color.red, Color.blue, timer);
                BongoCatController.Instance.renderObject.material.SetColor("_EmissionColor", BongoCatController.Instance.renderObject.material.color * 1);
                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    redToBlue = false;
                    blueToGreen = true;
                }
            }
        }
        if (comboScore <= 49)
        {
            BongoCatController.Instance.renderObject.material.color = new Color(0.5f, 0.5f, 0.5f);
            BongoCatController.Instance.renderObject.material.SetColor("_EmissionColor", BongoCatController.Instance.renderObject.material.color);
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
        if (context.started)
        {
            DDRLanes[0].Hit();
            DDRLanes[0].ArmDown();
        }
        else if (context.canceled)
        {
            DDRLanes[0].ArmUp();
        }
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
        if (context.started)
        {
            DDRLanes[1].Hit();
            DDRLanes[1].ArmDown();
        }
        else if (context.canceled)
        {
            DDRLanes[1].ArmUp();
        }
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
        if (context.started)
        {
            DDRLanes[2].Hit();
            DDRLanes[2].ArmDown();
        }
        else if (context.canceled)
        {
            DDRLanes[2].ArmUp();
        }
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
        if (context.started)
        {
            DDRLanes[3].Hit();
            DDRLanes[3].ArmDown();
        }
        else if (context.canceled)
        {
            DDRLanes[3].ArmUp();
        }
    }

    /// <summary>
    /// Show feedback for the player's note timing at a specific position
    /// </summary>
    /// <param name="feedback">The type of feedback to display</param>
    /// <param name="position">The position to display the feedback at</param>
    public void ShowNoteFeedback(NoteFeedback feedback, Vector3 position)
    {
        GameObject feedbackObject = Instantiate(noteFeedbackPrefab, new Vector3(position.x, 0.05f, -6.777f), Camera.main.transform.rotation);
        feedbackObject.GetComponent<NoteFeedbackManager>().SetFeedbackType(feedback);
        feedbackObject.transform.localScale = Vector3.one * 0.3f;
    }
}
