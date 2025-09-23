using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float jumpHeight = 2f;
    // public float gravity = -9.81f;
    public float gravity = 0f;

    [Header("Look")]
    public float lookSensitivity = 2f;
    public Camera playerCamera;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    private CharacterController controller;
    private PlayerInput input;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private Vector3 velocity;
    private float pitch = 0f;
    private bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();

        var actions = input.actions;
        moveAction = actions["Move"];
        lookAction = actions["Look"];
        jumpAction = actions["Jump"];

        if (!playerCamera)
            playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // --- Ground Check ---
        Vector3 spherePos = transform.position + Vector3.down * (controller.height / 2f - controller.radius + 0.05f);
        isGrounded = Physics.CheckSphere(spherePos, groundCheckDistance, groundMask);

        // if (isGrounded && velocity.y < 0f)
        //     velocity.y = -2f;

        // --- Movement ---
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        // --- Jump ---
        if (isGrounded && jumpAction.WasPerformedThisFrame())
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Look ---
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime * 60f;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime * 60f;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (!controller) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 spherePos = transform.position + Vector3.down * (controller.height / 2f - controller.radius + 0.05f);
        Gizmos.DrawWireSphere(spherePos, groundCheckDistance);
    }
}
