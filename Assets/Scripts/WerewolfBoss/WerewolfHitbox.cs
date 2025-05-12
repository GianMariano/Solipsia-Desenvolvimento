using UnityEngine;

public class WerewolfHitbox : MonoBehaviour
{
    private bool isAttacking = false;
    int damage = 2;

    void Start()
    {
        if (PlayerController.Instance != null)
        {
            Debug.Log("PlayerController Instance está funcionando");
        }
        else
        {
            Debug.LogError("PlayerController Instance não está acessível!");
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < 1f && !isAttacking) // ajuste para evitar ataques repetidos
            {
                isAttacking = true;
                Attack();
            }
        }
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    // Resetando o ataque quando o player sai da hitbox
    protected void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }


}
