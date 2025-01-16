using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] public float playerSpeed = 20f; // Speed of the player movement
    [SerializeField] private float gravity = -10; // Gravity force applied to the player
    
    [Header("Mouse Settings")]
    [SerializeField] public float mouseSensitivity = 1.5f; // Sensitivity of the mouse input
    [SerializeField] public float mouseSmoothing = 1.5f; // Smoothing factor for mouse input
    
    [Header("References")]
    [SerializeField] public Animator cameraAnimator; // Reference to the camera animator for head bob effect
    private CharacterController _characterController;
    private Vector3 _movementVector;
    private float _verticalVelocity;
    private Vector2 _smoothedMouseDelta;
    private Vector2 _mouseDelta;
    private Vector2 _lookAngles;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    private void Start()
    {
        // Initialize the character controller and lock the cursor
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Handle player input and actions each frame
    /// </summary>
    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        CheckForHeadBob();
    }

    /// <summary>
    /// Handles player movement based on keyboard input and gravity
    /// </summary>
    private void HandleMovement()
    {
        // Get input for horizontal and vertical movement
        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        inputVector = transform.TransformDirection(inputVector).normalized;

        // Apply gravity
        if (_characterController.isGrounded)
        {
            _verticalVelocity = 0f;
            if (Input.GetButton("Jump"))
            {
                _verticalVelocity = Mathf.Sqrt(-0.1f * gravity * playerSpeed);
            }
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine the movement vector with gravity
        _movementVector = (inputVector * playerSpeed) + Vector3.up * _verticalVelocity;
        _characterController.Move(_movementVector * Time.deltaTime);
    }

    /// <summary>
    /// Handles mouse input for rotating the player and the camera
    /// </summary>
    private void HandleMouseLook()
    {
        // Get raw mouse input
        _mouseDelta.x = Input.GetAxis("Mouse X") * mouseSensitivity;
        _mouseDelta.y = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Apply smoothing to mouse input
        _smoothedMouseDelta = Vector2.Lerp(_smoothedMouseDelta, _mouseDelta, 1f / mouseSmoothing);

        // Update camera rotation angles
        _lookAngles.x += _smoothedMouseDelta.x;
        _lookAngles.y = Mathf.Clamp(_lookAngles.y - _smoothedMouseDelta.y, -90f, 90f);

        // Apply rotation to player and camera
        transform.localRotation = Quaternion.Euler(0f, _lookAngles.x, 0f);
        Camera.main.transform.localRotation = Quaternion.Euler(_lookAngles.y, 0f, 0f);
    }

    /// <summary>
    /// Checks if the player is moving and updates the head bob animation
    /// </summary>
    private void CheckForHeadBob()
    {
        bool isWalking = _characterController.velocity.magnitude > 0.1f;
        cameraAnimator.SetBool(IsWalking, isWalking);
    }
}