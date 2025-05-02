using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skeleton : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        
        base.Start();
    }

    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        // Flip na direção do jogador
        if (PlayerController.Instance.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
