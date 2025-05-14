using UnityEngine;
public class Lancer : Enemy
{
    public GameObject spearPrefab;
    public Transform shootPoint;
    public float shootCooldown = 1.5f;
    private float lastShootTime;
    private Animator animator;
    public AudioClip spearThrowClip;
    private AudioSource audioSource;


    void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }


    protected override void Update()
    {
        base.Update();

        if (active && player != null)
        {
            animator.SetTrigger("playerDanger");

            Vector2 directionAway = (transform.position - player.transform.position);
            directionAway.y = 0;
            directionAway.Normalize();

            transform.position += (Vector3)directionAway * speed * Time.deltaTime;

            if (Time.time >= lastShootTime + shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }


    void Shoot()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        
        GameObject spear = Instantiate(spearPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        Spear spearScript = spear.GetComponent<Spear>();
        rb.linearVelocity = direction * spearScript.speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spear.transform.rotation = Quaternion.Euler(0, 0, angle + 45f);
        if (spearThrowClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(spearThrowClip);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica apenas se o objeto com o qual colidiu tem a tag "Enemy Limiter"
        if (collision.CompareTag("Enemy Limiter"))
        {
            speed = 0f; // Define a velocidade como zero
        }
    }

    public void StopMovement()
    {
        speed = 0f;
    }




}