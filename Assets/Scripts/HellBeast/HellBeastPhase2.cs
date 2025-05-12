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
    public bool IsPhase2Finished { get; private set; } = false;


    public GameObject hellBeastPhase1Object;

    int damage = 2;
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
        yield return MoveToPoint(leftEdge, 1f);
        yield return TeleportToPoint(midle, 1f);
        yield return MoveToPoint(leftMidle, 1.5f);
        yield return MoveToPoint(rightMidle, 1.5f);
        yield return MoveToPoint(leftEdge, 2f);
        yield return MoveToPoint(rightEdge, 2.5f);
        yield return TeleportToPoint(leftEdge, 0.5f);
        yield return MoveToPoint(rightEdge, 3f);
        animator.SetTrigger("backToNormal");


        yield return new WaitForSeconds(1f);

        hellBeastPhase1Object.SetActive(true);
        gameObject.SetActive(false);

        HellBeast hellBeastScript = hellBeastPhase1Object.GetComponent<HellBeast>();
        if (hellBeastScript != null)
        {
            hellBeastScript.StartCoroutine(hellBeastScript.BecomeVulnerable(5f));
        }

        animator.SetTrigger("backToNormal");
        IsPhase2Finished = true;
    }

    public IEnumerator MoveToPoint(Transform target, float speed)
    {
        targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
        moveSpeed = speed;
        isMoving = true;

        animator.SetTrigger("isAttacking");

        yield return new WaitUntil(() => !isMoving);
    }

    public IEnumerator TeleportToPoint(Transform point, float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = new Vector3(point.position.x, transform.position.y, transform.position.z);

        if (point.position.x < transform.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);
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
