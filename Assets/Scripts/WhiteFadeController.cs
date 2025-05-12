using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WhiteFadeController : MonoBehaviour
{
    public static WhiteFadeController Instance;

    [Header("Fade Settings")]
    [SerializeField] private Image whiteFadeImage;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fullWhiteDuration = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        if (whiteFadeImage != null)
        {
            whiteFadeImage.color = new Color(1, 1, 1, 0);
            whiteFadeImage.raycastTarget = false;
        }
    }

    public IEnumerator StartWhiteFadeEffect(System.Action onFullWhite = null)
    {
        if (whiteFadeImage == null) yield break;

        // Fade Out (transparente -> branco)
        yield return Fade(0f, 1f, fadeOutDuration);

        // Tela totalmente branca - momento para executar o teleporte
        onFullWhite?.Invoke();
        yield return new WaitForSeconds(fullWhiteDuration);

        // Fade In (branco -> transparente)
        yield return Fade(1f, 0f, fadeInDuration);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = new Color(1, 1, 1, startAlpha);
        Color endColor = new Color(1, 1, 1, endAlpha);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            whiteFadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        whiteFadeImage.color = endColor;
    }
}