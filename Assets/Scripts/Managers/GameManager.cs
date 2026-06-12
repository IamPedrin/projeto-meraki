using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD do jogo")]
    public TextMeshProUGUI textoHUDTotal;
    public TextMeshProUGUI textoHUDMoedas;
    private int _totalHistorico = 0;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI textoPontuacaoFinal;
    public TextMeshProUGUI textoMoedasGanhas;

    [Header("Economia")]
    public int valorPorAlimento = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AtualizarHUDMoedas();
    }

    public void AdicionarPontoHUD()
    {
        _totalHistorico++;

        if (textoHUDTotal != null)
        {
            textoHUDTotal.text = "Coletados: " + _totalHistorico;
        }
    }

    public void AtualizarHUDMoedas()
    {
        if (textoHUDMoedas != null)
        {

            textoHUDMoedas.text = "Moedas: " + BancoMoedas.ObterSaldo();
        }
    }

    public void MostrarGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        textoPontuacaoFinal.text = "Você coletou: " + _totalHistorico + " alimentos!";

        int lucroDaPartida = _totalHistorico * valorPorAlimento;
        if (lucroDaPartida > 0)
        {
            BancoMoedas.AdicionarMoedas(lucroDaPartida);
        }

        if (textoMoedasGanhas != null)
        {
            textoMoedasGanhas.text = "Bônus final: +" + lucroDaPartida + " moedas!";
        }
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
