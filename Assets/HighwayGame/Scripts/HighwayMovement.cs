using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of the highway
/// </summary>
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
            if (SongManager.Instance == null || SongManager.Instance.noteTime == 0)
            {
                transform.position -= new Vector3(0, 0, 16 * Time.deltaTime);
            }
            else
            {
                transform.position -= new Vector3(0, 0, 16 / SongManager.Instance.noteTime * Time.deltaTime);
            }
        }
    }
}
