using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialogueBox;           
    public TextMeshProUGUI dialogueText;     
    public string[] sentences;               
    private int index = 0;
    private bool playerInRange = false;
    private bool dialogueActive = false;

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
}
