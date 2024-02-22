using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.position.z <= -10)
        {
            transform.position += new Vector3(0, 0, 20);
        }
        else
        {
            transform.position -= new Vector3(0, 0, 16 / HighwaySongManager.Instance.noteTime * Time.deltaTime);
        }
    }
}
