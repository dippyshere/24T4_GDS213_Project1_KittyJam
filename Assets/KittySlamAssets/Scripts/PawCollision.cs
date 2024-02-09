using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawCollision : MonoBehaviour
{

    public ArmController armController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            armController.PickUpItem(other.gameObject);
        }
    }
}
