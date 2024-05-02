using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class ProfileIconButtonManager : MonoBehaviour
{
    [SerializeField, Tooltip("The image that displays the profile icon")] private Image profileIconImage;
    [SerializeField, Tooltip("The tween component for the checkbox")] private DOTweenAnimation checkboxTween;
    [HideInInspector, Tooltip("The index of the profile icon")] public int profileIconIndex;

    public void SetProfileIcon(int profileIconIndex)
    {
        this.profileIconIndex = profileIconIndex;
        string profileIconPath = "Assets/Textures/ProfilePictures/" + profileIconIndex + ".png";
        try
        {
            Addressables.LoadAssetAsync<Sprite>(profileIconPath).Completed += (op) =>
            {
                if (profileIconImage != null)
                {
                    profileIconImage.sprite = op.Result;
                }
            };
        }
        catch (System.Exception)
        {

        }
    }

    public void OnIconClick()
    {
        ProfileIconSelectionManager.instance.SetActiveProfileIcon(profileIconIndex);
    }

    public void EnableCheckbox()
    {
        checkboxTween.DORestart();
    }

    public void DisableCheckbox()
    {
        checkboxTween.DOPlayBackwards();
    }
}
