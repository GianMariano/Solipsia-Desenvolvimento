using System.Collections;
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
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float pulseSpeed = 2.0f;

    [Header("Audio")]
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;
    
    private Vector3 originalScale;
    [SerializeField] private GameObject floatingTextPrefab;

    private void Start()
    {
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        float pulseFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
        transform.localScale = originalScale * pulseFactor;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        
        if (player != null)
        {
            StartCoroutine(CollectPowerFragment(player));
        }
    }

    private IEnumerator CollectPowerFragment(PlayerController player)
    {
        // Desativa o collider para evitar múltiplas coletas
        GetComponent<Collider2D>().enabled = false;
       
       if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Inicia o fade out
        yield return WhiteFadeController.Instance.StartWhiteFadeEffect(() => {
            player.RespawnPlayer();
        });
        
        if (grantDash)
        {
            player.canDash = true;
            Debug.Log("Dash power granted!");
            ShowFloatingText(player, "Dash Desbloqueado! (F)");
            Debug.Log("Texto flutuante instanciado!");
        }
        
        if (grantDoubleJump)
        {
            player.canDoubleJump = true;
            Debug.Log("Double Jump power granted!");
            ShowFloatingText(player, "Pulo Duplo Desbloqueado! (Espaço)");
        }
        
        if (grantShoot)
        {
            player.canShoot = true;
            Debug.Log("Fireball power granted!");
            ShowFloatingText(player, "Magia de Fogo Desbloqueada! (Mouse2)");
        }
        
        ColorProgressManager.Instance.CollectFragment();

        

        Destroy(gameObject);
    }

    private void ShowFloatingText(PlayerController player, string message)
    {
        if (floatingTextPrefab != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab);
            textObj.GetComponent<FloatingText>().Initialize(player.transform, message);
        }
    }
}