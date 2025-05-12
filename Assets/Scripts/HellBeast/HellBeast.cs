using UnityEngine;
using System.Collections;

public class HellBeast : Enemy
{
    public GameObject fireballPrefab;
    public Transform leftEdge;
    public Transform rightEdge;
    public Transform shootPoint;
    private Animator animator;

    public bool isInvulnerable = true;

    private bool isOnRight = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ChargeAttack(5f));
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

    IEnumerator ChargeAttack(float duration)
    {
        Debug.Log("METODO CHAMADO");
        animator.SetBool("isCharge", true);
        yield return new WaitForSeconds(duration);
        animator.SetBool("isCharge", false); 

    }

    IEnumerator BecomeVulnerable(float flashDuration)
    {

        isInvulnerable = false;
        animator.SetTrigger("isStunned");
        yield return new WaitForSeconds(flashDuration);
        animator.SetTrigger("isStunned");
        isInvulnerable = true;

    }

    public override void EnemyHit(float _damageDone)
    {
        Debug.Log("ELE ESTÁ" + isInvulnerable);
        if (!isInvulnerable)
        {
            base.EnemyHit(_damageDone);
        }


    }
}
