using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float speed;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float damage;
    private MoneyManager moneyManager;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Verifica se o MoneyManager existe antes de atribuir
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
        if (health <= 0)
        {
            if (moneyManager != null)
            {
                moneyManager.UpdateMoney(200); // Adiciona dinheiro
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
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < 1f) // ajuste
            {
                Attack();
            }
        }
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}