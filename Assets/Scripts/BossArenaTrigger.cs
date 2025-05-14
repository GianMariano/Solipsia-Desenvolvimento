using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    public string bossName; // Nome do boss correspondente
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            MusicManager.Instance.PlayBossMusic(bossName);
        }
    }

    // Novo método para parar a música do boss
    public void StopBossMusic()
    {
        MusicManager.Instance.StopAllMusic();
    }
}
