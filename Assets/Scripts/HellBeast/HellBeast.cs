using UnityEngine;
using System.Collections;

public class HellBeast : Enemy
{
    public GameObject fireballPrefab;
    public GameObject healPrefab;
    public Transform leftEdge;
    public Transform rightEdge;
    public Transform midle;
    public Transform shootPoint;
    private Animator animator;
    private float originalHealth;

    public bool isInvulnerable = true;
    private bool isOnRight = true;

    private Coroutine phase1Routine;
    private bool isPhase2Active = false;

    public GameObject phase2Object;
    private bool isTakingDamageCooldown = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalHealth = health;
        // phase1Routine = StartCoroutine(AttackCycle1());
        StartCoroutine(BecomeVulnerable(5f));

        if (healPrefab != null)
            healPrefab.SetActive(false);
    }

    IEnumerator AttackCycle1()
    {
        while (true)
        {
            yield return StartCoroutine(TeleportAfterDelay(3f));
            yield return StartCoroutine(ShootFireballBurst(1, 1f, 0.5f));

            yield return StartCoroutine(TeleportAfterDelay(3f));
            yield return StartCoroutine(ShootFireballBurst(1, 1f, 0.5f));

            yield return StartCoroutine(TeleportAfterDelay(0.5f));
            yield return StartCoroutine(ShootFireballBurst(1, 1f, 0.5f));

            yield return StartCoroutine(TeleportAfterDelay(0.5f));
            yield return StartCoroutine(ShootFireballBurst(1, 1f, 0.5f));

            yield return StartCoroutine(TeleportAfterDelay(1.5f));
            yield return StartCoroutine(ShootFireballBurst(3, 2f, 0.7f));

            yield return StartCoroutine(TeleportAfterDelay(1.5f));
            yield return StartCoroutine(ShootFireballBurst(3, 2f, 0.7f));

            yield return StartCoroutine(TeleportAfterDelay(1.5f));
            yield return StartCoroutine(ShootFireballBurst(4, 2f, 0.5f));
            yield return StartCoroutine(ShootFireballBurst(2, 3f, 0.8f));

            yield return StartCoroutine(TeleportAfterDelay(1.5f));
            yield return StartCoroutine(ShootFireballBurst(4, 2f, 0.5f));
            yield return StartCoroutine(ShootFireballBurst(2, 3f, 0.8f));

            yield return StartCoroutine(BecomeVulnerable(5f));
        }
    }

    IEnumerator TeleportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isOnRight)
        {
            transform.position = leftEdge.position;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.position = rightEdge.position;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        isOnRight = !isOnRight;
    }

    IEnumerator ShootFireballBurst(int quantity, float speed, float cooldownBetweenShots)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        float xDirection = player.transform.position.x - transform.position.x;
        int direction = xDirection >= 0 ? 1 : -1;

        for (int i = 0; i < quantity; i++)
        {
            GameObject fireballObj = Instantiate(fireballPrefab, shootPoint.position, Quaternion.identity);
            BossFireball fireball = fireballObj.GetComponent<BossFireball>();
            fireball.Launch(direction, speed);

            yield return new WaitForSeconds(cooldownBetweenShots);
        }
    }

    public IEnumerator BecomeVulnerable(float flashDuration)
    {
        isInvulnerable = false;
        animator.SetTrigger("isStunned");

        if (healPrefab != null)
            healPrefab.SetActive(true);

        yield return new WaitForSeconds(flashDuration);

        animator.SetTrigger("isStunned");
        isInvulnerable = true;
    }

    public void MakeVulnerableNow(float duration)
    {
        StartCoroutine(BecomeVulnerable(duration));
    }

    public override void EnemyHit(float _damageDone)
    {
        if (!isInvulnerable && !isTakingDamageCooldown)
        {
            StartCoroutine(DamageCooldownRoutine(_damageDone));
        }
    }

    private IEnumerator DamageCooldownRoutine(float damage)
    {
        isTakingDamageCooldown = true;

        health -= damage;
        base.EnemyHit(damage);

        Debug.Log(isPhase2Active);
        if (health <= originalHealth / 2 && !isPhase2Active)
        {
            StartCoroutine(SwitchToPhase2());
        }

        yield return new WaitForSeconds(1f);
        isTakingDamageCooldown = false;
    }

    private IEnumerator SwitchToPhase2()
    {
        Debug.Log("chamado");
        isPhase2Active = true;

        if (phase1Routine != null)
            StopCoroutine(phase1Routine);
        Debug.Log("é aqui certeza");

        HellBeastPhase2 phase2Script = phase2Object.GetComponent<HellBeastPhase2>();
        phase2Script.hellBeastScript = this;

        phase2Object.SetActive(true);
        gameObject.SetActive(false);
        
        Debug.Log("CARALHO");
        isPhase2Active = false;

        yield return null;
    }
}