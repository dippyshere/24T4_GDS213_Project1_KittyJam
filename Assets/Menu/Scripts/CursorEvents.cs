using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorEvents : EventTrigger
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        CursorTextureController.instance.SetCursorHover();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        CursorTextureController.instance.SetCursorDefault();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        CursorTextureController.instance.SetCursorClick();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        CursorTextureController.instance.SetCursorHover();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        CursorTextureController.instance.SetCursorDefault();
    }
}
