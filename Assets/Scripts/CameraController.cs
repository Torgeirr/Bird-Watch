using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraPivot; // Transform of the pivot object
    public Camera mainCamera;
    public float pivotSpeed = 5f;
    public float maxVerticalAngle = 80f; // Hard limit on how far the player can look vertically

    private Vector3 targetEulerAngles;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }

        // Get normalized cursor position (center of screen is (0, 0))
        Vector2 normalizedCursorPos = new Vector2(
            (Input.mousePosition.x / Screen.width) - 0.5f,
            (Input.mousePosition.y / Screen.height) - 0.5f
        );

        // Calculate target angles based on cursor position
        targetEulerAngles.x = Mathf.Clamp(-normalizedCursorPos.y * 90f, -maxVerticalAngle, maxVerticalAngle);
        targetEulerAngles.y = normalizedCursorPos.x * 180f;

        // Smoothly rotate the pivot using Slerp
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles.x, targetEulerAngles.y, 0);
        cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, Time.deltaTime * pivotSpeed);
        //mainCamera.fieldOfView = 60f;
    }

    private void QuitApplication()
    {
        Debug.Log("Until next time...");
        Application.Quit();
    }
}
