using UnityEngine;
using System.Collections;

public class Mimic : MonoBehaviour
{
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 ogPosition;
    private bool isJumpingRoutineRunning = false;
    private BoxCollider2D mimicColider;

    [Header("Dash Settings:")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashColliderYScale = 0.7f;
    [SerializeField] public bool lookingRight = true;
    [SerializeField] public bool isDashing = false;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    [Space(5)]

    [SerializeField] private bool willJump = false;
    [SerializeField] private bool willDash = false;
    [SerializeField] private bool willShoot = false;

    [Header("Fireball Settings:")]
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballCooldown = 1.5f;
    [SerializeField] private bool isShooting = false;
    private float fireballCooldownTimer = 0f;

    void Start()
    {
        mimicColider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        ogPosition = transform.position;
        originalColliderSize = mimicColider.size;
        originalColliderOffset = mimicColider.offset;
    }

    void Update()
    {
        if (willJump && !isJumpingRoutineRunning && IsGrounded())
        {
            StartCoroutine(JumpRoutine());
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && IsGrounded())
        {
            animator.Play("Idle");
        }

        if (willDash && !isDashing)
        {
            StartCoroutine(Dash());
        }

        if (willShoot && !isShooting)
        {
            StartCoroutine(ShootFireball());
        }
    }


    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private IEnumerator JumpRoutine()
    {
        isJumpingRoutineRunning = true;

        animator.Play("Idle");
        yield return new WaitForSeconds(1f);

        PerformJump();
        yield return new WaitForSeconds(0.4f);

        PerformJump();
        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => IsGrounded());

        isJumpingRoutineRunning = false;
    }

    private void PerformJump()
    {
        animator.CrossFade("Jump", 0.1f);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    IEnumerator Dash()
    {
        animator.Play("Idle");
        yield return new WaitForSeconds(1f);

        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        mimicColider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * dashColliderYScale);
        float offsetY = (originalColliderSize.y - mimicColider.size.y) / 2;
        mimicColider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - offsetY);

        float dashDirection = lookingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;

        mimicColider.size = originalColliderSize;
        mimicColider.offset = originalColliderOffset;

        yield return new WaitForSeconds(1f);

        transform.position = ogPosition;

        isDashing = false;
    }

    IEnumerator ShootFireball()
    {
        isShooting = true;
        lookingRight = true;
        animator.Play("Idle");
        yield return new WaitForSeconds(1f);

        fireballCooldownTimer = fireballCooldown;
    
        yield return new WaitForSeconds(fireballCooldown);
        
        Vector3 fireballDirection = lookingRight ? Vector3.right : Vector3.left;

        // Create the fireball
        if (fireballSpawnPoint != null && fireballPrefab != null)
        {
            // Instantiate the fireball at the spawn point
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

            // Set the fireball's direction based on player facing
            if (!lookingRight)
            {
                // Flip the fireball if player is facing left
                fireball.transform.localScale = new Vector3(-fireball.transform.localScale.x, fireball.transform.localScale.y, fireball.transform.localScale.z);
            }
        }
        isShooting = false;
    }

}
