using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the collision of the paw with items
/// </summary>
public class PawCollision : MonoBehaviour
{

    [SerializeField, Tooltip("Reference to the arm controller")] private ArmController armController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            armController.PickUpItem(other.gameObject);
        }
    }
}
