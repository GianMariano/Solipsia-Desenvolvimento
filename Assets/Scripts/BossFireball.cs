using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossFireball : MonoBehaviour
{
    private float speed;
    private int direction;

    public float lifetime = 5f;

    public void Launch(int xDirection, float fireballSpeed)
    {
        direction = xDirection;
        speed = fireballSpeed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
