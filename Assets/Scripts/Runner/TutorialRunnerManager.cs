using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialRunnerManager : MonoBehaviour
{
    [Header("Elementos do Ecrã")]
    public GameObject painelTutorial;
    public Image imagemObjetivo;
    public TextMeshProUGUI textoObjetivo;

    [Header("Interface do Jogo (HUD)")]
    public GameObject hudDoJogo;

    private void Start()
    {
        PrepararTelaDeInicio();
    }

    public void PrepararTelaDeInicio()
    {
        if (hudDoJogo != null) hudDoJogo.SetActive(false);

        painelTutorial.SetActive(true);

        if (GameManager.Instance != null && GameManager.Instance.alimentoObjetivo != null)
        {
            imagemObjetivo.sprite = GameManager.Instance.alimentoObjetivo.iconeVisual;
            textoObjetivo.text = "Objetivo da Fase:\nColete " + GameManager.Instance.alimentoObjetivo.nomeAlimento + "!";
        }

        Time.timeScale = 0f;
    }

    public void BotaoComecarCorrida()
    {
        painelTutorial.SetActive(false);

        if (hudDoJogo != null) hudDoJogo.SetActive(true);

        Time.timeScale = 1f;
    }
}