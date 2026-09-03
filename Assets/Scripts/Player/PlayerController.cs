using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Header("Gravedad")]
    [SerializeField] private float gravity = -15f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraPivot;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private CharacterController controller;
    private Animator animator;
    private InputAction moveAction;
    private InputAction sprintAction;

    private float currentSpeed;
    private float verticalVelocity;
    private float rotationVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        InputActionMap player = inputActions.FindActionMap("Player");
        moveAction = player.FindAction("Move");
        sprintAction = player.FindAction("Sprint");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        bool running = sprintAction.IsPressed();

        // velocidad objetivo, suavizada para que arranque y frene con peso
        float targetSpeed = input == Vector2.zero ? 0f : (running ? runSpeed : walkSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // direccion relativa a la camara + rotacion suavizada del personaje
        Vector3 moveDirection = Vector3.zero;
        if (input != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraPivot.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        ApplyGravity();

        controller.Move((moveDirection * currentSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

        animator.SetFloat(SpeedHash, currentSpeed, 0.1f, Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;          // pega el personaje al piso en rampas y escalones
        else
            verticalVelocity += gravity * Time.deltaTime;
    }
}