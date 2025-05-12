using UnityEngine;
using System.Collections;

public class HellBeastPhase2 : MonoBehaviour
{
    private Animator animator;

    public Transform leftEdge;
    public Transform leftMidle;
    public Transform midle;
    public Transform rightEdge;
    public Transform rightMidle;

    public HellBeast hellBeastScript;

    private Vector3 targetPosition;
    private float moveSpeed = 0f;
    private bool isMoving = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return PlayChargeAnimation();

        yield return MoveToPoint(leftEdge, 1f);
        yield return TeleportToPoint(midle, 1f);
        yield return MoveToPoint(leftMidle, 1.5f);
        yield return MoveToPoint(rightMidle, 1.5f);
        yield return MoveToPoint(leftEdge, 2f);
        yield return MoveToPoint(rightEdge, 2.5f);
        yield return TeleportToPoint(leftEdge, 0.5f);
        yield return MoveToPoint(rightEdge, 3f);

        if (hellBeastScript != null)
        {
            hellBeastScript.gameObject.SetActive(true);
            hellBeastScript.MakeVulnerableNow(5f);
        }

        gameObject.SetActive(false);
    }

    public IEnumerator MoveToPoint(Transform target, float speed)
    {
        targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
        moveSpeed = speed;
        isMoving = true;

        while (isMoving)
        {
            yield return PlayChargeAttackingAnimation();
        }
    }

    public IEnumerator TeleportToPoint(Transform point, float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = new Vector3(point.position.x, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(point.position.x < transform.position.x ? -1f : 1f, 1f, 1f);

        yield return PlayChargeAnimation();
    }

    public IEnumerator PlayChargeAnimation()
    {
        animator.CrossFade("Charge", 0.1f);
        yield return WaitForAnimationToEnd("Charge");
    }

    public IEnumerator PlayChargeAttackingAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ChargeAttacking"))
        {
            animator.CrossFade("ChargeAttacking", 0.1f);
        }
        yield return WaitForAnimationToEnd("ChargeAttacking");
    }

    private IEnumerator WaitForAnimationToEnd(string stateName)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}
