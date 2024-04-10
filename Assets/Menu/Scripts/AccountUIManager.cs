using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;

public class AccountUIManager : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the text containing the account name")] private TextMeshProUGUI accountNameText;

	private async void Start()
	{
		accountNameText.text = "";
        try
        {
            string playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            accountNameText.text = playerName[..^5];
		}
		catch (Exception e)
		{
			Debug.LogError("Failed to get player name: " + e.Message);
			accountNameText.text = "Account";
		}
	}
}
