using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class LoginManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the login manager")] public static LoginManager Instance;
    [Header("Configuration")]
    [SerializeField, Tooltip("The scene load info to use once successfully logged in")] private SceneLoadInfo sceneLoadInfo;
    [Header("References")]
    [SerializeField, Tooltip("Reference to the gameobject that contains the Login UI")] private GameObject loginPanel;
    [SerializeField, Tooltip("Reference to the login username text input field")] private TMP_InputField loginUsername;
    [SerializeField, Tooltip("Reference to the login password text input field")] private TMP_InputField loginPassword;
    [SerializeField, Tooltip("Reference to the gameobject that contains the Signup UI")] private GameObject signupPanel;
    [SerializeField, Tooltip("Reference to the signup username text input field")] private TMP_InputField signupUsername;
    [SerializeField, Tooltip("Reference to the signup password text input field")] private TMP_InputField signupPassword;
    [SerializeField, Tooltip("Reference to the switch page button text")] private TextMeshProUGUI switchPageText;
    [SerializeField, Tooltip("Reference to the button that logs in the user")] private Button loginButton;
    [SerializeField, Tooltip("Reference to the button that signs up the user")] private Button signupButton;
    [SerializeField, Tooltip("Reference to the text that displays debugging information")] private TextMeshProUGUI debugText;
    [SerializeField, Tooltip("Reference to the profile icon selection UI")] private GameObject profileIconSelectionUI;
    [Tooltip("The currently visible panel")] private bool isLoginPanelActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        debugText.text = "";
        isLoginPanelActive = false;
    }

    public void OnPanelToggle()
    {
        isLoginPanelActive = !isLoginPanelActive;
        loginPanel.SetActive(isLoginPanelActive);
        signupPanel.SetActive(!isLoginPanelActive);
        switchPageText.text = isLoginPanelActive ? "Sign up" : "Log in";
    }

    public async void OnLogin()
    {
        if (loginUsername.text.Length < 3)
        {
            debugText.text = "Username must be at least 3 characters long.";
            return;
        }
        if (loginPassword.text.Length < 8)
        {
            debugText.text = "Password must be at least 8 characters long.";
            return;
        }
        debugText.text = "Logging in...";
        DisableAllInput();
        await KittyAccountManager.Instance.SignInWithUsernamePasswordAsync(loginUsername.text, loginPassword.text);
        if (AuthenticationService.Instance.IsSignedIn)
        {
            debugText.text = "Login successful!\nAccount ID: " + AuthenticationService.Instance.PlayerId + "\nName: " + AuthenticationService.Instance.PlayerName + "\nAccess Token: " + AuthenticationService.Instance.AccessToken;
            if (DownloadManager.Instance != null)
            {
                DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
            }
        }
        else
        {
            debugText.text = "Login failed. Please try again.";
            EnableAllInput();
        }
    }

    public async void OnSignup()
    {
        if (signupUsername.text.Length < 3)
        {
            debugText.text = "Username must be at least 3 characters long.";
            return;
        }
        if (signupPassword.text.Length < 8)
        {
            debugText.text = "Password must be at least 8 characters long.";
            return;
        }
        if (!Regex.IsMatch(signupPassword.text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$"))
        {
            debugText.text = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 symbol.";
            return;
        }
        debugText.text = "Signing up...";
        DisableAllInput();
        await KittyAccountManager.Instance.SignUpWithUsernamePasswordAsync(signupUsername.text, signupPassword.text);
        if (AuthenticationService.Instance.IsSignedIn)
        {
            debugText.text = "Signup successful!\nAccount ID: " + AuthenticationService.Instance.PlayerId + "\nName: " + AuthenticationService.Instance.PlayerName + "\nAccess Token: " + AuthenticationService.Instance.AccessToken;
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(signupUsername.text);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            if (DownloadManager.Instance != null)
            {
                //DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
                profileIconSelectionUI.SetActive(true);
                gameObject.SetActive(false);
            }
        }
        else
        {
            debugText.text = "Signup failed. Please try again.";
            EnableAllInput();
        }
    }

    public async void OnGuestAccount()
    {
        debugText.text = "Creating guest account...";
        DisableAllInput();
        await KittyAccountManager.Instance.SignInAnonymouslyAsync();
        if (AuthenticationService.Instance.IsSignedIn)
        {
            debugText.text = "Signup successful!\nAccount ID: " + AuthenticationService.Instance.PlayerId + "\nAccess Token: " + AuthenticationService.Instance.AccessToken;
            if (DownloadManager.Instance != null)
            {
                DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
            }
        }
        else
        {
            debugText.text = "Signup failed. Please try again.";
            EnableAllInput();
        }
    }

    private void DisableAllInput()
    {
        loginUsername.interactable = false;
        loginPassword.interactable = false;
        signupUsername.interactable = false;
        signupPassword.interactable = false;
        switchPageText.transform.parent.GetComponent<Button>().interactable = false;
        loginButton.interactable = false;
        signupButton.interactable = false;
    }

    private void EnableAllInput()
    {
        loginUsername.interactable = true;
        loginPassword.interactable = true;
        signupUsername.interactable = true;
        signupPassword.interactable = true;
        switchPageText.transform.parent.GetComponent<Button>().interactable = true;
        loginButton.interactable = true;
        signupButton.interactable = true;
    }
}
