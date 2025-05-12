using UnityEngine;

public class WereWolf : Enemy
{
    public enum BossState { Idle, DashingToLeft, DashingToRight, Stunned, ReturningToCenter, Jumping }

    private BossState currentState = BossState.Idle;

    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private float stunDuration = 5f;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float cameraShakeIntensity = 0.2f;
    [SerializeField] private float cameraShakeDuration = 0.5f;
    [SerializeField] private BoxCollider2D doorCollider;
    private Rigidbody2D rb;

    private float stunTimer;
    private bool wasGroundedLastFrame = false;
    bool hasJumped = false;

    private Animator animator;
    private Camera mainCamera;

    protected override void Attack()
    {
    }
    protected override void Start()
    {

        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    protected override void Update()
    {
        if (health <= 0)
        {
            if (doorCollider != null && doorCollider.enabled)
            {
                doorCollider.enabled = false;
                Debug.Log("Porta destravada!");
            }
        }

        base.Update(); // Mantém a lógica de destruição do inimigo e outras coisas da classe Enemy
  
    if (health <= 0) return;

        switch (currentState)
        {
            case BossState.Idle:
                animator.Play("Werewolf_Idle");
                break;

            case BossState.DashingToLeft:
                animator.Play("Werewolf_Run");

                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(leftEdge.position.x, transform.position.y),
                    speed * Time.deltaTime);

                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(transform.position, leftEdge.position) < 0.5f)
                {
                    Debug.Log("Cheguei no LeftEdge");
                    currentState = BossState.Stunned;
                    stunTimer = stunDuration;
                    ShakeCamera();
                }
                break;

            case BossState.DashingToRight:
                animator.Play("Werewolf_Run");

                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(rightEdge.position.x, transform.position.y),
                    speed * Time.deltaTime);

                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(transform.position, rightEdge.position) < 0.5f)
                {
                    Debug.Log("Cheguei no RightEdge");
                    currentState = BossState.Stunned;
                    stunTimer = stunDuration;
                    ShakeCamera();
                }
                break;

            case BossState.Stunned:
                animator.Play("Werewolf_Stunned");
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0)
                {
                    currentState = BossState.ReturningToCenter;
                }
                break;

            case BossState.ReturningToCenter:
                animator.Play("Werewolf_Run");
                MoveTo(centerPoint.position);

                if (Vector2.Distance(transform.position, centerPoint.position) < 0.5f)
                {
                    currentState = BossState.Jumping;
                    hasJumped = false;
                }
                break;

            case BossState.Jumping:
                if (!hasJumped)
                {
                    animator.SetTrigger("Jump");
                    hasJumped = true;
                    JumpTowardsPlayer();
                    Debug.Log("PULADO");
                }

                bool isGrounded = IsGrounded();
                Debug.Log(isGrounded);

                if (isGrounded && !wasGroundedLastFrame)
                {
                    Debug.Log("Boss aterrissou");
                    ShakeCamera();

                    if (PlayerController.Instance.pState.grounded)
                    {
                        PlayerController.Instance.TakeDamage(damage);
                    }

                    hasJumped = false;
                    currentState = DetermineNextDirection();
                }
                break;
        }
    }

    private void ShakeCamera()
    {
        if (mainCamera != null)
        {
            StartCoroutine(ShakeCameraCoroutine());
        }
    }

    private System.Collections.IEnumerator ShakeCameraCoroutine()
    {
        float elapsed = 0f;
        Vector3 originalCameraPosition = mainCamera.transform.position;

        while (elapsed < cameraShakeDuration)
        {
            Vector3 shakeOffset = Random.insideUnitCircle * cameraShakeIntensity;
            mainCamera.transform.position = originalCameraPosition + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
    }

    private WereWolf.BossState DetermineNextDirection()
    {
        float distanceToLeft = Vector2.Distance(PlayerController.Instance.transform.position, leftEdge.position);
        float distanceToRight = Vector2.Distance(PlayerController.Instance.transform.position, rightEdge.position);

        return (distanceToLeft < distanceToRight) ? BossState.DashingToLeft : BossState.DashingToRight;
    }

    private void MoveTo(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.x, transform.position.y), speed * Time.deltaTime);

        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void JumpTowardsPlayer()
    {
        Vector2 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, jumpForce);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == BossState.Idle)
        {
            currentState = DetermineNextDirection();
        }
    }

    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);

        switch (currentState)
        {
            case BossState.Stunned:
                Debug.Log("Dano crítico - boss está atordoado!");
                break;
            default:
                Debug.Log("Boss levou dano!");
                break;
        }
        
    }


}