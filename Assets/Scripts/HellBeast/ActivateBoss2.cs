using UnityEngine;

public class ActivateBoss2 : MonoBehaviour
{
    // Variável pública para arrastar o GameObject da Fase 1 do Boss no Inspector
    public GameObject bossPhase1GameObject;

    // Flag para garantir que o boss seja ativado apenas uma vez
    private bool bossActivated = false;

    // Start é chamado uma vez antes da primeira execução do Update
    void Start()
    {
        // É uma boa prática garantir que o GameObject do boss esteja desativado no início
        // para que ele só seja ativado pelo trigger.
        // Você pode fazer isso manualmente no Inspector ou via código aqui, se preferir.
        // Exemplo: if (bossPhase1GameObject != null) bossPhase1GameObject.SetActive(false);
        // No entanto, geralmente é melhor configurar o estado inicial no editor.
    }

    // OnTriggerEnter2D é chamado quando outro Collider2D entra no trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o boss ainda não foi ativado e se o objeto que entrou no trigger é o Player
        if (!bossActivated && other.CompareTag("Player"))
        {
            // Verifica se a referência ao GameObject do boss foi configurada no Inspector
            if (bossPhase1GameObject != null)
            {
                // Ativa o GameObject da Fase 1 do boss
                bossPhase1GameObject.SetActive(true);

                // Marca que o boss foi ativado para não ativar novamente
                bossActivated = true;

                // Opcional: Desativar este GameObject do trigger para que ele não possa ser acionado novamente
                // gameObject.SetActive(false);
                // Ou, se preferir, apenas desative o componente Collider2D:
                // GetComponent<Collider2D>().enabled = false;

                Debug.Log("Boss Fase 1 Ativado pelo Player!");
            }
            else
            {
                Debug.LogError("GameObject da Fase 1 do Boss não foi atribuído no Inspector do ActivateBoss2!");
            }
        }
    }
}

