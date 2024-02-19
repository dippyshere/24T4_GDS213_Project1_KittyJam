using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayMovement : MonoBehaviour
{
    [SerializeField, Tooltip("The speed of the highway")] public float speedMultiplier = 1;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < -10)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 10);
        }
        else
        {
            transform.position -= new Vector3(0, 0, (HighwaySongManager.Instance.noteTime * 4) * speedMultiplier * Time.deltaTime);
        }
    }
}
