using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private string targetTag = "DestructibleObject"; // Tag for objects with colliders to disable
    
    private Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        // Destroy the fireball after its lifetime
        Destroy(gameObject, lifetime);
        
        // Get direction from scale (negative X scale means moving left)
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        
        // Set velocity in the correct direction
        rb.velocity = new Vector2(direction * speed, 0);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit the target object
        if (collision.CompareTag(targetTag))
        {
            // Get the target object component and disable its collider
            DestructibleObject destructible = collision.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.DisableCollider();
            }
            
            // Destroy the fireball
            Destroy(gameObject);
        }
        
        // Check if we hit an enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Deal damage to the enemy
            enemy.EnemyHit(damage);
            
            // Destroy the fireball
            Destroy(gameObject);
        }
        
        // If fireball hits a ground layer object, destroy it
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}