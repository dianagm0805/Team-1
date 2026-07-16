using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class VRJump : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference jumpActionReference;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (jumpActionReference != null)
        {
            jumpActionReference.action.performed += OnJump;
        }
    }

    private void OnDisable()
    {
        if (jumpActionReference != null)
        {
            jumpActionReference.action.performed -= OnJump;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Only jump if the Character Controller is touching the ground
        if (characterController.isGrounded)
        {
            // Physics formula to calculate required velocity for a specific height
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;

        // Reset downward velocity when grounded so gravity doesn't infinitely accumulate
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small constant force to keep the player pinned to the floor
        }

        // Apply constant gravity over time
        playerVelocity.y += gravity * Time.deltaTime;

        // Move the Character Controller vertically
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
