using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float speed;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float damage;
    [SerializeField] protected int goldGiven;
    private MoneyManager moneyManager;
    protected bool active = false;


    protected virtual void Start()
    {

        GameObject moneyManagerObj = GameObject.Find("MoneyManager");
        if (moneyManagerObj != null)
        {
            moneyManager = moneyManagerObj.GetComponent<MoneyManager>();
        }
        else
        {
            Debug.LogError("GameObject 'MoneyManager' nao encontrado na cena!");
        }
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (PlayerController.Instance == null || PlayerController.Instance.isDead) 
        {
            return;
        }

        if (health <= 0)
        {
            if (moneyManager != null)
            {
                moneyManager.UpdateMoney(goldGiven); // Adiciona dinheiro
            }
            else
            {
                Debug.LogWarning("MoneyManager nao atribuido!");
            }
            Destroy(gameObject);
        }
    }

    public virtual void EnemyHit(float _damageDone)
    {
        health -= _damageDone;

    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible 
            && !PlayerController.Instance.isDead && PlayerController.Instance.Health > 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < 1f)
            {
                Attack();
            }
        }
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }


    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

}