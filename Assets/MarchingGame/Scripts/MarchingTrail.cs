using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MarchingTrail : MonoBehaviour
{
    public TrailRenderer trailRenderer;

    Vector3 worldPosition;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        trailRenderer.transform.position = worldPosition;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);


        if (Input.GetMouseButtonDown(0))
        {
            trailRenderer.enabled = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            trailRenderer.enabled = false;
            trailRenderer.Clear();
        }

    }
}
