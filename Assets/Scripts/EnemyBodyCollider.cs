using UnityEngine;

public class EnemyBodyCollider : MonoBehaviour
{
    private Lancer lancer;

    void Start()
    {
        lancer = GetComponentInParent<Lancer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy Limiter"))
        {
            lancer.StopMovement();
        }
    }
}

