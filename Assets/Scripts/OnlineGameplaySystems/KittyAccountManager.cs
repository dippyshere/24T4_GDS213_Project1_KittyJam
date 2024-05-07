using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Core.Environments;

/// <summary>
/// Handles the account management for Kitty Jam.
/// </summary>
public class KittyAccountManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton instance to the account manager for Kitty Jam")] public static KittyAccountManager Instance;

    async void Awake()
    {
        Instance = this;
#if UNITY_IOS || UNITY_ANDROID
        Vibration.Init();
#endif
        try
        {
            InitializationOptions options = new();
#if UNITY_EDITOR
            options.SetEnvironmentName("dev");
#else
            options.SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Creates a new account with an anonymous login.
    /// </summary>
    /// <returns>The task of the sign-in process.</returns>
    public async UniTask<string> SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Success);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibratePeek();
#endif
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            return null;
        }
        catch (AuthenticationException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
    }

    /// <summary>
    /// Creates a new account with a username and password.
    /// </summary>
    /// <param name="username">The username to use for the account.</param>
    /// <param name="password">The password to use for the account.</param>
    /// <returns>The task of the sign-up process.</returns>
    public async UniTask<string> SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Success);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibratePeek();
#endif
            Debug.Log("SignUp is successful.");
            return null;
        }
        catch (AuthenticationException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
    }

    /// <summary>
    /// Signs in with a username and password.
    /// </summary>
    /// <param name="username">The username to use for the account.</param>
    /// <param name="password">The password to use for the account.</param>
    /// <returns>The task of the sign-in process.</returns>
    public async UniTask<string> SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Success);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibratePeek();
#endif
            Debug.Log("SignIn is successful.");
            return null;
        }
        catch (AuthenticationException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
    }

    /// <summary>
    /// Upgrades the anonymous account to a username and password account.
    /// </summary>
    /// <param name="username">The username to use for the account.</param>
    /// <param name="password">The password to use for the account.</param>
    /// <returns>The task of the upgrade process.</returns>
    public async UniTask<string> AddUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Success);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibratePeek();
#endif
            Debug.Log("Username and password added.");
            return null;
        }
        catch (AuthenticationException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
    }

    /// <summary>
    /// Updates the password of the current account.
    /// </summary>
    /// <param name="currentPassword">The current password of the account.</param>
    /// <param name="newPassword">The new password to use for the account.</param>
    /// <returns>The task of the password update process.</returns>
    public async UniTask<string> UpdatePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Success);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibratePeek();
#endif
            Debug.Log("Password updated.");
            return null;
        }
        catch (AuthenticationException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
#if UNITY_IOS
            try
            {
                Vibration.VibrateIOS(NotificationFeedbackStyle.Error);
            }
            catch (Exception)
            {

            }
#elif UNITY_ANDROID
            Vibration.VibrateNope();
#endif
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogError(ex);
            return ex.Message;
        }
    }
}
