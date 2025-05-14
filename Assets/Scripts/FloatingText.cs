using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float lifetime = 2f;
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    private Transform target;

    public void Initialize(Transform followTarget, string message)
    {
        target = followTarget;
        GetComponentInChildren<Text>().text = message;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity; 
        }
    }
}
