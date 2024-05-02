using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;

public class ProfileIconSelectionManager : MonoBehaviour
{
    [HideInInspector, Tooltip("Instance of the ProfileIconSelectionManager")] public static ProfileIconSelectionManager instance;
    [Header("References")]
    [SerializeField, Tooltip("Scene load info to use once the profile icon is selected")] private SceneLoadInfo signupSceneLoadInfo;
    [SerializeField, Tooltip("Scene load info to use once when skipping onboarding to log back in")] private SceneLoadInfo loginSceneLoadInfo;
    [SerializeField, Tooltip("Reference to the gameobject that contains all the profile icons")] private GameObject profileIconsParent;
    [SerializeField, Tooltip("Prefab to use for profile icon buttons")] private GameObject profileIconButtonPrefab;
    [SerializeField, Tooltip("reference to the continue button tweener")] private DOTweenAnimation continueButtonTween;
    [Tooltip("The currently selected profile icon")] private int selectedProfileIcon = 0;
    [Tooltip("If the profile icon has been set")] private bool isSet = false;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Addressables.LoadResourceLocationsAsync("profilepictures", typeof(Sprite)).Completed += (op) =>
        {
            foreach (Transform child in profileIconsParent.transform)
            {
                Destroy(child.gameObject);
            }
            int profileIconCount = op.Result.Count;
            for (int i = 1; i <= profileIconCount && i <= 10; i++)
            {
                GameObject profileIconButton = Instantiate(profileIconButtonPrefab, profileIconsParent.transform);
                profileIconButton.GetComponent<ProfileIconButtonManager>().SetProfileIcon(i);
            }
            BeatAnimation.instance.RefreshBeatObjects();
            if (GlobalVariables.Get<int>("selectedProfileIndex") != 0)
            {
                SetActiveProfileIcon(GlobalVariables.Get<int>("selectedProfileIndex"));
            }
        };
    }

    public void SetActiveProfileIcon(int profileIconIndex)
    {
        if (selectedProfileIcon == 0)
        {
            continueButtonTween.DORestart();
        }
        selectedProfileIcon = profileIconIndex;
        GlobalVariables.Set("selectedProfileIndex", profileIconIndex);
        foreach (Transform child in profileIconsParent.transform)
        {
            if (child.TryGetComponent(out ProfileIconButtonManager profileIconButtonManager))
            {
                if (profileIconButtonManager.profileIconIndex == profileIconIndex)
                {
                    profileIconButtonManager.EnableCheckbox();
                }
                else
                {
                    profileIconButtonManager.DisableCheckbox();
                }
            }
        }
    }

    public void ContinueButton()
    {
        if (isSet) return;
        isSet = true;
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: signupSceneLoadInfo);
    }

    public void SkipOnboarding()
    {
        if (isSet) return;
        isSet = true;
        DownloadManager.Instance.BeginDownloadAssetsCoroutine(sceneLoadInfo: loginSceneLoadInfo);
    }
}
