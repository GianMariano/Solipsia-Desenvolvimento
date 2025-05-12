using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WhiteFadeController : MonoBehaviour
{
    public static WhiteFadeController Instance;

    public Image whiteFadeImage;
    public float whiteFadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public IEnumerator WhiteFadeOut()
    {
        float t = 0f;
        while (t < whiteFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0, 1, t / whiteFadeDuration);
            whiteFadeImage.color = new Color(1, 1, 1, a);
            yield return null;
        }
    }

    public IEnumerator WhiteFadeIn()
    {
        float t = 0f;
        while (t < whiteFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, t / whiteFadeDuration);
            whiteFadeImage.color = new Color(1, 1, 1, a);
            yield return null;
        }
    }
}
