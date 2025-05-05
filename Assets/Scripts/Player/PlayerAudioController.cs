using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip runClip;
    public AudioClip preJumpClip;
    public AudioClip postJumpClip;

    [Header("Settings")]
    public float runVolume = 0.5f;
    public float jumpVolume = 0.7f;

    private PlayerController player;
    private Rigidbody2D rb;
    private AudioSource runSource;
    private AudioSource oneShotSource;

    private bool wasGroundedLastFrame = false;

    void Start()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        // Audio source for looping run
        runSource = gameObject.AddComponent<AudioSource>();
        runSource.clip = runClip;
        runSource.loop = true;
        runSource.volume = runVolume;

        // Audio source for one-shot SFX (jump)
        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.loop = false;
        oneShotSource.volume = jumpVolume;
    }

    void Update()
    {
        HandleRunSound();
        HandleJumpSounds();
    }

    private void HandleRunSound()
    {
        bool isMovingHorizontally = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isGrounded = IsGrounded();

        if (isGrounded && isMovingHorizontally)
        {
            if (!runSource.isPlaying)
                runSource.Play();
        }
        else
        {
            if (runSource.isPlaying)
                runSource.Stop();
        }
    }

    private void HandleJumpSounds()
    {
        bool isGrounded = IsGrounded();

        // Pre-jump (leaving the ground)
        if (wasGroundedLastFrame && !isGrounded)
        {
            oneShotSource.PlayOneShot(preJumpClip);
        }

        // Post-jump (landing)
        if (!wasGroundedLastFrame && isGrounded)
        {
            oneShotSource.PlayOneShot(postJumpClip);
        }

        wasGroundedLastFrame = isGrounded;
    }

    private bool IsGrounded()
    {
        // Use same logic as PlayerController (sync method if needed)
        return Physics2D.OverlapCircle(player.groundCheck.position, player.groundCheckRadius, player.groundLayer);
    }
}
