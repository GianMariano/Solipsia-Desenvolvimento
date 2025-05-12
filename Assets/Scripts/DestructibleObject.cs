using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] private bool destroyEntireGameObject = false; // Whether to destroy the entire GameObject or just visuals
    [SerializeField] private float destroyDelay = 0.2f;
    [SerializeField] private GameObject destroyEffect;
    
    [Header("Destruction Particle Settings")]
    [SerializeField] private bool createFragments = true;
    [SerializeField] private GameObject fragmentPrefab;
    [SerializeField] private int fragmentCount = 5;
    [SerializeField] private float fragmentForce = 3f;
    
    private BoxCollider2D boxCollider;
    private SpriteRenderer[] spriteRenderers;
    
    private void Awake()
    {
        // Get components
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); // Get all sprite renderers in this object and children
    }
    
    public void DisableCollider()
    {
        // Disable the box collider immediately
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
        
        // Create destruction effect if assigned
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        
        // Create fragments for destruction visual
        if (createFragments && fragmentPrefab != null)
        {
            CreateDestructionFragments();
        }
        
        // Start the destruction process
        StartCoroutine(DestroySprites());
    }
    
    private IEnumerator DestroySprites()
    {
        // Disable all sprite renderers to make the object "disappear"
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        
        // Wait for the specified delay
        yield return new WaitForSeconds(destroyDelay);
        
        // Destroy the entire GameObject if specified
        if (destroyEntireGameObject)
        {
            Destroy(gameObject);
        }
        // Otherwise, we've already disabled the sprites and collider
    }
    
    private void CreateDestructionFragments()
    {
        // Create fragment particles to show the object breaking apart
        for (int i = 0; i < fragmentCount; i++)
        {
            // Calculate random position within the bounds of the object
            Bounds bounds = boxCollider != null ? boxCollider.bounds : new Bounds(transform.position, Vector3.one);
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            Vector3 randomPos = new Vector3(randomX, randomY, transform.position.z);
            
            // Create fragment
            GameObject fragment = Instantiate(fragmentPrefab, randomPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            
            // Add force to make it fly outward
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calculate direction from center of object
                Vector2 direction = ((Vector2)(fragment.transform.position - transform.position)).normalized;
                if (direction == Vector2.zero) direction = Random.insideUnitCircle.normalized;
                
                // Add force
                rb.AddForce(direction * fragmentForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
                
                // Destroy fragment after some time
                Destroy(fragment, 2f);
            }
        }
    }
}