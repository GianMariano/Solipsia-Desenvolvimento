using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private int damageAmount = 999; // Only used if instantKill is false

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision is with the player
        PlayerController player = collision.GetComponent<PlayerController>();
        
        if (player != null)
        {
            player.TakeDamage(damageAmount);
        }
    }
}