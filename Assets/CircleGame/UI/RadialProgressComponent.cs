using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MyUILibrary;

public class RadialProgressComponent : MonoBehaviour
{

    RadialProgress m_RadialProgress;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_RadialProgress = root.hierarchy[root.hierarchy.childCount - 1] as RadialProgress;
    }

    void Update()
    {
        // For demo purpose, give the progress property dynamic values.
        m_RadialProgress.progress = ((Mathf.Sin(Time.time) + 1.0f) / 2.0f) * 60.0f + 10.0f;
    }
}