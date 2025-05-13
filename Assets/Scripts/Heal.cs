using UnityEngine;
using TMPro;
using System.Collections;

public class Heal : MonoBehaviour
{
    private PlayerController player;
    private bool playerInRange = false;
    private Vector3 startPos;
    
    
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        startPos = transform.position;
    }

    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            int healAmount = Mathf.RoundToInt(player.maxHealth * 0.2f);
                player.GainHealth(healAmount);
                gameObject.SetActive(false);
        }
        FloatEffect();
    }
        void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

        }
    }
    void FloatEffect()
    {
        float floatY = Mathf.Sin(Time.time * 2f) * 0.05f;
        transform.position = startPos + new Vector3(0, floatY, 0);
    }
}