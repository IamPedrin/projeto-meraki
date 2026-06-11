using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD do jogo")]
    public TextMeshProUGUI textoAlimentosColetados;
    private int _totalHistorico = 0;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI textoPontuacaoFinal;

    private void Awake()
    {
        Instance = this;
    }

    public void AdicionarPontoHUD()
    {
        _totalHistorico++;

        if (textoAlimentosColetados != null)
        {
            textoAlimentosColetados.text = "Coletados: " + _totalHistorico;
        }
    }

    public void MostrarGameOver(int _)
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        textoPontuacaoFinal.text = "Você coletou: " + _totalHistorico + " alimentos!";
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
