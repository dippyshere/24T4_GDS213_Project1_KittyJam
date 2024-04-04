using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTextureController : MonoBehaviour
{
    [HideInInspector, Tooltip("Singleton reference for the cursor controller.")] public static CursorTextureController instance;
    [Header("Cursor Settings")]
    [SerializeField, Tooltip("The cursor texture to use for the default cursor.")] private Texture2D defaultCursorTexture;
    [SerializeField, Tooltip("The hot spot of the cursor texture.")] private Vector2 cursorHotspot = Vector2.zero;
    [SerializeField, Tooltip("The cursor texture to use for the hover cursor.")] private Texture2D hoverCursorTexture;
    [SerializeField, Tooltip("The hot spot of the hover cursor texture.")] private Vector2 hoverCursorHotspot = Vector2.zero;
    [SerializeField, Tooltip("The cursor texture to use for the click cursor.")] private Texture2D clickCursorTexture;
    [SerializeField, Tooltip("The hot spot of the click cursor texture.")] private Vector2 clickCursorHotspot = Vector2.zero;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Cursor.SetCursor(defaultCursorTexture, cursorHotspot, CursorMode.Auto);
    }

    public void SetCursorHover()
    {
        Cursor.SetCursor(hoverCursorTexture, hoverCursorHotspot, CursorMode.Auto);
    }

    public void SetCursorDefault()
    {
        Cursor.SetCursor(defaultCursorTexture, cursorHotspot, CursorMode.Auto);
    }

    public void SetCursorClick()
    {
        Cursor.SetCursor(clickCursorTexture, clickCursorHotspot, CursorMode.Auto);
    }
}
