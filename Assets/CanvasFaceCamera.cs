using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Find the main camera in the scene
        mainCameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (mainCameraTransform != null)
        {
            Vector3 cameraForward = mainCameraTransform.forward;
            // Zero out the vertical component, optional
            //cameraForward.y = 0f;

            // Rotate the object to face the camera horizontally
            transform.rotation = Quaternion.LookRotation(cameraForward);
        }
        else
        {
            Debug.LogWarning("Main camera not found in the scene!");
        }
    }
}
