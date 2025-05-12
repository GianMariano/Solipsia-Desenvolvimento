using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance;

    public Text gameOverText; // arraste no Inspector
    public PlayerController player; // arraste o Player no Inspector
    public float waitBeforeFadeIn = 0.5f;
    public float waitAfterText = 1.5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Garante que o texto comece invisível
        if (gameOverText != null)
            gameOverText.color = new Color(1, 1, 1, 0);
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Fade para escurecer a tela
        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        // Pequena pausa antes do texto
        yield return new WaitForSeconds(waitBeforeFadeIn);

        // Mostra o texto "Game Over"
        yield return StartCoroutine(FadeInText(gameOverText));

        // Espera com o texto visível
        yield return new WaitForSeconds(waitAfterText);

        // Esconde o texto
        yield return StartCoroutine(FadeOutText(gameOverText));

        // Respawna o jogador
        player.RespawnPlayer(); 

        // Volta com o Fade In
        yield return StartCoroutine(FadeManager.Instance.FadeIn());
    }

    private IEnumerator FadeInText(Text text)
    {
        float t = 0f;
        Color c = text.color;
        while (t < 1f)
        {
            t += Time.deltaTime;
            text.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t));
            yield return null;
        }
    }

    private IEnumerator FadeOutText(Text text)
    {
        float t = 0f;
        Color c = text.color;
        while (t < 1f)
        {
            t += Time.deltaTime;
            text.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }
    }
}
