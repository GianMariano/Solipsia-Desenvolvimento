using UnityEngine;
using System.Collections;
using TMPro;

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
    private bool isPhase2CurrentlyActive = false;

    public GameObject phase2Object;
    private bool isTakingDamageCooldown = false;

    // Diálogo
    [Header("Dialogue Settings")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float timeBetweenLines = 2f;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private bool isInDialogue = false;
    private int currentLine = 0;
    private float dialogueTimer = 0f;
    private bool battleStarted = false;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        originalHealth = health;

        if (healPrefab != null)
            healPrefab.SetActive(false);

        // Inicia diálogo se houver falas, senão começa a batalha direto
        if (dialogueLines != null && dialogueLines.Length > 0)
        {
            StartDialogue();
        }
        else
        {
            StartBattle();
        }
    }

    protected override void Update()
    {
        UpdateDialogue();
    }

    void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
        isTakingDamageCooldown = false;
    }

    // ------------------ DIÁLOGO --------------------

    private void StartDialogue()
    {
        isInDialogue = true;
        currentLine = 0;
        dialogueTimer = timeBetweenLines;

        if (dialogueBox != null) dialogueBox.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (dialogueText != null && currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
    }

    private void UpdateDialogue()
    {
        if (!isInDialogue) return;

        dialogueTimer -= Time.deltaTime;

        if (dialogueTimer <= 0)
        {
            currentLine++;

            if (currentLine >= dialogueLines.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowCurrentLine();
                dialogueTimer = timeBetweenLines;
            }
        }
    }

    private void EndDialogue()
    {
        isInDialogue = false;
        if (dialogueBox != null) dialogueBox.SetActive(false);
        StartBattle();
    }

    private void StartBattle()
    {
        battleStarted = true;

        if (phase2Object == null)
        {
            phase1Routine = StartCoroutine(AttackCycle1());
        }
        else
        {
            phase1Routine = StartCoroutine(AttackCycle1());
        }
    }

    // ------------------ ATAQUES --------------------

    IEnumerator AttackCycle1()
    {
        while (true)
        {
            if (!battleStarted) yield break;

            if (phase2Object != null && health <= originalHealth / 2 && !isPhase2CurrentlyActive)
            {
                StartCoroutine(SwitchToPhase2());
                yield break;
            }

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
            if (fireball != null)
            {
                fireball.Launch(direction, speed);
            }
            yield return new WaitForSeconds(cooldownBetweenShots);
        }
    }

    public IEnumerator BecomeVulnerable(float flashDuration)
    {
        isInvulnerable = false;
        isTakingDamageCooldown = false;
        animator.SetTrigger("isStunned");

        if (healPrefab != null)
            healPrefab.SetActive(true);

        yield return new WaitForSeconds(flashDuration);

        animator.ResetTrigger("isStunned");
        animator.SetTrigger("isIdle");
        isInvulnerable = true;
        if (healPrefab != null)
            healPrefab.SetActive(false);

        if (phase2Object != null && health <= originalHealth / 2)
        {
            if (!phase2Object.activeSelf)
            {
                isPhase2CurrentlyActive = true;
                phase2Object.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    public void MakeVulnerableNow(float duration)
    {
        isTakingDamageCooldown = false;
        if (gameObject.activeSelf)
        {
            StartCoroutine(BecomeVulnerable(duration));
        }
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

        if (health <= 0)
        {
            isInvulnerable = true;

            GameObject[] especiais = GameObject.FindGameObjectsWithTag("Special");
            foreach (GameObject obj in especiais)
            {
                Destroy(obj);
            }

            Destroy(gameObject);
            yield break;
        }

        if (phase2Object != null && health <= originalHealth / 2 && !isPhase2CurrentlyActive)
        {
            StartCoroutine(SwitchToPhase2());
        }

        yield return new WaitForSeconds(1f); 
        isTakingDamageCooldown = false;
    }

    private IEnumerator SwitchToPhase2()
    {
        isPhase2CurrentlyActive = true;

        if (phase1Routine != null)
        {
            StopCoroutine(phase1Routine);
            phase1Routine = null;
        }

        HellBeastPhase2 phase2Script = phase2Object.GetComponent<HellBeastPhase2>();
        if (phase2Script != null)
        {
            phase2Script.hellBeastScript = this;
        }
        else
        {
            isPhase2CurrentlyActive = false;
            yield break;
        }

        phase2Object.SetActive(true);
        gameObject.SetActive(false);

        yield return null;
    }
}
