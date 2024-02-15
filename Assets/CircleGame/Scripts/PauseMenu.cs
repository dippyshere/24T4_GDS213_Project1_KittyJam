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

    public ArmController armController;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager = FindObjectOfType<GameManager>();
        cursorController.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        armController.canSlam = false;
    }

    private void OnEnable()
    {
        cursorController.UnlockCursor();
        Time.timeScale = 0f;
        postProcessVolume.profile = upgradePostProcess;
        armController.canSlam = false;
    }

    private void OnDisable()
    {
        cursorController.LockCursor();
        Time.timeScale = 1f;
        postProcessVolume.profile = defaultPostProcess;
        armController.canSlam = true;
    }

    public void PauseAction(bool pause)
    {
        if (pause)
        {
            cursorController.UnlockCursor();
            Time.timeScale = 0f;
            postProcessVolume.profile = upgradePostProcess;
            armController.canSlam = false;
        }
        else
        {
            cursorController.LockCursor();
            Time.timeScale = 1f;
            postProcessVolume.profile = defaultPostProcess;
            armController.canSlam = true;
        }
    }
}
