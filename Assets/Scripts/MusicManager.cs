using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [System.Serializable]
    public class AreaMusic
    {
        public string areaName;
        public AudioClip musicClip;
    }

    [System.Serializable]
    public class BossMusic
    {
        public string bossName;
        public AudioClip musicClip;
    }

    [Header("Area Music")]
    public List<AreaMusic> areaMusics = new List<AreaMusic>();
    public AudioClip defaultAreaMusic;

    [Header("Boss Music")]
    public List<BossMusic> bossMusics = new List<BossMusic>();
    public AudioClip defaultBossMusic;

    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    private AudioSource audioSource;
    private string currentArea;
    private string currentBoss;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = musicVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAreaMusic(string areaName)
    {
        currentArea = areaName;
        AudioClip clipToPlay = defaultAreaMusic;

        foreach (AreaMusic area in areaMusics)
        {
            if (area.areaName == areaName)
            {
                clipToPlay = area.musicClip;
                break;
            }
        }

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    public void PlayBossMusic(string bossName)
    {
        currentBoss = bossName;
        AudioClip clipToPlay = defaultBossMusic;

        foreach (BossMusic boss in bossMusics)
        {
            if (boss.bossName == bossName)
            {
                clipToPlay = boss.musicClip;
                break;
            }
        }

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    public void ReturnToAreaMusic()
    {
        if (!string.IsNullOrEmpty(currentArea))
        {
            PlayAreaMusic(currentArea);
        }
        else
        {
            PlayDefaultMusic();
        }
    }

    public void PlayDefaultMusic()
    {
        if (defaultAreaMusic != null)
        {
            audioSource.clip = defaultAreaMusic;
            audioSource.Play();
        }
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