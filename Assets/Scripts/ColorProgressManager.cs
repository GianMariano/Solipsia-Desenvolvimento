using UnityEngine;

public class ColorProgressManager : MonoBehaviour
{
    public static ColorProgressManager Instance;

    private int collected = 0;
    private const int total = 3;

    private SpriteColorRestorer[] restorers;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        restorers = FindObjectsOfType<SpriteColorRestorer>();
    }

    public void CollectFragment()
    {
        collected++;
        float progress = Mathf.Clamp01((float)collected / total);

        foreach (var restorer in restorers)
        {
            restorer.SetColorProgress(progress);
        }
    }
}
