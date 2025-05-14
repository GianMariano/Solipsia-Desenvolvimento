using UnityEngine;
using System.Collections;

public class ErevosBoss : Enemy
{
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
    
    private Animator animator;
    private bool isAttacking = false;
    private bool isMoving = false;
    private Vector3 currentDestination;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool isTakingDamageCooldown = false;
    private bool bossActivated = false; // Nova flag para controlar a ativação do boss
    private Coroutine attackPatternCoroutine; // Referência para a coroutine do padrão de ataque
    
    protected override void Start()
    {
        // Não chama base.Start() para evitar conflitos com a classe Enemy
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (fireballAttack == null)
            fireballAttack = GetComponentInChildren<ErevosFireball>(true);
            
        // NÃO inicia o padrão de ataque - esperamos a ativação pelo trigger
        
        // Configura o trigger de ativação
        SetupTriggerArea();
        
        Debug.Log("ErevosBoss inicializado. Esperando player entrar na área para começar a atacar.");
    }
    
    private void SetupTriggerArea()
    {
        // Se não tiver um trigger definido, cria um
        if (triggerAreaObject == null)
        {
            // Criar objeto para o trigger
            GameObject triggerObject = new GameObject("BossTriggerArea");
            triggerObject.transform.position = transform.position;
            triggerObject.transform.parent = transform.parent; // Mesmo parent do boss
            
            // Adiciona um BoxCollider2D como trigger
            BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector2(10f, 5f); // Tamanho da área de trigger - ajuste conforme necessário
            
            // Adiciona o script de trigger
            BossTriggerArea triggerScript = triggerObject.AddComponent<BossTriggerArea>();
            triggerScript.SetBoss(this);
            
            triggerAreaObject = triggerObject.transform;
        }
        else
        {
            // Se já tem um objeto definido, garante que ele tem o script e o collider trigger
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
            
            // Garante que é um trigger
            triggerCollider.isTrigger = true;
        }
    }
    
    // Método chamado quando o player entrar na área do trigger
    public void ActivateBoss()
    {
        if (!bossActivated && !isDead)
        {
            bossActivated = true;
            Debug.Log("Boss ativado! Começando o padrão de ataque.");
            
            // Inicia o padrão de ataque
            attackPatternCoroutine = StartCoroutine(MovementPattern());
        }
    }
    
    // Sobrescreve o método EnemyHit da classe Enemy
    public override void EnemyHit(float damageAmount)
    {
        Debug.Log($"ErevosBoss.EnemyHit chamado com dano: {damageAmount}");
        
        // Se estiver invulnerável ou em cooldown, ignora o dano
        if (isInvulnerable || isTakingDamageCooldown || isDead)
        {
            Debug.Log($"Boss não tomou dano - Invulnerável: {isInvulnerable}, Cooldown: {isTakingDamageCooldown}, Morto: {isDead}");
            return;
        }
        
        // Aplica o dano
        bossHealth -= damageAmount;
        
        // Inicia o cooldown para evitar dano múltiplo
        StartCoroutine(DamageCooldown());
        
        // Efeito visual de dano (pisca vermelho)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }
        
        Debug.Log($"Boss tomou {damageAmount} de dano. Vida: {bossHealth}/{maxBossHealth}");
        
        // Verifica se morreu
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
        // Atualiza movimento se estiver se movendo para um destino
        if (isMoving && !isDead)
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
            return;
        }
        
        // Normaliza a direção e move
        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Atualiza a escala para virar na direção correta
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
            // Se estiver atacando, aguarda
            if (isAttacking)
            {
                yield return null;
                continue;
            }
            
            // Padrão de movimento e ataque
            // Centro -> Esquerda -> Direita -> Centro
            
            // Vai para o centro e ataca
            yield return StartCoroutine(MoveTo(midPoint));
            yield return StartCoroutine(PerformAttack(2));
            
            // Vai para a esquerda e ataca
            yield return StartCoroutine(MoveTo(leftEdge));
            yield return StartCoroutine(PerformAttack(3));
            
            // Vai para a direita e ataca
            yield return StartCoroutine(MoveTo(rightEdge));
            yield return StartCoroutine(PerformAttack(3));
            
            // Volta para o centro e ataca novamente
            yield return StartCoroutine(MoveTo(midPoint));
            yield return StartCoroutine(PerformAttack(2));
            
            // Torna-se vulnerável após o ciclo de ataques
            yield return StartCoroutine(BecomeVulnerable(5f));
            
            // Pequena pausa entre ciclos
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator MoveTo(Transform target)
    {
        // Define o destino e inicia o movimento
        currentDestination = new Vector3(target.position.x, transform.position.y, transform.position.z);
        isMoving = true;
        
        // Aguarda até chegar ao destino
        while (isMoving && Vector3.Distance(transform.position, currentDestination) > distanceThreshold)
        {
            if (isDead)
                yield break;
                
            yield return null;
        }
        
        // Chegou ao destino, para de andar
        isMoving = false;
        
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
    
    private IEnumerator PerformAttack(int numberOfFireballs)
    {
        isAttacking = true;
        
        // Adiciona um pouco de tempo antes do ataque
        yield return new WaitForSeconds(0.2f);
        
        // Dispara o número especificado de bolas de fogo
        for (int i = 0; i < numberOfFireballs; i++)
        {
            if (isDead)
                yield break;
                
            // Dispara uma animação de ataque
            animator.SetTrigger("Attack");
            
            // Espera o tempo da animação antes de disparar
            yield return new WaitForSeconds(0.5f);
            
            // Dispara a bola de fogo
            if (fireballAttack != null)
            {
                fireballAttack.FireLeafPatternAttack(1);
            }
            
            // Cooldown entre bolas de fogo
            if (i < numberOfFireballs - 1)
            {
                yield return new WaitForSeconds(attackCooldown);
            }
        }
        
        // Tempo de espera após a sequência de ataques
        yield return new WaitForSeconds(1f);
        
        isAttacking = false;
    }
    
    private IEnumerator BecomeVulnerable(float duration)
    {
        // Torna o boss vulnerável
        isInvulnerable = false;
        
        // Inicia o efeito de piscar para indicar vulnerabilidade
        StartCoroutine(BlinkEffect(duration));
        
        Debug.Log("Boss está vulnerável por " + duration + " segundos");
        
        yield return new WaitForSeconds(duration);
        
        // Volta a ser invulnerável
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
        
        // Garante que o sprite fique visível no final
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
        
        // Destrói o GameObject imediatamente
        Destroy(gameObject);
    }
    
    // Método público para forçar um ataque imediato (útil para eventos)
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
                boss.ActivateBoss();
            }
            
            Debug.Log("Player entrou na área do boss!");
        }
    }
}