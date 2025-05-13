using UnityEngine;
using System.Collections;

public class ErevosBoss : MonoBehaviour
{
    [Header("Atributos Básicos")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damage = 2f;
    [SerializeField] private int goldReward = 500;
    
    [Header("Estado do Boss")]
    [SerializeField] private bool isInvulnerable = true;
    [SerializeField] private bool isPhase2 = false;
    [SerializeField] private float phaseTransitionHealthPercent = 0.5f;
    
    [Header("Posicionamento")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private Transform midPoint;
    
    [Header("Movimentação")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f; // Velocidade maior para fase 2
    [SerializeField] private float closeRangeDistance = 2f; // Distância considerada "perto" do player
    [SerializeField] private bool canMove = true;
    [SerializeField] private float distanceThreshold = 0.1f; // Distância mínima para considerar chegada ao destino
    
    [Header("Ataques")]
    [SerializeField] private ErevosFireball fireballAttack;
    [SerializeField] private float basicAttackCooldown = 3f;
    [SerializeField] private float specialAttackCooldown = 8f;
    [SerializeField] private float vulnerableDuration = 5f;
    [SerializeField] private bool canAttack = true;
    
    [Header("Efeitos")]
    [SerializeField] private GameObject vulnerableVisualEffect;
    [SerializeField] private GameObject phaseTransitionEffect;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject hitEffect;
    
    private Animator animator;
    private bool isTakingDamageCooldown = false;
    private bool isAttacking = false;
    private Coroutine currentAttackRoutine;
    private bool isMoving = false;
    private Vector3 currentDestination;
    private MoneyManager moneyManager;
    private bool isDead = false;
    private bool isStunned = false;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        // Se o fireballAttack não foi configurado no inspector, procure no filho
        if (fireballAttack == null)
            fireballAttack = GetComponentInChildren<ErevosFireball>(true);
    }
    
    private void Start()
    {
        // Tenta encontrar o MoneyManager na cena
        GameObject moneyManagerObj = GameObject.Find("MoneyManager");
        if (moneyManagerObj != null)
        {
            moneyManager = moneyManagerObj.GetComponent<MoneyManager>();
        }
        
        if (vulnerableVisualEffect != null)
            vulnerableVisualEffect.SetActive(false);
            
        // Inicia a rotina de ataque
        StartCoroutine(AttackCycle());
    }
    
    private void Update()
    {
        if (isDead || isStunned)
            return;
            
        // Verificar transição de fase
        if (!isPhase2 && health <= maxHealth * phaseTransitionHealthPercent)
        {
            StartCoroutine(TransitionToPhase2());
        }
        
        // Atualiza movimento se estiver se movendo para um destino
        if (isMoving && canMove)
        {
            MoveTowardsDestination();
        }
    }
    
    private void MoveTowardsDestination()
    {
        if (currentDestination == null)
            return;
            
        // Calcula a direção e distância
        Vector3 direction = currentDestination - transform.position;
        direction.y = 0; // Mantém a mesma altura
        float distance = direction.magnitude;
        
        // Se chegou ao destino, para de mover
        if (distance <= distanceThreshold)
        {
            isMoving = false;
            animator.SetBool("Walking", false);
            return;
        }
        
        // Normaliza a direção e move
        direction.Normalize();
        float currentSpeed = isPhase2 ? runSpeed : walkSpeed;
        transform.position += direction * currentSpeed * Time.deltaTime;
        
        // Atualiza a escala para virar na direção correta
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        
        // Atualiza animação
        animator.SetBool("Walking", true);
    }
    
    private IEnumerator AttackCycle()
    {
        // Aguarda um pequeno tempo antes de iniciar os ataques
        yield return new WaitForSeconds(1.5f);
        
        while (!isDead)
        {
            // Se estiver em movimento ou não puder atacar, pula
            if (!canAttack || isStunned)
            {
                yield return null;
                continue;
            }
            
            // Se puder atacar, escolhe um ataque baseado na fase
            if (!isAttacking)
            {
                isAttacking = true;
                
                if (isPhase2)
                {
                    yield return StartCoroutine(Phase2AttackPattern());
                }
                else
                {
                    yield return StartCoroutine(Phase1AttackPattern());
                }
                
                isAttacking = false;
                
                // Após um ciclo de ataques, torna o boss vulnerável
                if (!isInvulnerable)
                {
                    yield return StartCoroutine(BecomeVulnerable(vulnerableDuration));
                }
            }
            
            yield return null;
        }
    }
    
    private IEnumerator Phase1AttackPattern()
    {
        // Padrão de ataque da fase 1
        yield return StartCoroutine(WalkTo(midPoint));
        
        // Realizar o ataque de bolas de fogo 2 vezes
        for (int i = 0; i < 2; i++)
        {
            if (fireballAttack != null)
            {
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0.5f); // Tempo de animação antes de atirar
                fireballAttack.FireLeafPatternAttack(1); // 1 rajada
            }
            yield return new WaitForSeconds(basicAttackCooldown);
        }
        
        // Caminha para uma borda e ataca
        yield return StartCoroutine(WalkTo(leftEdge));
        
        if (fireballAttack != null)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            fireballAttack.FireLeafPatternAttack(1);
        }
        
        yield return new WaitForSeconds(basicAttackCooldown);
        
        // Caminha para outra borda e ataca
        yield return StartCoroutine(WalkTo(rightEdge));
        
        if (fireballAttack != null)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            fireballAttack.FireLeafPatternAttack(1);
        }
    }
    
    private IEnumerator Phase2AttackPattern()
    {
        // Padrão de ataque mais intenso da fase 2
        yield return StartCoroutine(WalkTo(midPoint));
        
        // Ataque especial de 3 rajadas
        if (fireballAttack != null)
        {
            animator.SetTrigger("SpecialAttack");
            yield return new WaitForSeconds(0.7f); // Tempo de preparação maior para ataque especial
            fireballAttack.FireLeafPatternAttack(3); // 3 rajadas
        }
        
        yield return new WaitForSeconds(specialAttackCooldown * 0.5f);
        
        // Patrulha mais rápido e ataca de ambos os lados
        yield return StartCoroutine(WalkTo(leftEdge));
        
        if (fireballAttack != null)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.4f);
            fireballAttack.FireLeafPatternAttack(2); // 2 rajadas
        }
        
        yield return new WaitForSeconds(basicAttackCooldown * 0.6f); // Cooldown reduzido na fase 2
        
        yield return StartCoroutine(WalkTo(rightEdge));
        
        if (fireballAttack != null)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.4f);
            fireballAttack.FireLeafPatternAttack(2);
        }
        
        yield return new WaitForSeconds(basicAttackCooldown * 0.6f);
        
        // Retorna ao centro para um último ataque
        yield return StartCoroutine(WalkTo(midPoint));
        
        if (fireballAttack != null)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            fireballAttack.FireLeafPatternAttack(1);
        }
    }
    
    private IEnumerator WalkTo(Transform target)
    {
        // Se não puder mover, retorna imediatamente
        if (!canMove) yield break;
        
        // Define o destino e inicia o movimento
        currentDestination = new Vector3(target.position.x, transform.position.y, transform.position.z);
        isMoving = true;
        
        // Aguarda até chegar ao destino ou ser interrompido
        while (isMoving && Vector3.Distance(transform.position, currentDestination) > distanceThreshold)
        {
            // Se o boss for atordoado ou morrer durante o movimento, interrompe
            if (isStunned || isDead)
            {
                isMoving = false;
                animator.SetBool("Walking", false);
                yield break;
            }
            
            yield return null;
        }
        
        // Chegou ao destino, para de andar
        isMoving = false;
        animator.SetBool("Walking", false);
        
        // Pequena pausa após chegar
        yield return new WaitForSeconds(0.3f);
        
        // Ajusta a escala para olhar para o player após chegar
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
    
    private IEnumerator TransitionToPhase2()
    {
        // Evita que a transição seja chamada várias vezes
        isPhase2 = true;
        
        // Pausa os ataques e torna invulnerável durante a transição
        canAttack = false;
        isInvulnerable = true;
        canMove = false;
        isMoving = false;
        isStunned = true;
        
        // Para qualquer movimento
        animator.SetBool("Walking", false);
        
        // Vai para o centro
        yield return StartCoroutine(WalkTo(midPoint));
        
        // Animação de transição de fase
        animator.SetTrigger("PhaseChange");
        
        // Efeito visual da transição
        if (phaseTransitionEffect != null)
        {
            GameObject effect = Instantiate(phaseTransitionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2.5f);
        }
        
        // Aguarda a conclusão da animação
        yield return new WaitForSeconds(3f);
        
        // Continua com os ataques
        canAttack = true;
        canMove = true;
        isStunned = false;
    }
    
    private IEnumerator BecomeVulnerable(float duration)
    {
        isInvulnerable = false;
        isTakingDamageCooldown = false;
        isStunned = true;
        
        // Para o movimento
        isMoving = false;
        canMove = false;
        animator.SetBool("Walking", false);
        
        // Animação de atordoado
        animator.SetTrigger("Stunned");
        
        // Ativa efeito visual de vulnerabilidade
        if (vulnerableVisualEffect != null)
            vulnerableVisualEffect.SetActive(true);
            
        yield return new WaitForSeconds(duration);
        
        // Remove estado de vulnerabilidade
        isInvulnerable = true;
        isStunned = false;
        canMove = true;
        
        // Desativa efeito visual
        if (vulnerableVisualEffect != null)
            vulnerableVisualEffect.SetActive(false);
            
        // Retorna ao estado idle
        animator.SetTrigger("RecoverFromStun");
    }
    
    // Método para receber dano
    public void TakeDamage(float damageAmount)
    {
        if (isInvulnerable || isTakingDamageCooldown || isDead)
            return;
            
        StartCoroutine(DamageCooldownRoutine(damageAmount));
    }
    
    private IEnumerator DamageCooldownRoutine(float damageAmount)
    {
        isTakingDamageCooldown = true;
        
        // Aplica o dano
        health -= damageAmount;
        
        // Efeito visual de dano
        animator.SetTrigger("TakeDamage");
        
        // Efeito de impacto
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
        
        // Debug de dano
        Debug.Log("Erevos Boss took " + damageAmount + " damage. Health: " + health);
        
        // Verifica morte
        if (health <= 0 && !isDead)
        {
            Die();
            yield break;
        }
        
        // Tempo de cooldown entre danos
        yield return new WaitForSeconds(0.5f);
        isTakingDamageCooldown = false;
    }
    
    private void Die()
    {
        isDead = true;
        isStunned = true;
        isInvulnerable = true;
        canMove = false;
        isMoving = false;
        
        // Para o movimento
        animator.SetBool("Walking", false);
        
        // Cancela qualquer coroutine ativa
        StopAllCoroutines();
        
        // Animação de morte
        animator.SetTrigger("Death");
        
        // Adiciona recompensa monetária
        if (moneyManager != null)
        {
            moneyManager.UpdateMoney(goldReward);
        }
        
        // Cria efeito de morte
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }
        
        // Desativa componentes de colisão
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        // Destroi o boss após a animação de morte
        StartCoroutine(DestroyAfterDelay(3f));
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    
    // Método para atacar o jogador em caso de colisão
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDead && !isStunned)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.pState.invincible && !player.isDead)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer < closeRangeDistance)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }
    
    // Método público para forçar imediatamente um ataque (útil para eventos)
    public void ForceAttack(int rajadas = 1)
    {
        if (fireballAttack != null && !isDead && !isStunned)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(DelayedFireballAttack(rajadas, 0.5f));
        }
    }
    
    private IEnumerator DelayedFireballAttack(int rajadas, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (fireballAttack != null)
        {
            fireballAttack.FireLeafPatternAttack(rajadas);
        }
    }
    
    // Getters públicos para saúde
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;
}