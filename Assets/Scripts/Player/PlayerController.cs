using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Movement")]
    [Tooltip("Horizontal move speed")]
    public float moveSpeed = 8f;
    [Tooltip("Ground acceleration rate")]
    public float acceleration = 60f;
    [Tooltip("Ground deceleration rate")]
    public float deceleration = 60f;
    [Tooltip("Air acceleration rate")]
    public float airAcceleration = 30f;
    [Tooltip("Air deceleration rate")]
    public float airDeceleration = 20f;
    [Range(0f, 1f), Tooltip("Air control multiplier")]
    public float airControl = 0.8f;

    [Header("Jumping")]
    [Tooltip("Max jump height in units")]
    public float jumpHeight = 1.5f;
    [Tooltip("Time to reach apex")]
    public float jumpApexTime = 0.4f;
    [Tooltip("Multiplier for cutting jump short")]
    public float jumpCutMultiplier = 0.5f;
    [Tooltip("Fall speed multiplier")]
    public float fallMultiplier = 1.5f;
    [Tooltip("Max number of jumps")]
    public int maxJumps = 2;
    [Tooltip("Grace period after leaving ground")]
    public float coyoteTime = 0.15f;
    [Tooltip("Buffer window before landing")]
    public float jumpBufferTime = 0.2f;

    [Header("Fast Fall")]
    [Tooltip("Fast fall gravity multiplier")]
    public float fastFallMultiplier = 2.5f;
    [Tooltip("Fast fall override velocity")]
    public float fastFallVelocity = -15f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool showDebugLogs = true;

    #endregion

    #region Private Fields

    [SerializeField] private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float jumpForce;
    private float gravityScale;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isFastFalling;
    private int jumpCount;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool jumpRequested = false;
    private bool isFacingRight = true;

    // private bool hasReleasedJumpButton = true;

    private enum LastInput { None, Jump, FastFall }
    private LastInput lastInput = LastInput.None;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityScale = rb.gravityScale;
        jumpForce = CalculateJumpForce();

        jumpCount = 0;
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        isJumping = false;
        // hasReleasedJumpButton = true;

        if (showDebugLogs)
            Debug.Log($"Jump Force: {jumpForce}");
    }

    private void Update()
    {
        GatherInput();
        CheckGrounded();
        HandleCoyoteTime();
        HandleJumpBuffer();

        TryJump();
        ApplyVariableJumpCut();
    }

    private void FixedUpdate()
    {
        if (jumpRequested)
        {
            PerformJump();
            jumpRequested = false;
        }
        HandleMovement();
        HandleFastFall();
        ApplyGravityModifiers();
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    #endregion

    #region Input Handling

    private void GatherInput()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            lastInput = LastInput.Jump;
            jumpBufferCounter = jumpBufferTime;
            // hasReleasedJumpButton = false;
        }
        if (Input.GetButtonUp("Jump"))
        {
            // hasReleasedJumpButton = true;
            jumpBufferCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.S))
            lastInput = LastInput.FastFall;

        isFastFalling = (lastInput == LastInput.FastFall) && Input.GetKey(KeyCode.S);
    }

    #endregion

    #region Jumping

    private void TryJump()
    {
        bool canFirst = (isGrounded || coyoteTimeCounter > 0f) && jumpCount < 1;
        bool canSecond = !isGrounded && jumpCount > 0 && jumpCount < maxJumps;

        if (jumpBufferCounter > 0f && (canFirst || canSecond))
        {
            // PerformJump();
            jumpRequested = true;
            jumpBufferCounter = 0f;
        }
    }

    private void PerformJump()
    {
        float force = (jumpCount == 0) ? jumpForce : jumpForce * 0.85f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        jumpCount++;
        isJumping = true;
        coyoteTimeCounter = 0f;

        if (showDebugLogs)
            Debug.Log($"Jump {jumpCount} executed: force={force}");
    }

    private void HandleJumpBuffer()
    {
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;
    }

    private void HandleCoyoteTime()
    {
        if (wasGrounded && !isGrounded)
            coyoteTimeCounter = coyoteTime;
        else if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // After coyote expires on a fall, consume first jump
        if (!isGrounded && coyoteTimeCounter <= 0f && jumpCount == 0)
        {
            jumpCount = 1;
            if (showDebugLogs)
                Debug.Log("Coyote ended: granting one air jump (as if first jump used).");
        }
    }

    private void ApplyVariableJumpCut()
    {
        if (isJumping && Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            isJumping = false;
            if (showDebugLogs)
                Debug.Log("Jump cut applied");
        }
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        float targetSpeed = moveDirection.x * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate;

        if (isGrounded)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? airAcceleration : airDeceleration;
            if (Mathf.Sign(targetSpeed) != Mathf.Sign(rb.linearVelocity.x))
                accelRate *= airControl;
        }

        rb.AddForce(Vector2.right * speedDiff * accelRate, ForceMode2D.Force);
        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);

        // Flip character based on direction
        if (moveDirection.x > 0 && !isFacingRight)
            Flip();
        else if (moveDirection.x < 0 && isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // Multiply the X scale by -1 to mirror the sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    #endregion

    #region Fast Fall & Gravity

    private void HandleFastFall()
    {
        if (isFastFalling && !isGrounded)
        {
            if (rb.linearVelocity.y > fastFallVelocity)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fastFallVelocity);
        }
    }

    private void ApplyGravityModifiers()
    {
        float gravityMul = 1f;
        if (rb.linearVelocity.y < 0f)
            gravityMul = isFastFalling ? fastFallMultiplier : fallMultiplier;
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
            gravityMul = fallMultiplier;

        rb.gravityScale = gravityScale * gravityMul;
    }

    #endregion

    #region Ground Detection & Utilities

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            isJumping = false;
            if (showDebugLogs)
                Debug.Log("Landed, jumpCount reset");
        }

        if (isGrounded != wasGrounded && !isGrounded && !isJumping && showDebugLogs)
            Debug.Log("Walking off edge, coyote started");
    }

    private float CalculateJumpForce() => Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale * jumpHeight);

    #endregion
}
