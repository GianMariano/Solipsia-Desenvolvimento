using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BadEndingController : MonoBehaviour
{
    public Image blackScreen;     // Imagem preta para o fade
    public Text introText;        // Texto de introdução
    public Image endingImage;     // Imagem final que será exibida

    public float textDelay = 2f;      // Tempo para mostrar o texto
    public float fadeDuration = 1f;   // Tempo do fade-in

    private void Start()
    {
        StartCoroutine(PlayBadEndingSequence());
    }

    private IEnumerator PlayBadEndingSequence()
    {
        // Começa com tela preta visível e imagem final oculta
        blackScreen.gameObject.SetActive(true);
        introText.gameObject.SetActive(true);

        // Espera alguns segundos com o texto
        yield return new WaitForSeconds(textDelay);

        introText.gameObject.SetActive(false);
        
        // Faz o fade-in da tela preta
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

    }
}
