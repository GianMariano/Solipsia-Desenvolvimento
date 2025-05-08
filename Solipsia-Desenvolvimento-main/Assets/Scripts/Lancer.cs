using UnityEngine;

public class Lancer : Enemy
{
    public GameObject spearPrefab;
    public Transform shootPoint;
    public float shootCooldown = 1.5f;
    private float lastShootTime;
    private Animator animator;

    void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        Vector2 direction = (player.transform.position - shootPoint.position).normalized;

        GameObject spear = Instantiate(spearPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * 10f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spear.transform.rotation = Quaternion.Euler(0, 0, angle + 45f); // compensando a inclinação original do sprite
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy Limiter") && collision.gameObject != gameObject)
        {
            Debug.Log("BATI");
            speed = 0f;
        }
    }




}
