using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using TMPro;
using DG.Tweening;

public class LoginManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the login manager")] public static LoginManager Instance;
    [Header("Configuration")]
    [SerializeField, Tooltip("The scene load info to use when advancing through menus")] private SceneLoadInfo sceneLoadInfo;
    [SerializeField, Tooltip("The scene load info to use when going back through menus")] private SceneLoadInfo backSceneLoadInfo;
    [Header("References")]
    [SerializeField, Tooltip("Reference to the username text input field")] private TMP_InputField usernameInput;
    [SerializeField, Tooltip("Reference to the password text input field")] private TMP_InputField passwordInput;
    [SerializeField, Tooltip("Reference to the continue button")] private Button continueButton;
    [SerializeField, Tooltip("reference to the continue button tweener")] private DOTweenAnimation continueButtonTween;
    [SerializeField, Tooltip("Reference to the username length requirement checkbox tweener")] private DOTweenAnimation usernameLengthRequirementTween;
    [SerializeField, Tooltip("Reference to the password length requirement checkbox tweener")] private DOTweenAnimation passwordLengthRequirementTween;
    [SerializeField, Tooltip("Reference to the password lowercase requirement checkbox tweener")] private DOTweenAnimation passwordLowercaseRequirementTween;
    [SerializeField, Tooltip("Reference to the password uppercase requirement checkbox tweener")] private DOTweenAnimation passwordUppercaseRequirementTween;
    [SerializeField, Tooltip("Reference to the password number requirement checkbox tweener")] private DOTweenAnimation passwordNumberRequirementTween;
    [SerializeField, Tooltip("Reference to the password symbol requirement checkbox tweener")] private DOTweenAnimation passwordSymbolRequirementTween;
    [SerializeField, Tooltip("Reference to the text that displays debugging information")] private TextMeshProUGUI debugText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (debugText != null)
        {
            debugText.text = "";
        }
        if (usernameInput != null)
        {
            if (GlobalVariables.Get<string>("usernameInput") != null)
            {
                usernameInput.text = GlobalVariables.Get<string>("usernameInput");
                OnUsernameTextUpdated(GlobalVariables.Get<string>("usernameInput"));
            }
            else
            {
                usernameInput.text = "";
                OnUsernameTextUpdated("");
            }
        }
        if (passwordInput != null)
        {
            passwordInput.text = "";
            OnPasswordTextUpdated("");
        }
    }

    public void OnBackPressed()
    {
        DisableAllInput();
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: backSceneLoadInfo);
    }

    public void SubscribeUsernameLogin()
    {
        // when enter is pressed, call onUsernameLoginContinue
        //m_SubmitListener = InputSystem.onAnyButtonPress.Call((control) =>
        //{
        //    if (control.name == "Enter")
        //    {
        //        OnUsernameLoginContinue();
        //        m_SubmitListener.Dispose();
        //        m_SubmitListener = null;
        //    }
        //});
    }

    public void SubscribePasswordLogin()
    {
        // when enter is pressed, call onPasswordLoginContinue
        //m_SubmitListener = InputSystem.onAnyButtonPress.Call((control) =>
        //{
        //    if (control.name == "Enter")
        //    {
        //        OnPasswordLoginContinue();
        //        m_SubmitListener.Dispose();
        //        m_SubmitListener = null;
        //    }
        //});
    }

    public void SubscribeUsernameSignup()
    {
        // when enter is pressed, call onUsernameSignupContinue
        //m_SubmitListener = InputSystem.onAnyButtonPress.Call((control) =>
        //{
        //    if (control.name == "Enter")
        //    {
        //        OnUsernameSignupContinue();
        //        m_SubmitListener.Dispose();
        //        m_SubmitListener = null;
        //    }
        //});
    }

    public void SubscribePasswordSignup()
    {
        // when enter is pressed, call onPasswordSignupContinue
        //m_SubmitListener = InputSystem.onAnyButtonPress.Call((control) =>
        //{
        //    if (control.name == "Enter")
        //    {
        //        OnPasswordSignupContinue();
        //        m_SubmitListener.Dispose();
        //        m_SubmitListener = null;
        //    }
        //});
    }

    public void UnsubscribeSubmit()
    {
        //m_SubmitListener?.Dispose();
    }

    public void OnUsernameTextUpdated(string newText)
    {
        if (newText.Length < 3)
        {
            if (continueButton.interactable)
            {
                continueButtonTween.DOPlayBackwards();
                usernameLengthRequirementTween.DOPlayBackwards();
            }
            continueButton.interactable = false;
        }
        else
        {
            if (!continueButton.interactable)
            {
                continueButtonTween.DOPlayForward();
                usernameLengthRequirementTween.DOPlayForward();
            }
            continueButton.interactable = true;
            GlobalVariables.Set("usernameInput", newText);
        }
    }

    public void OnPasswordTextUpdated(string newText)
    {
        if (newText.Length > 8 && Regex.IsMatch(newText, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$"))
        {
            if (!continueButton.interactable)
            {
                continueButtonTween.DOPlayForward();
            }
            continueButton.interactable = true;
        }
        else
        {
            if (continueButton.interactable)
            {
                continueButtonTween.DOPlayBackwards();
            }
            continueButton.interactable = false;
        }

        if (newText.Length >= 8)
        {
            passwordLengthRequirementTween.DOPlayForward();
        }
        else
        {
            passwordLengthRequirementTween.DOPlayBackwards();
        }

        if (Regex.IsMatch(newText, @"[a-z]"))
        {
            passwordLowercaseRequirementTween.DOPlayForward();
        }
        else
        {
            passwordLowercaseRequirementTween.DOPlayBackwards();
        }

        if (Regex.IsMatch(newText, @"[A-Z]"))
        {
            passwordUppercaseRequirementTween.DOPlayForward();
        }
        else
        {
            passwordUppercaseRequirementTween.DOPlayBackwards();
        }

        if (Regex.IsMatch(newText, @"\d"))
        {
            passwordNumberRequirementTween.DOPlayForward();
        }
        else
        {
            passwordNumberRequirementTween.DOPlayBackwards();
        }

        if (Regex.IsMatch(newText, @"[^\da-zA-Z]"))
        {
            passwordSymbolRequirementTween.DOPlayForward();
        }
        else
        {
            passwordSymbolRequirementTween.DOPlayBackwards();
        }
    }

    public async void OnUsernameLoginContinue()
    {
        if (continueButton.interactable)
        {
            debugText.text = "";
            DisableAllInput();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            string cloudCodeRequest = await CloudCodeService.Instance.CallEndpointAsync<string>("check_username_availability", new Dictionary<string, object>() { { "username", usernameInput.text } });
            if (cloudCodeRequest == "1")
            {
                DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
            }
            else
            {
                debugText.text = "Your account does not exist.";
                EnableAllInput();
            }
        }
    }

    public async void OnUsernameSignupContinue()
    {
        if (continueButton.interactable)
        {
            debugText.text = "";
            DisableAllInput();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            string cloudCodeRequest = await CloudCodeService.Instance.CallEndpointAsync<string>("check_username_availability", new Dictionary<string, object>() { { "username", usernameInput.text.ToLowerInvariant() } });
            if (cloudCodeRequest == "0")
            {
                DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
            }
            else
            {
                debugText.text = "This username has already been taken.";
                EnableAllInput();
            }
        }
    }

    public async void OnPasswordLoginContinue()
    {
        if (continueButton.interactable)
        {
            debugText.text = "";
            DisableAllInput();
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut(true);
                AuthenticationService.Instance.ClearSessionToken();
            }
            string result = await KittyAccountManager.Instance.SignInWithUsernamePasswordAsync(GlobalVariables.Get<string>("usernameInput"), passwordInput.text);
            if (AuthenticationService.Instance.IsSignedIn)
            {
                if (DownloadManager.Instance != null)
                {
                    DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
                }
            }
            else
            {
                debugText.text = result;
                EnableAllInput();
            }
        }
    }

    public async void OnPasswordSignupContinue()
    {
        if (continueButton.interactable)
        {
            debugText.text = "";
            DisableAllInput();
            string result = await KittyAccountManager.Instance.AddUsernamePasswordAsync(GlobalVariables.Get<string>("usernameInput"), passwordInput.text);
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    AuthenticationService.Instance.SignOut(true);
                    AuthenticationService.Instance.ClearSessionToken();
                    result = await KittyAccountManager.Instance.SignInWithUsernamePasswordAsync(GlobalVariables.Get<string>("usernameInput"), passwordInput.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    EnableAllInput();
                    debugText.text = result;
                    return;
                }
            }
            if (AuthenticationService.Instance.IsSignedIn)
            {
                try
                {
                    if (GlobalVariables.Get<string>("usernameInput") != null && GlobalVariables.Get<string>("usernameInput").Length >= 3)
                    {
                        await AuthenticationService.Instance.UpdatePlayerNameAsync(GlobalVariables.Get<string>("usernameInput"));
                    }
                    if (GlobalVariables.Get<int>("selectedProfileIndex") != 0 && GlobalVariables.Get<string>("usernameInput") != null && GlobalVariables.Get<string>("usernameInput").Length >= 3)
                    {
                        Dictionary<string, object> data = new() { { "profileIndex", GlobalVariables.Get<int>("selectedProfileIndex") }, { "username", GlobalVariables.Get<string>("usernameInput").ToLowerInvariant() } };
                        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));
                    }
                    else if (GlobalVariables.Get<string>("usernameInput") != null && GlobalVariables.Get<string>("usernameInput").Length >= 3)
                    {
                        Dictionary<string, object> data = new() { { "username", GlobalVariables.Get<string>("usernameInput").ToLowerInvariant() } };
                        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));
                    }
                    else if (GlobalVariables.Get<int>("selectedProfileIndex") != 0)
                    {
                        Dictionary<string, object> data = new() { { "profileIndex", GlobalVariables.Get<int>("selectedProfileIndex") } };
                        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                if (DownloadManager.Instance != null)
                {
                    DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
                }
            }
            else
            {
                debugText.text = result;
                EnableAllInput();
            }
        }
    }

    public async void OnLogin()
    {
        if (usernameInput.text.Length < 3)
        {
            debugText.text = "Username must be at least 3 characters long.";
            return;
        }
        if (passwordInput.text.Length < 8)
        {
            debugText.text = "Password must be at least 8 characters long.";
            return;
        }
        debugText.text = "Logging in...";
        DisableAllInput();
        await KittyAccountManager.Instance.SignInWithUsernamePasswordAsync(usernameInput.text, passwordInput.text);
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
        if (usernameInput.text.Length < 3)
        {
            debugText.text = "Username must be at least 3 characters long.";
            return;
        }
        if (passwordInput.text.Length < 8)
        {
            debugText.text = "Password must be at least 8 characters long.";
            return;
        }
        if (!Regex.IsMatch(passwordInput.text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$"))
        {
            debugText.text = "Password must contain at least 1 lowercase letter, 1 uppercase letter, 1 number, and 1 symbol.";
            return;
        }
        debugText.text = "Signing up...";
        DisableAllInput();
        await KittyAccountManager.Instance.SignUpWithUsernamePasswordAsync(usernameInput.text, passwordInput.text);
        if (AuthenticationService.Instance.IsSignedIn)
        {
            debugText.text = "Signup successful!\nAccount ID: " + AuthenticationService.Instance.PlayerId + "\nName: " + AuthenticationService.Instance.PlayerName + "\nAccess Token: " + AuthenticationService.Instance.AccessToken;
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(usernameInput.text);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            if (DownloadManager.Instance != null)
            {
                //DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: sceneLoadInfo);
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
        if (usernameInput != null)
        {
            usernameInput.interactable = false;
        }
        if (passwordInput != null)
        {
            passwordInput.interactable = false;
        }
        continueButton.interactable = false;
    }

    private void EnableAllInput()
    {
        if (usernameInput != null)
        {
            usernameInput.ActivateInputField();
            usernameInput.interactable = true;
        }
        if (passwordInput != null)
        {
            passwordInput.ActivateInputField();
            passwordInput.interactable = true;
        }
        continueButton.interactable = true;
    }
}
