using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button continuarButton;
    public Button desistirButton;
    public PlayerController player;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameOverPanel.SetActive(false);
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

        gameOverPanel.SetActive(true);

        continuarButton.onClick.RemoveAllListeners();
        desistirButton.onClick.RemoveAllListeners();

        continuarButton.onClick.AddListener(() =>
        {
            gameOverPanel.SetActive(false);
            StartCoroutine(RespawnSequence());
        });

        desistirButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Desistencia");
        });
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(0.2f);
        player.RespawnPlayer();

        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        player.isDead = false;
        player.pState.invincible = false;
    }
}
