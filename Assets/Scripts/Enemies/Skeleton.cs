using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skeleton : Enemy
{
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private float footstepDelay = 0.4f; 

    private float footstepTimer;
    private bool playerOnArea = false;
    protected override void Start()
    {
        
        base.Start();
    }
 
    protected override void Update()
    {
        base.Update();

        if (active)
        {
            // Movimento
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);

            // Som de passos
            if (Mathf.Abs(PlayerController.Instance.transform.position.x - transform.position.x) > 0.1f)
            {
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepDelay)
                {
                    footstepAudio.Play();
                    footstepTimer = 0f;
                }
            }

            // Flip
            if (PlayerController.Instance.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            active = false;
        }
    }



    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
