using UnityEngine;

public class FlyDemon : Enemy
{
    private Vector3 startPos;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    protected override void Update()
    {
        base.Update();
        FloatEffect();
    }

    void FloatEffect()
    {
        float floatY = Mathf.Sin(Time.time * 2f) * 1f;
        transform.position = startPos + new Vector3(0, floatY, 0);
    }

    public override void EnemyHit(float _damageDone)
    {
        base.EnemyHit(_damageDone);
    }
}
