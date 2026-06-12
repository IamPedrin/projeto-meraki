using UnityEngine;
using TMPro;

public class MostrarNomeJogador : MonoBehaviour
{
    [Header("Interface")]
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