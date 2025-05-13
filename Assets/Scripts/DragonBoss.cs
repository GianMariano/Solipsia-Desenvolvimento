using UnityEngine;

public class DragonBoss : Enemy
{
    public enum DragonState { Idle, Flying, BreathingFire, Landing }
    
    private DragonState currentState = DragonState.Idle;

    [Header("Dialogue Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float timeBetweenLines = 2f;
    [SerializeField] private GameObject dialogueBox; // Referência a um objeto UI de diálogo
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText; // Componente de texto
    private bool isInDialogue = false;
    private int currentLine = 0;
    private float dialogueTimer = 0f;
    
    [Header("Dragon Settings")]
    [SerializeField] private float flyHeight = 5f;
    [SerializeField] private float flySpeed = 3f;
    [SerializeField] private float fireBreathDuration = 4f;
    [SerializeField] private float timeBetweenFireballs = 0.5f;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private float fireballSpeed = 7f;
    [SerializeField] private float groundPhaseDuration = 5f;
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private float cameraShakeIntensity = 0.2f;
    [SerializeField] private float cameraShakeDuration = 0.5f;
    

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 groundPosition;
    private float fireBreathTimer;
    private float fireballTimer;
    private float groundPhaseTimer;
    private Camera mainCamera;
    private bool isFacingRight = true;
    private Vector3 originalScale;
    private bool battleStarted = false;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        groundPosition = transform.position;
        
        originalScale = transform.localScale;
        
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

        base.Update();
        UpdateDialogue();

        if (health <= 0 || !battleStarted || isInDialogue) return;

        switch (currentState)
        {
            case DragonState.Idle:
                HandleIdleState();
                break;
                
            case DragonState.Flying:
                HandleFlyingState();
                break;
                
            case DragonState.BreathingFire:
                HandleBreathingFireState();
                break;
                
            case DragonState.Landing:
                HandleLandingState();
                break;
        }
        
        UpdateFacingDirection();
    }

    private void StartDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            StartBattle();
            return;
        }

        isInDialogue = true;
        currentLine = 0;
        dialogueTimer = timeBetweenLines;
        
        // Ativa a caixa de diálogo e mostra a primeira linha
        if (dialogueBox != null) dialogueBox.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (dialogueText != null && currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
    }

    private void UpdateDialogue()
    {
        if (!isInDialogue) return;

        dialogueTimer -= Time.deltaTime;
        
        if (dialogueTimer <= 0)
        {
            currentLine++;
            
            if (currentLine >= dialogueLines.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowCurrentLine();
                dialogueTimer = timeBetweenLines;
            }
        }
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        StartBattle();
    }

    private void StartBattle()
    {
        battleStarted = true;
        currentState = DragonState.Flying;
    }

    private void HandleIdleState()
    {
        animator.Play("Dragon_Idle");
        
        groundPhaseTimer -= Time.deltaTime;
        if (groundPhaseTimer <= 0)
        {
            currentState = DragonState.Flying;
        }
    }

    private void HandleFlyingState()
    {
        // Voar para cima
        Vector2 targetPosition = new Vector2(groundPosition.x, groundPosition.y + flyHeight);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, flySpeed * Time.deltaTime);
        
        // Começar a atirar assim que começar a voar (remova a verificação de distância)
        currentState = DragonState.BreathingFire;
        fireBreathTimer = fireBreathDuration;
        animator.Play("Dragon_breath");
        
        // Atualizar o timer do fireball para atirar imediatamente
        fireballTimer = 0;
    }

    private void HandleBreathingFireState()
    {
        // Continuar voando para cima enquanto atira
        Vector2 targetPosition = new Vector2(groundPosition.x, groundPosition.y + flyHeight);
        if (transform.position.y < targetPosition.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, flySpeed * Time.deltaTime);
        }
        
        fireBreathTimer -= Time.deltaTime;
        fireballTimer -= Time.deltaTime;
        
        // Atirar bolas de fogo periodicamente
        if (fireballTimer <= 0)
        {
            ShootFireball();
            fireballTimer = timeBetweenFireballs;
        }
        
        // Quando o tempo de respiração de fogo acabar, começar a descer
        if (fireBreathTimer <= 0)
        {
            currentState = DragonState.Landing;
        }
    }

    private void HandleLandingState()
    {
        // Voltar para o chão
        transform.position = Vector2.MoveTowards(transform.position, groundPosition, flySpeed * Time.deltaTime);
        
        // Quando chegar no chão, voltar ao estado Idle
        if (Vector2.Distance(transform.position, groundPosition) < 0.1f)
        {
            currentState = DragonState.Idle;
            groundPhaseTimer = groundPhaseDuration;
            animator.Play("Dragon_Idle");
            ShakeCamera();
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || fireballSpawnPoint == null)
        {
            Debug.LogError("Fireball prefab ou spawn point não configurados!");
            return;
        }
        
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        BossFireball fireballScript = fireball.GetComponent<BossFireball>();
        
        if (fireballScript != null)
        {
            int direction = isFacingRight ? 1 : -1;
            Debug.Log($"Atirando fireball na direção: {direction}"); // Log para debug
            fireballScript.Launch(direction, fireballSpeed);
        }
        else
        {
            Debug.LogError("Componente BossFireball não encontrado no prefab!");
        }
    }

    private void UpdateFacingDirection()
    {
        if (PlayerController.Instance != null)
        {
            // Virar para o jogador
            isFacingRight = PlayerController.Instance.transform.position.x > transform.position.x;
            transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == DragonState.Idle && !battleStarted && !isInDialogue)
        {
            StartDialogue();
        }
    }

    public override void EnemyHit(float _damageDone)
    {
        // Só pode receber dano quando estiver no chão
        if (currentState == DragonState.Idle)
        {
            base.EnemyHit(_damageDone);
            Debug.Log("Dragão levou dano!");
        }
        else
        {
            Debug.Log("Dragão está voando - invulnerável!");
        }
    }
}