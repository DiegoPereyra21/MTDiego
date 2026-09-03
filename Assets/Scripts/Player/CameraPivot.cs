using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private bool lockCursor = true;

    private InputAction lookAction;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        lookAction = inputActions.FindActionMap("Player").FindAction("Look");

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void OnEnable()
    {
        lookAction.Enable();
        if (lockCursor) Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        lookAction.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    private void LateUpdate()
    {
        Vector2 look = lookAction.ReadValue<Vector2>() * sensitivity;

        yaw += look.x;
        pitch = Mathf.Clamp(pitch - look.y, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}