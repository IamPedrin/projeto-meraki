using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI textoPontuacaoFinal;

    private void Awake()
    {
        Instance = this;
    }

    public void MostrarGameOver(int totalAlimentos)
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        textoPontuacaoFinal.text = "Você coletou: " + totalAlimentos + " alimentos!";
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
