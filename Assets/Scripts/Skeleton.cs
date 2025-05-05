using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skeleton : Enemy
{
    private bool playerOnArea = false;
    protected override void Start()
    {
        
        base.Start();
    }

    
    
    protected override void Update()
    {
        base.Update();

        if (playerOnArea)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);

            //Flip
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
            playerOnArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnArea = false;
        }
    }

    public void Activate()
    {
        playerOnArea = true;
    }

    public void Deactivate()
    {
        playerOnArea = false;
    }


    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
