using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip normalMusicClip;
    public AudioClip bossMusicClip;

    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayNormalMusic(); // Isso toca a música normal com volume aplicado
    }

    public void PlayNormalMusic()
    {
        audioSource.clip = normalMusicClip;
        audioSource.volume = musicVolume; // GARANTIR que o volume é aplicado
        audioSource.Play();
    }

    public void PlayBossMusic()
    {
        audioSource.clip = bossMusicClip;
        audioSource.volume = musicVolume;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public void StopAllMusic()
    {
        audioSource.Stop();
    }
}
