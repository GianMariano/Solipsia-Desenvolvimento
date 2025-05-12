using UnityEngine;
using System.Collections;

public class DoorParallax : MonoBehaviour
{
    public Transform targetPosition; // Onde o player vai aparecer (área1)
    private bool playerInRange = false;

    [Header("Background Switch")]
    [SerializeField] private GameObject background1;
    [SerializeField] private GameObject background2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Aqui você pode ativar o aviso "Pressione E"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Aqui você pode desativar o aviso
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    private IEnumerator TeleportPlayer()
    {
        // Fade out
        yield return FadeManager.Instance.FadeOut();

        // Move player
        PlayerController.Instance.transform.position = targetPosition.position;

        // Troca de background
        if (background1 != null) background1.SetActive(false);
        if (background2 != null) background2.SetActive(true);

        // Fade in
        yield return FadeManager.Instance.FadeIn();
    }
}
