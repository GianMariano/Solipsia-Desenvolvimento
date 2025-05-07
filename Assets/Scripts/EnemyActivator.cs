using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [SerializeField] private Skeleton enemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.Activate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.Deactivate();
        }
    }
}
