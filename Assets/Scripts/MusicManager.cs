using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip normalMusicClip;
    public AudioClip bossMusicClip;

    [Range(0f, 1f)]
    public float musicVolume = 0.1f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém entre cenas

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Toca a música normal assim que o jogo começa
        PlayNormalMusic();
    }

    public void PlayNormalMusic()
    {
        if (audioSource.clip != normalMusicClip)
        {
            audioSource.Stop();
            audioSource.clip = normalMusicClip;
            audioSource.Play();
        }
    }

    public void PlayBossMusic()
    {
        if (audioSource.clip != bossMusicClip)
        {
            audioSource.Stop();
            audioSource.clip = bossMusicClip;
            audioSource.Play();
        }
    }

    public void StopAllMusic()
    {
        audioSource.Stop();
    }
}
