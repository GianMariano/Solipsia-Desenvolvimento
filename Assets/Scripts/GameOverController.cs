using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Namespace para TextMeshPro

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance;

    public TextMeshProUGUI gameOverText; // Mudado para TextMeshProUGUI
    public PlayerController player;
    public float waitBeforeFadeIn = 0.5f;
    public float waitAfterText = 1.5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (gameOverText != null)
            gameOverText.color = new Color(1, 1, 1, 0);
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        player.isDead = true;
        player.pState.invincible = true;
        
        yield return StartCoroutine(FadeManager.Instance.FadeOut());
        yield return new WaitForSeconds(waitBeforeFadeIn);

        if (gameOverText != null)
        {
            yield return StartCoroutine(FadeInText(gameOverText));
            yield return new WaitForSeconds(waitAfterText);
            yield return StartCoroutine(FadeOutText(gameOverText));
        }

        player.RespawnPlayer(); 
        yield return StartCoroutine(FadeManager.Instance.FadeIn());
        
        player.isDead = false;
        player.pState.invincible = false;
    }

    // Métodos atualizados para usar TextMeshProUGUI
    private IEnumerator FadeInText(TextMeshProUGUI text)
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

    private IEnumerator FadeOutText(TextMeshProUGUI text)
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