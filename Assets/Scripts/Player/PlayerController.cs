using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimentação: ")]
    [SerializeField] private float walkSpeed = 1;
    [SerializeField] private float jumpForce = 45;
    [Space(5)]

    [Header("Configurações de Ground Check:")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; 
    [SerializeField] private Vector2 SideAttackArea; 
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private LayerMask attackableLayer; 
    [SerializeField] private float damage; 
    private float timeSinceAttack;
    private bool attack = false;
    [Space(5)]

    [Header("Fireball Settings:")]
    [SerializeField] private GameObject fireballPrefab; 
    [SerializeField] private Transform fireballSpawnPoint; 
    [SerializeField] private float fireballCooldown = 0.5f;
    private float fireballCooldownTimer = 0f;
    private bool fireballReady = true;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    public HealthBar healthBar;
    [Space(5)]

    [Header("Power Ups:")]
    [SerializeField] public bool canDash = true;
    [SerializeField] public bool canShoot = true;
    [SerializeField] public bool canDoubleJump = true;
    [Space(5)]

    [Header("Dash Settings:")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashColliderYScale = 0.5f; // How much to reduce collider height (0.5 = half size)
    [Space(5)]

    [Header("Checkpoint Settings")]
    private Vector3 respawnPosition;
    private bool checkpointReached = false;
    [Space(5)]

    [Header("Audio Settings:")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private float xAxis, yAxis;
    private float gravity;
    private int jumpCount = 0; 
    private bool shoot = false;
    private bool jumpButtonPressedLastFrame = false; 
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private bool canDashNow = true;
    private BoxCollider2D playerCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    public static PlayerController Instance; 
    public GameObject gameOver;
    public bool isDead = false;
    [SerializeField] private MoneyManager moneyManager;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        Health = maxHealth;
    }
    
    void Start()
    {   
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthBar.SetMaxHealth(maxHealth);
        audioSource = GetComponent<AudioSource>();

        // Get reference to the box collider
        playerCollider = GetComponent<BoxCollider2D>();
        if (playerCollider != null)
        {
            // Store the original collider size and offset
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset;
        }

        GameObject moneyManagerObj = GameObject.Find("MoneyManager");
        if (moneyManagerObj != null)
        {
            moneyManager = moneyManagerObj.GetComponent<MoneyManager>();
        }

        respawnPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    void Update()
    {
        GetInputs();
        
        if (!isDashing)
        {
            Move();
            HandleJumpLogic();
            Flip();
            Attack();
            CheckDash();
            CheckFireball();
        }
        
        // Update dash cooldown timer
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canDashNow = true;
        }
        
        // Update fireball cooldown timer
        if (fireballCooldownTimer > 0)
        {
            fireballCooldownTimer -= Time.deltaTime;
        }
        else
        {
            fireballReady = true;
        }

    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetMouseButtonDown(0);
        shoot = Input.GetMouseButtonDown(1); // Right mouse button
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());

        if (Grounded() && Mathf.Abs(rb.linearVelocity.x) > 0.1f && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(walkSound);
        }
    }

    public bool Grounded()
    {
        bool isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
        
        // Reset jump counter when grounded
        if (isGrounded)
        {
            jumpCount = 0;
        }
        
        return isGrounded;
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    void CheckFireball()
    {
        
        if (shoot && canShoot && fireballReady)
        {
            ShootFireball();
        }
    }

    void ShootFireball()
    {
        
        fireballReady = false;
        fireballCooldownTimer = fireballCooldown;
        
        
        Vector3 fireballDirection = pState.lookingRight ? Vector3.right : Vector3.left;
        
        audioSource.PlayOneShot(shootSound);
        
        if (fireballSpawnPoint != null && fireballPrefab != null)
        {
            
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            
            
            if (!pState.lookingRight)
            {
                
                fireball.transform.localScale = new Vector3(-fireball.transform.localScale.x, fireball.transform.localScale.y, fireball.transform.localScale.z);
            }
        }
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            audioSource.PlayOneShot(attackSound);
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(SideAttackTransform, SideAttackArea);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        
        List<Enemy> hitEnemies = new List<Enemy>();

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage);
                hitEnemies.Add(e);
            }
        }
    }

    void CheckDash()
    {
        
        if (Input.GetKeyDown(KeyCode.F) && canDash && canDashNow && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        
        audioSource.PlayOneShot(dashSound);
        isDashing = true;
        canDashNow = false;
        dashCooldownTimer = dashCooldown;
        
        
        float originalGravity = rb.gravityScale;
        
        
        rb.gravityScale = 0;
        
        
        if (playerCollider != null)
        {
            
            playerCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * dashColliderYScale);
            
            
            float offsetY = (originalColliderSize.y - playerCollider.size.y) / 2;
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - offsetY);
        }
        
        
        float dashDirection = pState.lookingRight ? 1f : -1f;
        
        
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);
        
        
        pState.invincible = true;
        
        
        yield return new WaitForSeconds(dashDuration);
        
        
        isDashing = false;
        rb.gravityScale = originalGravity;
        pState.invincible = false;
        
        
        if (playerCollider != null)
        {
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }
    }

    public void TakeDamage(float _damage)
    {
        if (pState.invincible || isDead) return;
        audioSource.PlayOneShot(hurtSound);
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
        healthBar.SetHealth(health);
    }

    public void GainHealth(float _damage)
    {
        Health += Mathf.RoundToInt(_damage);
        healthBar.SetHealth(health);
    }

    public void RespawnPlayer()
    {
        transform.position = respawnPosition;
        Health = maxHealth;
        healthBar.SetHealth(health);
        rb.linearVelocity = Vector2.zero; 
        
        // Resetar todos os estados importantes
        isDead = false;
        pState.invincible = false;
        
        // Garantir que as animações sejam resetadas
        anim.ResetTrigger("Dead");
        anim.ResetTrigger("TakeDamage");
        anim.Play("Idle"); // Ou sua animação padrão
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("TakeDamage");
        ClampHealth();
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
            if (health == 0 && !isDead)
            {
                isDead = true;
                GameOverController.Instance.TriggerGameOver();  // Exibe a tela de Game Over 
                rb.linearVelocity = Vector2.zero;  // Impede qualquer movimento quando morto
                anim.SetTrigger("Dead");  // Aciona a animação de morte 
            }
        }
    }

    void HandleJumpLogic()
    {
        bool jumpButtonPressed = Input.GetButtonDown("Jump");
        
        // Handle jump height control when releasing button
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Only register a new jump press (prevents holding button from triggering multiple jumps)
        if (jumpButtonPressed && !jumpButtonPressedLastFrame)
        {
            // First jump from ground
            if (Grounded())
            {
                PerformJump();
                audioSource.PlayOneShot(jumpSound);
            }
            // Double jump in air
            else if (!Grounded() && canDoubleJump && jumpCount < 2)
            {
                PerformJump();
                audioSource.PlayOneShot(jumpSound);
            }
        }

        // Update button tracking
        jumpButtonPressedLastFrame = jumpButtonPressed;
        
        // Set animation state
        anim.SetBool("Jumping", !Grounded());
    }

    protected void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cooper"))
        {
            // Destroi o item
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(collectSound);
            
            if (moneyManager != null)
            {
                moneyManager.UpdateMoney(100);
            }
        }
        else if (collision.CompareTag("Silver"))
        {
            
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(collectSound);
            
            if (moneyManager != null)
            {
                moneyManager.UpdateMoney(300);
            }
        }
        else if (collision.CompareTag("Gold"))
        {
            
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(collectSound);
            
            if (moneyManager != null)
            {
                moneyManager.UpdateMoney(500);
            }
        }
    }
    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
        checkpointReached = true;
    }
}