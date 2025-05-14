using UnityEngine;
using UnityEngine.SceneManagement;  // Para carregar a cena
using System.Collections;
using UnityEngine.UI; // Para usar os botões

public class ErevosBoss : Enemy
{
    [Header("Dialogue Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float timeBetweenLines = 2f;
    [SerializeField] private GameObject dialogueBox; // Referência a um objeto UI de diálogo
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText; // Componente de texto
    private bool isInDialogue = false;
    private int currentLine = 0;
    private float dialogueTimer = 0f;

    [Header("Choices")]
    [SerializeField] private Button yesButton; // Botão SIM
    [SerializeField] private Button noButton;  // Botão NÃO

    [Header("Sistema de Vida")]
    [SerializeField] private float bossHealth = 100f;
    [SerializeField] private float maxBossHealth = 100f;
    [SerializeField] private bool isInvulnerable = true; // Começa invulnerável
    
    [Header("Posicionamento")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private Transform midPoint;
    [SerializeField] private Transform triggerAreaObject; // Objeto que contém o trigger de ativação
    
    [Header("Movimentação")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float distanceThreshold = 0.1f;
    
    [Header("Ataques")]
    [SerializeField] private ErevosFireball fireballAttack;
    [SerializeField] private float attackCooldown = 3f;

    [Header("Fade")]
    [SerializeField] private WhiteFadeController whiteFadeController;  // Referência para o fade branco

    private Animator animator;
    private bool isAttacking = false;
    private bool isMoving = false;
    private Vector3 currentDestination;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool isTakingDamageCooldown = false;
    private bool bossActivated = false; // Nova flag para controlar a ativação do boss
    private Coroutine attackPatternCoroutine; // Referência para a coroutine do padrão de ataque
    private bool choiceMade = false;
    
    
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (fireballAttack == null)
            fireballAttack = GetComponentInChildren<ErevosFireball>(true);
        
        // Configura o trigger de ativação
        SetupTriggerArea();
        
        Debug.Log("ErevosBoss inicializado. Esperando player entrar na área para começar a atacar.");
    }

    private void SetupTriggerArea()
    {
        // Se não tiver um trigger definido, cria um
        if (triggerAreaObject == null)
        {
            GameObject triggerObject = new GameObject("BossTriggerArea");
            triggerObject.transform.position = transform.position;
            triggerObject.transform.parent = transform.parent; // Mesmo parent do boss
            
            BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector2(10f, 5f); // Tamanho da área de trigger - ajuste conforme necessário
            
            BossTriggerArea triggerScript = triggerObject.AddComponent<BossTriggerArea>();
            triggerScript.SetBoss(this);
            
            triggerAreaObject = triggerObject.transform;
        }
        else
        {
            BossTriggerArea triggerScript = triggerAreaObject.GetComponent<BossTriggerArea>();
            if (triggerScript == null)
            {
                triggerScript = triggerAreaObject.gameObject.AddComponent<BossTriggerArea>();
                triggerScript.SetBoss(this);
            }
            
            BoxCollider2D triggerCollider = triggerAreaObject.GetComponent<BoxCollider2D>();
            if (triggerCollider == null)
            {
                triggerCollider = triggerAreaObject.gameObject.AddComponent<BoxCollider2D>();
                triggerCollider.isTrigger = true;
                triggerCollider.size = new Vector2(10f, 5f); // Tamanho padrão - ajuste conforme necessário
            }
            
            triggerCollider.isTrigger = true;
        }
    }

    public void StartDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            ActivateBoss();
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

    public void UpdateDialogue()
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
        choiceMade = true;  
    }
    
    private void ShowChoiceButtons(bool show)
    {
        if (yesButton != null && noButton != null)
        {
            yesButton.gameObject.SetActive(show);
            noButton.gameObject.SetActive(show);

            // Adiciona os listeners para os botões
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            if (show)
            {
                yesButton.onClick.AddListener(ChooseYes);
                noButton.onClick.AddListener(ChooseNo);
            }
            else
            {
                yesButton.onClick.RemoveListener(ChooseYes);
                noButton.onClick.RemoveListener(ChooseNo);
            }   
        }
    }

    public void ChooseYes()
    {
        // Se o jogador escolheu SIM, vai para a cena BadEnding
        Debug.Log("Jogador escolheu SIM. Indo para BadEnding...");
        SceneManager.LoadScene("BadEnding");
    }

    public void ChooseNo()
    {
    // Se o jogador escolheu NÃO, a batalha começa
        Debug.Log("Jogador escolheu NÃO. Iniciando a batalha...");
        ShowChoiceButtons(false);
        ActivateBoss();
    }
    private void SimulateButtonClick(Button button)
    {
        if (button != null)
        {
            button.onClick.Invoke();  // Simula o clique no botão
        }
    }

    // Método chamado quando o player entrar na área do trigger
    public void ActivateBoss()
    {
        if (!bossActivated && !isDead)
        {
            bossActivated = true;
            Debug.Log("Boss ativado! Começando o padrão de ataque.");
            
            attackPatternCoroutine = StartCoroutine(MovementPattern());
        }
    }    

    public override void EnemyHit(float damageAmount)
    {
        Debug.Log($"ErevosBoss.EnemyHit chamado com dano: {damageAmount}");

        if (isInvulnerable || isTakingDamageCooldown || isDead)
        {
            Debug.Log($"Boss não tomou dano - Invulnerável: {isInvulnerable}, Cooldown: {isTakingDamageCooldown}, Morto: {isDead}");
            return;
        }

        bossHealth -= damageAmount;

        StartCoroutine(DamageCooldown());

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }

        Debug.Log($"Boss tomou {damageAmount} de dano. Vida: {bossHealth}/{maxBossHealth}");

        if (bossHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    private IEnumerator DamageCooldown()
    {
        isTakingDamageCooldown = true;
        yield return new WaitForSeconds(0.2f);
        isTakingDamageCooldown = false;
    }

    private void Update()
    {
        UpdateDialogue();

        if (choiceMade) 
        {
            ShowChoiceButtons(true);

            if (Input.GetKeyDown(KeyCode.Y))
            {
                SimulateButtonClick(yesButton);  // Simula o clique no botão SIM
                choiceMade = false;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                SimulateButtonClick(noButton);   // Simula o clique no botão NÃO
                choiceMade = false;
            }
        }

        if (isMoving && !isDead)
        {
            MoveTowardsDestination();
        }
    }

    private void MoveTowardsDestination()
    {
        if (currentDestination == null)
            return;

        Vector3 direction = currentDestination - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        if (distance <= distanceThreshold)
        {
            isMoving = false;
            return;
        }

        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator MovementPattern()
    {
        while (!isDead)
        {
            if (isAttacking)
            {
                yield return null;
                continue;
            }

            yield return StartCoroutine(MoveTo(midPoint));
            yield return StartCoroutine(PerformAttack(2));

            yield return StartCoroutine(MoveTo(leftEdge));
            yield return StartCoroutine(PerformAttack(3));

            yield return StartCoroutine(MoveTo(rightEdge));
            yield return StartCoroutine(PerformAttack(3));

            yield return StartCoroutine(MoveTo(midPoint));
            yield return StartCoroutine(PerformAttack(2));

            yield return StartCoroutine(BecomeVulnerable(5f));

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator MoveTo(Transform target)
    {
        currentDestination = new Vector3(target.position.x, transform.position.y, transform.position.z);
        isMoving = true;

        while (isMoving && Vector3.Distance(transform.position, currentDestination) > distanceThreshold)
        {
            if (isDead)
                yield break;

            yield return null;
        }

        isMoving = false;

        yield return new WaitForSeconds(0.3f);

        if (PlayerController.Instance != null)
        {
            Vector3 directionToPlayer = PlayerController.Instance.transform.position - transform.position;
            if (directionToPlayer.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private IEnumerator PerformAttack(int numberOfFireballs)
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < numberOfFireballs; i++)
        {
            if (isDead)
                yield break;

            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(0.5f);

            if (fireballAttack != null)
            {
                fireballAttack.FireLeafPatternAttack(1);
            }

            if (i < numberOfFireballs - 1)
            {
                yield return new WaitForSeconds(attackCooldown);
            }
        }

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    private IEnumerator BecomeVulnerable(float duration)
    {
        isInvulnerable = false;

        StartCoroutine(BlinkEffect(duration));

        Debug.Log("Boss está vulnerável por " + duration + " segundos");

        yield return new WaitForSeconds(duration);

        isInvulnerable = true;

        Debug.Log("Boss não está mais vulnerável");
    }

    private IEnumerator BlinkEffect(float duration)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime && !isDead)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(0.1f);
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    private void Die()
{
    isDead = true;
    isInvulnerable = true;
    isAttacking = false;
    isMoving = false;

    Debug.Log("Boss morreu!");

    // Desativa os colliders
    Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
    foreach (Collider2D col in colliders)
    {
        col.enabled = false;
    }

    // Verifica se o whiteFadeController não é nulo antes de usá-lo
    if (WhiteFadeController.Instance != null)
    {
        // Inicia o efeito de fade branco e carrega a cena GoodEnding
        StartCoroutine(WhiteFadeController.Instance.StartWhiteFadeEffect(() =>
        {
            // Adiciona uma pequena pausa antes de carregar a cena (opcional, para garantir que o fade aconteça primeiro)
            SceneManager.LoadScene("GoodEnding");
        }));
    }
    else
    {
        Debug.LogError("WhiteFadeController não foi atribuído ao ErevosBoss!");
    }

    // Destrói o GameObject
    //Destroy(gameObject);
}

    public void ForceAttack(int rajadas = 1)
    {

        if (fireballAttack != null && !isAttacking && !isDead)
        {
            StartCoroutine(PerformAttack(rajadas));
        }
    }
}

// Classe de trigger que ativa o boss quando o player entra na área
public class BossTriggerArea : MonoBehaviour
{
    private ErevosBoss boss;
    private bool triggered = false;
    
    public void SetBoss(ErevosBoss targetBoss)
    {
        boss = targetBoss;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            
            if (boss != null)
            {
                boss.StartDialogue();
            }
            
            Debug.Log("Player entrou na área do boss!");
        }
    }
}