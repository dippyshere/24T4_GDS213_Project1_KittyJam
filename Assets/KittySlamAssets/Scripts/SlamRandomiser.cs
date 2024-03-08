using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Randomises the rotation of the slam object
/// </summary>
public class SlamRandomiser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
