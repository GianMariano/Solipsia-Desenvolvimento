using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Transform SideAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 SideAttackArea; //how large the area of side attack is
    [SerializeField] private float timeBetweenAttack;
    [SerializeField] private LayerMask attackableLayer; //the layer the player can attack and recoil off of
    [SerializeField] private float damage; //the damage the player does to an enemy
    private float timeSinceAttack;
    private bool attack = false;
    [Space(5)]

    [Header("Fireball Settings:")]
    [SerializeField] private GameObject fireballPrefab; // Assign this in Inspector
    [SerializeField] private Transform fireballSpawnPoint; // Create an empty child object as spawn point
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
    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private float xAxis, yAxis;
    private float gravity;
    private int jumpCount = 0; // Simple counter for jumps
    private bool shoot = false;
    private bool jumpButtonPressedLastFrame = false; // Track button state
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private bool canDashNow = true;
    public static PlayerController Instance; 
    public GameObject gameOver;
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

        GameObject moneyManagerObj = GameObject.Find("MoneyManager");
        if (moneyManagerObj != null)
        {
            moneyManager = moneyManagerObj.GetComponent<MoneyManager>();
        }
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
        // Check if player can shoot and right mouse button is pressed
        if (shoot && canShoot && fireballReady)
        {
            ShootFireball();
        }
    }

    void ShootFireball()
    {
        // Set cooldown
        fireballReady = false;
        fireballCooldownTimer = fireballCooldown;
        
        // Determine the direction based on player facing
        Vector3 fireballDirection = pState.lookingRight ? Vector3.right : Vector3.left;
        
        // Create the fireball
        if (fireballSpawnPoint != null && fireballPrefab != null)
        {
            // Instantiate the fireball at the spawn point
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            
            // Set the fireball's direction based on player facing
            if (!pState.lookingRight)
            {
                // Flip the fireball if player is facing left
                fireball.transform.localScale = new Vector3(-fireball.transform.localScale.x, fireball.transform.localScale.y, fireball.transform.localScale.z);
            }
            
            Debug.Log("Fireball shot!");
        }
        else
        {
            Debug.LogError("Fireball prefab or spawn point not assigned!");
        }
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
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
        // Check for F key press to trigger dash
        if (Input.GetKeyDown(KeyCode.F) && canDash && canDashNow && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        // Set dash state and reset cooldown
        isDashing = true;
        canDashNow = false;
        dashCooldownTimer = dashCooldown;
        
        // Store original gravity
        float originalGravity = rb.gravityScale;
        
        // Remove gravity during dash
        rb.gravityScale = 0;
        
        // Get dash direction (based on player facing)
        float dashDirection = pState.lookingRight ? 1f : -1f;
        
        // Apply dash force
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);
        
        // Optional: Make player invincible during dash
        pState.invincible = true;
        
        // Debug log
        Debug.Log("Dashing!");
        
        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);
        
        // Reset everything post-dash
        isDashing = false;
        rb.gravityScale = originalGravity;
        pState.invincible = false;
        
        Debug.Log("Dash complete!");
    }

    public void TakeDamage(float _damage)
    {
        Debug.Log("Player levou dano!");
        if (pState.invincible) return;
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
        healthBar.SetHealth(health);
    }

    void ShowGameOver()
    {
        if (gameOver != null)
        {
            gameOver.SetActive(true); // Ativa a tela de Game Over
            Time.timeScale = 0f; // Pausa o jogo
        }
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
            if (health == 0)
            {
                ShowGameOver();
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
                Debug.Log("First jump executed. Jump count: " + jumpCount);
            }
            // Double jump in air
            else if (!Grounded() && canDoubleJump && jumpCount < 2)
            {
                PerformJump();
                Debug.Log("Double jump executed. Jump count: " + jumpCount);
            }
        }

        // Update button tracking
        jumpButtonPressedLastFrame = jumpButtonPressed;
        
        // Set animation state
        anim.SetBool("Jumping", !Grounded());
    }

    void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            // Destroi o item
            Destroy(collision.gameObject);

            // Adiciona dinheiro (você precisará acessar o PointManager)
            if (moneyManager != null)
            {
                moneyManager.UpdateMoney(100);
            }
        }
    }
}