using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;

public class AccountUIManager : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the text containing the account name")] private TextMeshProUGUI accountNameText;
    [SerializeField, Tooltip("Reference to the UI Image for the profile icon")] private Image profileIconImage;

	private async void Start()
	{
		accountNameText.text = "";
        try
        {
            string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            // if a # is present in the string, remove all characters after the # (including the #), otherwise use the full string
            if (playerName.Contains("#"))
            {
                playerName = playerName[..playerName.IndexOf("#")];
            }
            accountNameText.text = playerName;
        }
		catch (Exception e)
		{
			Debug.LogError("Failed to get player name: " + e.Message);
			accountNameText.text = "Account";
		}
        Dictionary<string, Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "profileIndex" }, new LoadOptions(new PublicReadAccessClassOptions()));
        if (playerData.TryGetValue("profileIndex", out Item keyName))
        {
            if (keyName.Value.GetAs<int>() > 0)
            {
                string profileIconPath = "Assets/Textures/ProfilePictures/" + keyName.Value.GetAs<int>() + ".png";
                Addressables.LoadAssetAsync<Sprite>(profileIconPath).Completed += (op) =>
                {
                    profileIconImage.sprite = op.Result;
                };
            }
        }
    }
}
