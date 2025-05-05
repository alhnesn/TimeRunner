using UnityEngine;

public class PlayerControllerEyll : MonoBehaviour
{
    #region Inspector Fields

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 60f;
    public float airAcceleration = 30f;
    public float airDeceleration = 20f;
    [Range(0f, 1f)] public float airControl = 0.8f;

    [Header("Jumping")]
    public float jumpHeight = 1.5f;
    public float jumpApexTime = 0.4f;
    public float jumpCutMultiplier = 0.5f;
    public float fallMultiplier = 1.5f;
    public int maxJumps = 2;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.2f;

    [Header("Fast Fall")]
    public float fastFallMultiplier = 2.5f;
    public float fastFallVelocity = -15f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool showDebugLogs = true;

    #endregion

    #region Private Fields

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float jumpForce;
    private float gravityScale;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isFastFalling;
    private bool isClimbing;
    private bool isFacingRight = true;
    private int jumpCount;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool jumpRequested = false;

    private enum LastInput { None, Jump, FastFall }
    private LastInput lastInput = LastInput.None;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gravityScale = rb.gravityScale;
        jumpForce = CalculateJumpForce();

        jumpCount = 0;
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;

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
        UpdateAnimator();
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
        HandleLadderClimb();
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
        }
        if (Input.GetButtonUp("Jump"))
        {
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

        if (!isClimbing && jumpBufferCounter > 0f && (canFirst || canSecond))
        {
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

    #region Movement & Climbing

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

        if (moveDirection.x > 0 && !isFacingRight)
            Flip();
        else if (moveDirection.x < 0 && isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void HandleLadderClimb()
    {
        if (IsTouchingLadder() && Input.GetKey(KeyCode.W))
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveSpeed);
        }
        else if (!Input.GetKey(KeyCode.W) || !IsTouchingLadder())
        {
            isClimbing = false;
            rb.gravityScale = gravityScale;
        }
    }

    private bool IsTouchingLadder()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f, groundLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Ladder"))
                return true;
        }
        return false;
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

        if (!isClimbing)
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

        if (!isGrounded && wasGrounded && showDebugLogs)
            Debug.Log("Walking off edge, coyote started");
    }

    private float CalculateJumpForce()
        => Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale * jumpHeight);

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("isRunning", Mathf.Abs(moveDirection.x) > 0.01f && isGrounded);
        animator.SetBool("isJumping", !isGrounded && rb.linearVelocity.y > 0f && !isClimbing);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isClimbing", isClimbing);
    }

    #endregion
}
