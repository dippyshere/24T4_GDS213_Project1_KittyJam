using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseMenu : MonoBehaviour
{
    public CursorController cursorController;

    public Volume postProcessVolume;
    public VolumeProfile defaultPostProcess;
    public VolumeProfile upgradePostProcess;

    //public GameManager gameManager;
    public ArmController armController;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = FindObjectOfType<GameManager>();
        cursorController.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        //gameManager.isPaused = true;
        armController.canSlam = false;
    }

    private void OnEnable()
    {
        cursorController.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        //gameManager.isPaused = true;
        armController.canSlam = false;
    }

    public void CloseUpgradeMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        cursorController.LockCursor();
        Time.timeScale = 1f;
        postProcessVolume.profile = defaultPostProcess;
        //gameManager.isPaused = false;
        armController.canSlam = true;
    }
}
