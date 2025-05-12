using UnityEngine;

public class FirePillarDamageZone : MonoBehaviour
{
    public int damage = 2;

    private void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
    }
}

