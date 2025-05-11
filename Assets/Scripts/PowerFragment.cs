using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerFragment : MonoBehaviour
{
    [Header("Powers to Grant")]
    [SerializeField] private bool grantDash = false;
    [SerializeField] private bool grantDoubleJump = false;
    [SerializeField] private bool grantShoot = false;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject collectEffect;
    
    [Header("Pulse Effect Settings")]
    [SerializeField] private float minScale = 0.9f;  // Minimum scale factor
    [SerializeField] private float maxScale = 1.1f;  // Maximum scale factor
    [SerializeField] private float pulseSpeed = 2.0f; // Speed of the pulse effect
    
    private Vector3 originalScale;
    
    private void Start()
    {
        // Store the original scale
        originalScale = transform.localScale;
    }
    
    private void Update()
    {
        // Calculate the pulse factor using a sine wave
        float pulseFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
        
        // Apply the pulsing scale effect
        transform.localScale = originalScale * pulseFactor;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collected the power fragment
        PlayerController player = collision.GetComponent<PlayerController>();
        
        if (player != null)
        {
            // Grant the selected powers
            if (grantDash)
            {
                player.canDash = true;
                Debug.Log("Dash power granted!");
            }
            
            if (grantDoubleJump)
            {
                player.canDoubleJump = true;
                Debug.Log("Double Jump power granted!");
            }
            
            if (grantShoot)
            {
                player.canShoot = true;
                Debug.Log("Fireball power granted!");
            }
            
            // Play collection effect if assigned
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
            
            // Destroy the power fragment
            Destroy(gameObject);
        }
    }
}