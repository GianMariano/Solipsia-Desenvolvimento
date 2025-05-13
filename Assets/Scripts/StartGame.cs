using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string nomeCenaDoJogo;

    public void IniciarJogo()
    {
        SceneManager.LoadScene(nomeCenaDoJogo);
    }
}
