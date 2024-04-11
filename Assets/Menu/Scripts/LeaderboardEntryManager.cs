using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards.Models;

/// <summary>
/// Controls the appearance of the leaderboard entry, and handles adding correct stats to the entry.
/// </summary>
public class LeaderboardEntryManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Reference to the username text")] private TextMeshProUGUI usernameText;
    [SerializeField, Tooltip("Reference to the score text")] private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Reference to the rank text")] private TextMeshProUGUI rankText;
    [SerializeField, Tooltip("Reference to the crown icon")] private Image crownIcon;
    [SerializeField, Tooltip("Reference to the background image")] private Image background;
    [SerializeField, Tooltip("The material variant to use for the first place crown")] private Material firstPlaceCrownMaterial;
    [SerializeField, Tooltip("The material variant to use for the second place crown")] private Material secondPlaceCrownMaterial;
    [SerializeField, Tooltip("The material variant to use for the third place crown")] private Material thirdPlaceCrownMaterial;
    [Header("Configuration")]
    [SerializeField, Tooltip("Colour of the background when the entry is the player's entry and the number is odd")] private Color playerColorOdd;
    [SerializeField, Tooltip("Colour of the background when the entry is the player's entry and the number is even")] private Color playerColorEven;
    [SerializeField, Tooltip("Colour of the background when the entry is not the player's entry and the number is odd")] private Color otherColorOdd;
    [SerializeField, Tooltip("Colour of the background when the entry is not the player's entry and the number is even")] private Color otherColorEven;

    void Start()
    {
        // Set the background colour based on the entry number
        if (transform.GetSiblingIndex() % 2 == 0)
        {
            background.color = otherColorEven;
        }
        else
        {
            background.color = otherColorOdd;
        }
    }

    public void SetProperties(LeaderboardEntry leaderboardEntry)
    {
        string playerName = leaderboardEntry.PlayerName;
        if (playerName.Contains("#"))
        {
            playerName = playerName[..playerName.IndexOf("#")];
        }
        usernameText.text = playerName;
        scoreText.text = leaderboardEntry.Score.ToString("N0", CultureInfo.InvariantCulture);
        rankText.text = (leaderboardEntry.Rank + 1).ToString() + "<sup>" + GetRankSuffix(leaderboardEntry.Rank + 1) + "</sup>";

        switch (leaderboardEntry.Rank + 1)
        {
            case 1:
                crownIcon.material = firstPlaceCrownMaterial;
                RectTransform rt = GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 64);
                break;
            case 2:
                crownIcon.material = secondPlaceCrownMaterial;
                rt = GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 58);
                break;
            case 3:
                crownIcon.material = thirdPlaceCrownMaterial;
                rt = GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 52);
                break;
            default:
                crownIcon.enabled = false;
                rt = GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 48);
                break;
        }

        if (leaderboardEntry.PlayerId == AuthenticationService.Instance.PlayerId)
        {
            if (transform.GetSiblingIndex() % 2 == 0)
            {
                background.color = playerColorEven;
            }
            else
            {
                background.color = playerColorOdd;
            }
        }
    }

    private string GetRankSuffix(int rank)
    {
        if (rank == 11 || rank == 12 || rank == 13)
        {
            return "th";
        }
        switch (rank % 10)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }
}
