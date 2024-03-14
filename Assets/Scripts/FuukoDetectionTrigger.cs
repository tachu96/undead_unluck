using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuukoDetectionTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController playerController;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fuuko"))
        {
            playerController.FuukoEnteredRange();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fuuko"))
        {
            playerController.FuukoExitedRange();
        }
    }
}
