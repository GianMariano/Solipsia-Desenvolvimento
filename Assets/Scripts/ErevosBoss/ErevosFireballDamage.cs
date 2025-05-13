using UnityEngine;

public class ErevosFireballDamage : MonoBehaviour
{
    public float damage = 2f;
    public float speed = 8f;
    
    private void Start()
    {
        // Se não tiver velocidade definida externamente, auto-destruir após tempo
        Destroy(gameObject, 5f);
    }
    
    // Este método pode ser chamado pelo ErevosFireball para definir a velocidade
    public void SetVelocity(Vector2 direction, float newSpeed)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            speed = newSpeed;
            rb.velocity = direction * speed;
            
            // Rotacionar a bola de fogo na direção do movimento
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
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
            
            // Cria efeito de impacto (opcional)
            CreateImpactEffect();
            
            // Destrói a bola de fogo
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            // Cria efeito de impacto em superfícies (opcional)
            CreateImpactEffect();
            
            // Destrói a bola de fogo quando colide com paredes ou chão
            Destroy(gameObject);
        }
    }
    
    private void CreateImpactEffect()
    {
        // Aqui você pode instanciar um efeito de impacto como partículas ou animação
        // Exemplo:
        // GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        // Destroy(impact, 1f);
    }
}