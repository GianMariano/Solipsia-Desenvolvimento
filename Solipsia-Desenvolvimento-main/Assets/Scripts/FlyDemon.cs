using UnityEngine;

public class FlyDemon : MonoBehaviour
{
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        FloatEffect();
    }

    void FloatEffect()
    {
        float floatY = Mathf.Sin(Time.time * 2f) * 1f;
        transform.position = startPos + new Vector3(0, floatY, 0);
    }
}
