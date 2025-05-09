using UnityEngine;

public class Spear : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public float damage = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
