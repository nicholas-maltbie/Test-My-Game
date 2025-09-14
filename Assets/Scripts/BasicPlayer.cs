using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BasicPlayer : MonoBehaviour
{
    // Grace time in seconds between when the player jumps and when they start sticking to floor.
    private const float JumpGracePeriod = 0.1f;

    // Time in which jump will be "buffered" and remain queued, set to 5 frames at 60 fps = 0.0833..s
    private const float JumpBufferTime = 5 * 1 / 60.0f;

    // Target height player wants to float off ground at.
    private const float TargetGroundHeight = 0.025f;

    // How strong the spring force holding to reach target height.
    private const float SpringStiffness = 50f;

    // Damping force to reduce oscillations (c) to reach target height.
    private const float Damping = 10f;

    [SerializeField]
    public bool useGravity = true;

    [SerializeField]
    public bool jumpEnabled = true;

    [SerializeField]
    private InputActionReference moveActionReference;

    [SerializeField]
    private InputActionReference jumpActionReference;

    [SerializeField]
    private float groundedDistance = 0.05f;

    [SerializeField]
    private float maxWalkAngle = 60f;

    [SerializeField]
    private float moveSpeed = 3;

    [SerializeField]
    private float jumpHeight = 3;

    [SerializeField]
    private float moveAcceleration = 8;

    [SerializeField]
    private LayerMask excludeCollision;

    // Player movement information
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;

    // Jump information
    private bool jumpQueued;
    private float timeSinceLastJump = Mathf.Infinity;
    private float jumpQueueTime = 0.0f;

    // Player shape
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;

    // Player grounded information
    private RaycastHit2D groundHit;
    private float GroundAngle => Vector2.Angle(groundHit.normal, Up);
    private float GroundDistance => groundHit.distance;
    private bool OnGround => groundHit != default;
    private bool Sliding => OnGround && GroundAngle > maxWalkAngle;


    // Physics information
    private Vector2 gravity;

    private Vector2 InitialJumpVelocity => Mathf.Sqrt(2 * this.gravity.magnitude * this.jumpHeight) * this.Up;
    private Vector2 Position2D => new Vector2(this.transform.position.x, this.transform.position.y);
    private Vector2 Down => this.gravity.normalized;
    private Vector2 Up => -this.gravity.normalized;
    private Vector2 Right => this.transform.right;

    public void Start()
    {
        this.moveAction = this.moveActionReference.action;
        this.jumpAction = this.jumpActionReference.action;
        this.boxCollider = this.GetComponent<BoxCollider2D>();
        this.rb2d = this.GetComponent<Rigidbody2D>();
        this.gravity = Physics2D.gravity;

        this.jumpAction.performed += this.OnJump;
    }

    public void OnDestroy()
    {
        this.jumpAction.performed -= this.OnJump;
    }

    public void Update()
    {
        this.moveInput = this.moveAction.ReadValue<Vector2>();
        if (jumpQueued)
        {
            jumpQueueTime += Time.deltaTime;
            if (jumpQueueTime >= JumpBufferTime)
            {
                jumpQueued = false;
            }
        }
        else
        {
            jumpQueueTime = 0.0f;
        }
    }

    public void FixedUpdate()
    {
        // Update grounded state
        CheckGrounded();

        // Adjust due to gravity influence.
        if ((!OnGround || Sliding || timeSinceLastJump > JumpGracePeriod) && useGravity)
        {
            // If not on ground or sliding, apply gravity
            this.rb2d.linearVelocity += gravity * Time.fixedDeltaTime;
        }
        else if (OnGround)
        {
            // Otherwise, apply a _strong_ damping force to the vertical component of the linear velocity.
            var verticalComponent = Vector2.Dot(this.rb2d.linearVelocity, Up) * Up;

            // Compute target vertical velocity to reach target grounded height
            var displacement = (TargetGroundHeight - GroundDistance) * Up;

            // Calculate spring force
            var springForce = SpringStiffness * displacement;

            // Calculate damping force proportional to velocity: F_damping = -c * v
            var dampingForce = -Damping * verticalComponent;

            // Total force to apply
            var force = springForce + dampingForce;
            this.rb2d.linearVelocity += force * Time.fixedDeltaTime;
        }

        // Allow the player to influence the speed in the horizontal component
        var targetVelocity = moveInput.x * Right * moveSpeed;
        var currentVelocity = Vector2.Dot(this.rb2d.linearVelocity, Right) * Right;

        var push = Vector2.ClampMagnitude(targetVelocity - currentVelocity, moveAcceleration * Time.fixedDeltaTime);
        this.rb2d.linearVelocity += push;

        // Apply jump if able
        if (OnGround && jumpQueued)
        {
            this.rb2d.linearVelocity += this.InitialJumpVelocity;
            this.jumpQueued = false;
            this.timeSinceLastJump = 0;
        }
        else
        {
            this.timeSinceLastJump += Time.fixedDeltaTime;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (jumpEnabled)
        {
            this.jumpQueued = true;
        }
    }

    private void CheckGrounded()
    {
        var boxCenter = this.Position2D + this.boxCollider.offset;
        this.groundHit = default;
        var hits = Physics2D.BoxCastAll(boxCenter, this.boxCollider.size, transform.eulerAngles.z, Down, groundedDistance, layerMask: ~excludeCollision);

        // Filter for first hit that is not a trigger or self
        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == gameObject)
            {
                continue;
            }

            this.groundHit = hit;
            return;
        }
    }
}
