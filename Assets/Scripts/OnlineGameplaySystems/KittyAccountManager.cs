using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        try
        {
#if UNITY_EDITOR
            InitializationOptions options = new();
            options.SetEnvironmentName("dev");
#else
            InitializationOptions options = new();
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
    public async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Creates a new account with a username and password.
    /// </summary>
    /// <param name="username">The username to use for the account.</param>
    /// <param name="password">The password to use for the account.</param>
    /// <returns>The task of the sign-up process.</returns>
    public async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Signs in with a username and password.
    /// </summary>
    /// <param name="username">The username to use for the account.</param>
    /// <param name="password">The password to use for the account.</param>
    /// <returns>The task of the sign-in process.</returns>
    public async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Upgrades the anonymous account to a username and password account.
    /// </summary>
    /// <param name="username">The username to use for the account.</param>
    /// <param name="password">The password to use for the account.</param>
    /// <returns>The task of the upgrade process.</returns>
    public async Task AddUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
            Debug.Log("Username and password added.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Updates the password of the current account.
    /// </summary>
    /// <param name="currentPassword">The current password of the account.</param>
    /// <param name="newPassword">The new password to use for the account.</param>
    /// <returns>The task of the password update process.</returns>
    public async Task UpdatePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
            Debug.Log("Password updated.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
