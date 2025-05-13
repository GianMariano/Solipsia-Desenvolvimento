using UnityEngine;
using System.Collections;

public class ErevosFireball : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float fireballSpeed = 8f;
    [SerializeField] private float fireballLifetime = 5f;
    [SerializeField] private int fireballDamage = 2;
    
    [Header("Configurações do Padrão em Leque")]
    [SerializeField] private int fireballsPerRajada = 3;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float delayBetweenFireballs = 0.1f;
    [SerializeField] private float delayBetweenRajadas = 0.7f;
    
    [Header("Efeitos")]
    [SerializeField] private GameObject chargingEffect;
    [SerializeField] private GameObject muzzleFlashEffect;
    [SerializeField] private AudioClip fireballSound;
    
    private AudioSource audioSource;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && fireballSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (chargingEffect != null)
            chargingEffect.SetActive(false);
    }
    
    // Método principal para disparar várias rajadas de bolas de fogo em leque
    public void FireLeafPatternAttack(int numberOfRajadas)
    {
        StartCoroutine(FireLeafPatternCoroutine(numberOfRajadas));
    }
    
    private IEnumerator FireLeafPatternCoroutine(int numberOfRajadas)
    {
        // Ativa o efeito de carregamento se existir
        if (chargingEffect != null)
            chargingEffect.SetActive(true);
            
        yield return new WaitForSeconds(0.2f); // Tempo curto de carregamento
        
        for (int rajada = 0; rajada < numberOfRajadas; rajada++)
        {
            FireLeafPattern();
            
            // Desativa o efeito de carregamento entre rajadas
            if (chargingEffect != null && rajada < numberOfRajadas - 1)
                chargingEffect.SetActive(false);
                
            // Aguarda entre rajadas
            if (rajada < numberOfRajadas - 1)
            {
                yield return new WaitForSeconds(delayBetweenRajadas);
                
                // Reativa o efeito de carregamento para a próxima rajada
                if (chargingEffect != null)
                    chargingEffect.SetActive(true);
                    
                yield return new WaitForSeconds(0.2f);
            }
        }
        
        // Desativa o efeito de carregamento ao finalizar todas as rajadas
        if (chargingEffect != null)
            chargingEffect.SetActive(false);
    }
    
    private void FireLeafPattern()
    {
        // Reproduz o som do disparo
        if (audioSource != null && fireballSound != null)
            audioSource.PlayOneShot(fireballSound);
            
        // Cria o efeito de muzzle flash
        if (muzzleFlashEffect != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashEffect, shootPoint.position, Quaternion.identity);
            Destroy(muzzleFlash, 0.5f);
        }
        
        // Calcula a direção base apontando para o player
        Vector2 baseDirection = Vector2.right;
        if (PlayerController.Instance != null)
        {
            baseDirection = (PlayerController.Instance.transform.position - shootPoint.position).normalized;
        }
        
        // Calcula o ângulo inicial para o leque
        float totalSpreadAngle = spreadAngle * (fireballsPerRajada - 1);
        float startAngle = -totalSpreadAngle / 2f;
        
        // Inicia a sequência de disparo das bolas de fogo
        StartCoroutine(FireSequence(baseDirection, startAngle));
    }
    
    private IEnumerator FireSequence(Vector2 baseDirection, float startAngle)
    {
        for (int i = 0; i < fireballsPerRajada; i++)
        {
            // Calcula o ângulo para esta bola de fogo
            float currentAngle = startAngle + (i * spreadAngle);
            
            // Calcula a direção rotacionada
            Vector2 direction = RotateVector(baseDirection, currentAngle);
            
            // Dispara a bola de fogo
            FireSingleFireball(direction);
            
            // Pequena pausa entre os disparos da mesma rajada
            yield return new WaitForSeconds(delayBetweenFireballs);
        }
    }
    
    private void FireSingleFireball(Vector2 direction)
    {
        if (fireballPrefab == null || shootPoint == null)
            return;
            
        // Instancia a bola de fogo
        GameObject fireball = Instantiate(fireballPrefab, shootPoint.position, Quaternion.identity);
        
        // Configura a bola de fogo
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * fireballSpeed;
        }
        
        // Rotaciona a bola de fogo na direção do movimento
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        fireball.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Configura comportamento especial se for um BossFireball
        BossFireball bossFireball = fireball.GetComponent<BossFireball>();
        if (bossFireball != null)
        {
            int directionSign = direction.x >= 0 ? 1 : -1;
            bossFireball.Launch(directionSign, fireballSpeed);
            bossFireball.lifetime = fireballLifetime;
        }
        else
        {
            // Destrói a bola de fogo após um tempo se não tiver script próprio
            Destroy(fireball, fireballLifetime);
        }
        
        // Configura o dano para objetos que colidam com o player
        FireballDamage damageComponent = fireball.GetComponent<FireballDamage>();
        if (damageComponent == null)
        {
            damageComponent = fireball.AddComponent<FireballDamage>();
        }
        damageComponent.damage = fireballDamage;
    }
    
    // Método auxiliar para rotacionar um vetor
    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        
        return new Vector2(
            (vector.x * cos) - (vector.y * sin),
            (vector.x * sin) + (vector.y * cos)
        );
    }
}

// Classe auxiliar para aplicar dano ao player
public class FireballDamage : MonoBehaviour
{
    public float damage = 2f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aplica dano ao player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            
            // Destrói a bola de fogo
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            // Destrói a bola de fogo quando colide com paredes ou chão
            Destroy(gameObject);
        }
    }
}