using UnityEngine;
using TMPro;

public class MostrarNomeJogador : MonoBehaviour
{
    [Header("Interface")]
    [Tooltip("Arraste o texto do Canvas onde o nome deve aparecer")]
    public TextMeshProUGUI textoDeBoasVindas;

    private void Start()
    {
        string nomeDaCrianca = PlayerPrefs.GetString("NomeCrianca", "Jogador");

        if (textoDeBoasVindas != null)
        {
            textoDeBoasVindas.text = "Olá, " + nomeDaCrianca + "!";
        }
    }
}