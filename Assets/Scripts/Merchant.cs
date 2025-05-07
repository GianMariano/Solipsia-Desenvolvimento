using UnityEngine;
using TMPro;
using System.Collections;

public class Merchant : MonoBehaviour
{
    public GameObject dialogueBox;           
    public TextMeshProUGUI dialogueText;     
    private MoneyManager money;
    private PlayerController player;
    public string[] sentences;               
    private int index = 0;
    private bool playerInRange = false;
    private bool dialogueActive = false;

    void Start()
    {
        money = FindObjectOfType<MoneyManager>();
        player = FindObjectOfType<PlayerController>();
        money.UpdateMoney(500);
        player.TakeDamage(2);
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogueActive)
            {
                dialogueBox.SetActive(true);
                dialogueText.text = sentences[index];
                dialogueActive = true;
            }
            else
            {
                if (index == 3)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Sell();
                    }
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        index = 6;
                        dialogueText.text = sentences[index];

                    }
                }
                else
                {
                    index++;
                    if (index < sentences.Length)
                    {
                        dialogueText.text = sentences[index];
                    }
                    else
                    {
                        dialogueBox.SetActive(false);
                        dialogueActive = false;
                        index = 0;
                    }
                }
            }
        }
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
            index = 0;
        }
    }

    void Sell()
    {
        if (money.money < 500)
        {
            dialogueText.text = sentences[4];
            StartCoroutine(CloseDialogueAfterDelay(2f));
        }
        if (player.health == player.maxHealth)
        {
            dialogueText.text = sentences[5];
            StartCoroutine(CloseDialogueAfterDelay(2f));
        }
        else
        {
            money.UpdateMoney(-500);
            int healAmount = Mathf.RoundToInt(player.maxHealth * 0.2f);
            player.GainHealth(healAmount);
            dialogueText.text = sentences[6]; 
            StartCoroutine(CloseDialogueAfterDelay(2f));
        }
    }

    IEnumerator CloseDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBox.SetActive(false);
        dialogueActive = false;
        index = 0;
    }

}
