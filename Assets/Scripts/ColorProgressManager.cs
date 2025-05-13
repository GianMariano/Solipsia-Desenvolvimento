using UnityEngine;
using UnityEngine.UI;

public class ColorProgressManager : MonoBehaviour
{
    public static ColorProgressManager Instance;

    [SerializeField] private Image screenTint;
    private int fragmentsCollected = 0;
    private readonly int totalFragments = 3;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateTint(); // inicializa o escurecimento
    }

    public void CollectFragment()
    {
        fragmentsCollected = Mathf.Clamp(fragmentsCollected + 1, 0, totalFragments);
        UpdateTint();
    }

    private void UpdateTint()
    {
        float alpha = Mathf.Lerp(0.8f, 0f, (float)fragmentsCollected / totalFragments);
        Color color = screenTint.color;
        color.a = alpha;
        screenTint.color = color;
    }
}
