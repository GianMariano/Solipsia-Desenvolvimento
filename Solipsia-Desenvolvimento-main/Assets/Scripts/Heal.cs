using UnityEngine;
using TMPro;
using System.Collections;

public class Heal : MonoBehaviour
{
    private PlayerController player;
    public string[] sentences; 
    public GameObject dialogueBox; 
    public TextMeshProUGUI dialogueText;   
    private bool playerInRange = false;
    private bool dialogueActive = false;
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
            if (player.health == player.maxHealth)
            {
                dialogueText.text = sentences[0];
                StartCoroutine(CloseDialogueAfterDelay(2f));
            }
            else
            {
                int healAmount = Mathf.RoundToInt(player.maxHealth * 0.2f);
                player.GainHealth(healAmount);
                Destroy(gameObject);
            }
        }
        FloatEffect();
    }

    IEnumerator CloseDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBox.SetActive(false);
        dialogueActive = false;
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
            dialogueBox.SetActive(false);
            dialogueActive = false;
        }
    }
    void FloatEffect()
    {
        float floatY = Mathf.Sin(Time.time * 2f) * 0.05f;
        transform.position = startPos + new Vector3(0, floatY, 0);
    }
}
