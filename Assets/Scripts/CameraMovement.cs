using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 0.15f;
    [SerializeField] float minAngle = -80f;
    [SerializeField] float maxAngle = 80f;

    float mouseX;
    float mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if(Mouse.current == null) {return;}

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        mouseX += mouseDelta.x * mouseSensitivity;
        mouseY -= mouseDelta.y * mouseSensitivity;

        mouseY = Mathf.Clamp(mouseY, minAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(mouseY,mouseX,0f);
    }
}
