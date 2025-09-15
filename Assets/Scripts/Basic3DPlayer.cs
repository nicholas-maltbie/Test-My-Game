using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
class Basic3DPlayer : MonoBehaviour
{
    private const float sensitivity = .2f;

    private const float minimumLookAngle = -60; // min look angle in degrees
    private const float maximumLookAngle = 60; // max look angle in degrees

    [SerializeField]
    private Transform cameraTarget;

    [SerializeField]
    private float jumpVelocity = 5;

    [SerializeField]
    private float moveSpeed = 3;

    /// <summary>
    /// Maximum acceleration in units per second squared.
    /// </summary>
    [SerializeField]
    private float maxAcceleration = 20.0f;

    private Camera playerCamera;
    private CharacterController characterController;
    private InputActionMap playerControls;

    private Vector3 worldVelocity;
    private Vector3 moveVelocity;
    private Vector2 inputDir;

    /// <summary>
    /// The difference in rotation since the last FixedUpdate.
    /// Modified during Update, and consumed during FixedUpdate.
    /// </summary>
    private float rotateDiff = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();

        playerControls = InputSystem.actions.FindActionMap("Player", true);
        playerControls["Jump"].performed += OnJump_Callback;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerControls["Jump"].performed -= OnJump_Callback;
    }

    private void DoJump()
    {
        worldVelocity = jumpVelocity * Vector3.up;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var horse = hit.gameObject.GetComponent<SecondHorse>();
        if (horse != null)
        {
            horse.OnPlayerHit(gameObject, hit.point);
        }
    }

    private void OnJump_Callback(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            DoJump();
        }
    }

    private void Update()
    {
        if (!playerCamera)
        {
            playerCamera = Camera.main;
            return;
        }

        if (!playerCamera)
        {
            playerCamera = Camera.main;
            return;
        }

        inputDir = playerControls["Move"].ReadValue<Vector2>();
        var look = playerControls["Look"];
        if (look.IsPressed())
        {
            OnLook_Callback(look.ReadValue<Vector2>());
        }
    }

    private void LateUpdate()
    {
        if (!playerCamera)
        {
            return;
        }

        playerCamera.transform.SetPositionAndRotation(cameraTarget.position, cameraTarget.rotation);
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, rotateDiff * sensitivity, 0);
        rotateDiff = 0;

        OnMove_Update(inputDir);

        if (!characterController.isGrounded)
        {
            worldVelocity += Physics.gravity * Time.fixedDeltaTime;
        }
        else if (Vector3.Dot(worldVelocity, Vector3.up) <= 0)
        {
            worldVelocity = Vector3.zero;
        }

        characterController.Move((moveVelocity + worldVelocity) * Time.fixedDeltaTime);
    }

    private void OnLook_Callback(Vector2 delta)
    {
        rotateDiff += delta.x;
        cameraTarget.Rotate(-delta.y * sensitivity, 0, 0);

        // Clamp player look angles.
        // Special processing needed for rotation on euler X due to 
        // its rotation value always being between 0 and 360 after transform.rotate()
        float rx = cameraTarget.localEulerAngles.x;
        if (rx >= 180f)
        {
            rx -= 360f;
        }

        var pitchRotation = Mathf.Clamp(rx, minimumLookAngle, maximumLookAngle) + 360f;
        cameraTarget.localEulerAngles = new Vector3(pitchRotation, 0, 0);
    }

    private void OnMove_Update(Vector2 inputDir)
    {
        // Compute clamped movement direction of magnitude between zero and 1
        Vector3 clampedMovementDir = Vector3.ClampMagnitude(transform.forward * inputDir.y + transform.right * inputDir.x, 1);
        var targetHorizontalVelocity = clampedMovementDir * moveSpeed;

        // Compute delta between target and current horizontal velocity.
        var deltaHorizontalVelocity = targetHorizontalVelocity - moveVelocity;

        // Compute the bounded delta by max acceleration
        var acceleration = Vector3.ClampMagnitude(deltaHorizontalVelocity, maxAcceleration * Time.fixedDeltaTime);

        // Apply some force based on acceleration
        moveVelocity += acceleration;
    }
}
