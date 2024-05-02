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
    [SerializeField, Tooltip("Reference to the continue button tweener")] private DOTweenAnimation continueButtonTween;
    [SerializeField, Tooltip("Reference to the left button for scrolling")] private GameObject leftButton;
    [SerializeField, Tooltip("Reference to the right button for scrolling")] private GameObject rightButton;
    [Tooltip("The currently selected profile icon")] private int selectedProfileIcon = 0;
    [Tooltip("If the profile icon has been set")] private bool isSet = false;
    [Tooltip("Cached number of profile icons")] private int profileIconCount = 0;
    [Tooltip("Current page index")] private int currentPageIndex = 0;

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
            profileIconCount = op.Result.Count;
            UpdatePage();
        };
    }

    public void UpdatePage()
    {
        foreach (Transform child in profileIconsParent.transform)
        {
            Destroy(child.gameObject);
        }
        int startIndex = currentPageIndex * 10;
        for (int i = 1 + startIndex; i <= profileIconCount && i <= 10 + startIndex; i++)
        {
            GameObject profileIconButton = Instantiate(profileIconButtonPrefab, profileIconsParent.transform);
            profileIconButton.GetComponent<ProfileIconButtonManager>().SetProfileIcon(i);
        }
        BeatAnimation.instance.RefreshBeatObjects();
        if (GlobalVariables.Get<int>("selectedProfileIndex") != 0)
        {
            SetActiveProfileIcon(GlobalVariables.Get<int>("selectedProfileIndex"));
        }
        UpdateButtons();
    }

    public void ScrollLeft()
    {
        if (profileIconCount <= 10) return;
        if (profileIconsParent.transform.childCount == 0) return;
        if (profileIconsParent.transform.GetChild(0).GetComponent<ProfileIconButtonManager>().profileIconIndex <= 1) return;
        currentPageIndex--;
        UpdatePage();
    }

    public void ScrollRight()
    {
        if (profileIconCount <= 10) return;
        if (profileIconsParent.transform.childCount == 0) return;
        if (profileIconsParent.transform.GetChild(profileIconsParent.transform.childCount - 1).GetComponent<ProfileIconButtonManager>().profileIconIndex >= profileIconCount) return;
        currentPageIndex++;
        UpdatePage();
    }

    public void UpdateButtons()
    {
        if (profileIconCount <= 10) return;
        if (profileIconsParent.transform.childCount == 0) return;
        if (currentPageIndex == 0)
        {
            leftButton.GetComponent<UnityEngine.EventSystems.EventTrigger>().OnPointerExit(null);
            leftButton.SetActive(false);
        }
        else
        {
            leftButton.SetActive(true);
        }
        if (currentPageIndex >= Mathf.CeilToInt((float)profileIconCount / 10) - 1)
        {
            rightButton.GetComponent<UnityEngine.EventSystems.EventTrigger>().OnPointerExit(null);
            rightButton.SetActive(false);
        }
        else
        {
            rightButton.SetActive(true);
        }
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
