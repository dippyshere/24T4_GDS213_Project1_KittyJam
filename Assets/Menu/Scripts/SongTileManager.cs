using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the data and UI of a song tile in the song selection menu
/// </summary>
public class SongTileManager : MonoBehaviour
{
    [Header("Song Tile Data")]
    [SerializeField, Tooltip("The song data that this song tile represents")] public SongData songData;
    [SerializeField, Tooltip("Whether the song is locked or unlocked for the player")] public bool isLocked;
    [SerializeField, Tooltip("Whether the song is new for the player")] public bool isNew;
    [SerializeField, Tooltip("Whether this script is for a song tile or game selection tile")] public bool isSongTile = true;
    [SerializeField, Tooltip("The animator component for the song tile")] private Animator tileAnimatorController;
    [SerializeField, Tooltip("The Image component that will display the song's album art")] private Image albumArt;
    [SerializeField, Tooltip("The Text component that will display the song's title when hovered")] private TextMeshProUGUI songTitleHover;
    [SerializeField, Tooltip("The Text component that will display the song's title")] private TextMeshProUGUI songTitle;
    [SerializeField, Tooltip("The Text component that will display the song's artist")] private TextMeshProUGUI songArtist;
    [SerializeField, Tooltip("The icon for CircleGame compatibility & completion")] private GameObject circleGameIcon;
    [SerializeField, Tooltip("The icon for HighwayGame compatibility")] private GameObject highwayGameIcon;
    [SerializeField, Tooltip("The icon for MarchingGame compatibility")] private GameObject marchingGameIcon;
    [SerializeField, Tooltip("The icon for BongoGame compatibility")] private GameObject bongoGameIcon;
    [SerializeField, Tooltip("The icon for telling the player that they have not yet played the song")] private GameObject bangIcon;
    [SerializeField, Tooltip("The icon for telling the player that they cannot play the song yet")] private GameObject lockIcon;
    [Header("Material Properties")]
    [SerializeField, Tooltip("Material to use for the song image")] private Material songMaterial;
    [SerializeField, Tooltip("The intensity of the outer glow effect"), Range(0, 0.55f)] public float outerGlowIntensity = 0.3f;
    [SerializeField, Tooltip("The power of the outer glow effect"), Range(0, 50f)] public float outerGlowPower = 5f;
    [SerializeField, Tooltip("The power of the outline effect"), Range(0, 50f)] public float outlinePower = 10f;
    [Tooltip("Reference to the button component for the song tile")] private Button songTileButton;

#if UNITY_EDITOR
    void OnValidate()
    {
        //Start();
        albumArt.material = new Material(songMaterial);
        UpdateMaterial();
    }
#endif

    private void Start()
    {
        albumArt.material = new Material(songMaterial);
        songTileButton = GetComponent<Button>();
        if (isSongTile && songData != null)
        {
            albumArt.sprite = songData.AlbumCover;
            songTitle.text = songData.SongName;
            songArtist.text = songData.ArtistName;
            songTitleHover.text = songData.SongName;
            circleGameIcon.SetActive(false);
            highwayGameIcon.SetActive(false);
            marchingGameIcon.SetActive(false);
            bongoGameIcon.SetActive(false);
            // temporary for playtesting purposes, will reset on refresh
            if (GlobalVariables.Get<string>(songData.name + "New") != null)
            {
                isNew = false;
            }
            else
            {
                isNew = true;
            }
            if (isNew)
            {
                bangIcon.SetActive(true);
            }
            else
            {
                bangIcon.SetActive(false);
            }
            if (isLocked)
            {
                lockIcon.SetActive(true);
                songTileButton.interactable = false;
            }
            else
            {
                lockIcon.SetActive(false);
                songTileButton.interactable = true;
            }
            foreach (GameMode gameMode in songData.GameModes)
            {
                switch (gameMode.GameType)
                {
                    case GameType.CircleGame:
                        circleGameIcon.SetActive(true);
                        break;
                    case GameType.HighwayGame:
                        highwayGameIcon.SetActive(true);
                        break;
                    case GameType.MarchingGame:
                        marchingGameIcon.SetActive(true);
                        break;
                    case GameType.BongoGame:
                        bongoGameIcon.SetActive(true);
                        break;
                }
            }
        }
    }

    private void LateUpdate()
    {
        UpdateMaterial();
    }

    /// <summary>
    /// Updates the material properties of the song tile
    /// </summary>
    private void UpdateMaterial()
    {
        if (albumArt != null)
        {
            albumArt.materialForRendering.SetFloat("_Intensity", outerGlowIntensity);
            albumArt.materialForRendering.SetFloat("_Power", outerGlowPower);
            albumArt.materialForRendering.SetFloat("_OutlinePower", outlinePower);
            if (isLocked)
            {
                albumArt.materialForRendering.SetInt("_isLocked", 1);
            }
            else
            {
                albumArt.materialForRendering.SetInt("_isLocked", 0);
            }
        }
    }

    /// <summary>
    /// Called when the mouse hovers over the song tile to animate the BPM of the song, activate the song preview, and display the song title
    /// </summary>
    public void OnHover()
    {
        if (isSongTile && tileAnimatorController != null && tileAnimatorController.GetBool("AnimateWithBPM") == false && songData.Bpm > 0)
        {
            SongSelectionManager.instance.ChangeSong(this);
            tileAnimatorController.SetBool("AnimateWithBPM", true);
            tileAnimatorController.SetFloat("BPMMultiplier", songData.Bpm / 60f);
        }
    }

    /// <summary>
    /// Called when the mouse clicks on the song tile to select the song and transition to the game selection menu
    /// </summary>
    public void OnClick()
    {
        if (isSongTile && songData != null)
        {
            GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            tileAnimatorController.SetBool("Clicked", true);
            albumArt.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            albumArt.rectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            songTitle.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            songTitle.rectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            SongSelectionManager.instance.SelectSong(songData);
        }
    }

    /// <summary>
    /// Resets the bpm animation of the song tile to synchronize with the song's bpm
    /// </summary>
    public void StartBPM()
    {
        tileAnimatorController.SetBool("ResetBPM", true);
    }

    /// <summary>
    /// Unused
    /// </summary>
    public void OnExit()
    {
        if (tileAnimatorController != null)
        {
            return;
        }
    }

    /// <summary>
    /// Stops the bpm animation of the song tile
    /// </summary>
    public void StopBPM()
    {
        if (tileAnimatorController != null)
        {
            tileAnimatorController.SetBool("AnimateWithBPM", false);
            tileAnimatorController.SetBool("ResetBPM", false);
        }
    }

    /// <summary>
    /// Updates the new badge of the song tile
    /// </summary>
    /// <param name="enableBadge">Set to true to enable the new badge, false to disable it</param>
    public void UpdateNewBadge(bool enableBadge)
    {
        isNew = !enableBadge;
        if (isNew)
        {
            bangIcon.SetActive(true);
        }
        else
        {
            bangIcon.SetActive(false);
        }
    }
}
