using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 5f;
    
    private Rigidbody2D rb;
    private bool movingLeft = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        // Destroy the fireball after its lifetime
        Destroy(gameObject, lifetime);
        
        // Determine direction from scale (set by PlayerController)
        movingLeft = transform.localScale.x < 0;
        
        // Apply velocity in the correct direction
        float direction = movingLeft ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
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