using UnityEngine;
using TMPro;
using System.Collections;

public class Heal : MonoBehaviour
{
    private PlayerController player;
    private bool playerInRange = false;
    private Vector3 startPos;

    public AudioClip healSound; // Áudio da cura
    private AudioSource audioSource;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        startPos = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            int healAmount = Mathf.RoundToInt(player.maxHealth * 0.2f);
            player.GainHealth(healAmount);

            // Toca o som de cura, se houver
            if (healSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(healSound);
            }

            // Desativa o objeto após um pequeno delay para deixar o som tocar
            StartCoroutine(DisableAfterSound());
        }

        FloatEffect();
    }

    IEnumerator DisableAfterSound()
    {
        yield return new WaitForSeconds(0.2f); // Pequeno tempo para deixar o som tocar
        gameObject.SetActive(false);
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
        transform.position = startPos + new Vector3(0, floatY, 0);
    }
}
