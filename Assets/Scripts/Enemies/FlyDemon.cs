using UnityEngine;

public class FlyDemon : Enemy
{
    private Vector3 startPos;
    public AudioClip flyingSoundClip;
    private AudioSource audioSource;

    public float soundRange = 10f; // Distância máxima para o som tocar
    private bool isSoundPlaying = false;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (flyingSoundClip != null && audioSource != null)
        {
            audioSource.clip = flyingSoundClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    protected override void Update()
    {
        base.Update();
        FloatEffect();
        HandleFlyingSound();
    }

    void FloatEffect()
    {
        float floatY = Mathf.Sin(Time.time * 2f) * 1f;
        transform.position = startPos + new Vector3(0, floatY, 0);
    }

    void HandleFlyingSound()
    {
        if (player == null || audioSource == null || flyingSoundClip == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= soundRange)
        {
            if (!isSoundPlaying)
            {
                audioSource.Play();
                isSoundPlaying = true;
            }
        }
        else
        {
            if (isSoundPlaying)
            {
                audioSource.Pause(); // Usa Pause() para continuar de onde parou ao se aproximar novamente
                isSoundPlaying = false;
            }
        }
    }

    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
