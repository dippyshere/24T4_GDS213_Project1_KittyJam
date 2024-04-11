using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordRPCManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference to the Discord RPC manager")] public static DiscordRPCManager Instance;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WSA
    [HideInInspector, Tooltip("The Discord RPC instance")] public Discord.Discord discord;
#else
    [HideInInspector, Tooltip("The Discord RPC instance")] public object discord;
#endif

    private void Awake()
    {
        Instance = this;
    }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WSA
    private void Start()
    {
        discord = new Discord.Discord(1226127046586142730, (UInt64)Discord.CreateFlags.NoRequireDiscord);
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            Details = "In the main menu",
            Assets =
            {
                LargeImage = "kittyjam_cover",
                LargeText = "Kitty Jam Main Menu"
            },
            Timestamps =
            {
                Start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            }
        };
        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res != Discord.Result.Ok)
            {
                Debug.LogError("Discord RPC failed to update");
            }
        });
    }
#endif

    public void UpdateActivity(string state = "", string details = "", Int64 start = 0, Int64 end = 0, string largeImageKey = "", string largeImageText = "", string smallImageKey = "", string smallImageText = "", string partyID = "", Int32 currentPartySize = 1, Int32 maxPartySize = 4, string matchSecret = "", string joinSecret = "", string spectateSecret = "", bool instance = false)
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WSA
        if (discord == null)
        {
            return;
        }
        Discord.ActivityManager activityManager = discord.GetActivityManager();
        Discord.Activity activity = new Discord.Activity();
        if (state != "")
        {
            activity.State = state;
        }
        if (details != "")
        {
            activity.Details = details;
        }
        if (start != 0)
        {
            activity.Timestamps.Start = start;
        }
        if (end != 0)
        {
            activity.Timestamps.End = end;
        }
        if (largeImageKey != "")
        {
            activity.Assets.LargeImage = largeImageKey;
        }
        if (largeImageText != "")
        {
            activity.Assets.LargeText = largeImageText;
        }
        if (smallImageKey != "")
        {
            activity.Assets.SmallImage = smallImageKey;
        }
        if (smallImageText != "")
        {
            activity.Assets.SmallText = smallImageText;
        }
        if (partyID != "")
        {
            activity.Party.Id = partyID;
        }
        if (currentPartySize != 1)
        {
            activity.Party.Size.CurrentSize = currentPartySize;
        }
        if (maxPartySize != 4)
        {
            activity.Party.Size.MaxSize = maxPartySize;
        }
        if (matchSecret != "")
        {
            activity.Secrets.Match = matchSecret;
        }
        if (joinSecret != "")
        {
            activity.Secrets.Join = joinSecret;
        }
        if (spectateSecret != "")
        {
            activity.Secrets.Spectate = spectateSecret;
        }
        activity.Instance = instance;
        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res != Discord.Result.Ok)
            {
                Debug.LogError("Discord RPC failed to update");
            }
        });
#endif
    }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WSA
    private void OnDestroy()
    {
        discord?.Dispose();
    }

    private void Update()
    {
        try
        {
            discord.RunCallbacks();
        }
        catch (Exception)
        {
            discord?.Dispose();
        }
    }
#endif
}
