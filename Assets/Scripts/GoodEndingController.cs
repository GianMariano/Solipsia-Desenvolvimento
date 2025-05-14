using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class GoodEndingController : MonoBehaviour
{
    public Image blackScreen;
    public Text introText;
    public float textDisplayTime = 3f;
    public float fadeDuration = 2f;

    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;

    void Start()
    {
        StartCoroutine(PlayGoodEndingSequence());
    }

    IEnumerator PlayGoodEndingSequence()
    {
        // Começa com tela preta e texto ativado
        blackScreen.color = new Color(0, 0, 0, 1);
        introText.enabled = true;
        videoDisplay.enabled = false;

        yield return new WaitForSeconds(textDisplayTime);

        // Some com o texto
        introText.enabled = false;

        // Mostra o vídeo e inicia
        videoDisplay.enabled = true;
        videoPlayer.Play();

        // Faz fade out da tela preta
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackScreen.gameObject.SetActive(false);
    }
}
