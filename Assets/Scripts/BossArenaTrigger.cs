using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            MusicManager.Instance.PlayBossMusic();
        }
    }
}
