using UnityEngine;

public class AreaMusicTrigger : MonoBehaviour
{
    public string areaName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.PlayAreaMusic(areaName);
        }
    }
}