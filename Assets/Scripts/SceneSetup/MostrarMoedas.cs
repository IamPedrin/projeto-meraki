using TMPro;
using UnityEngine;

public class MostrarMoedas : MonoBehaviour
{
    [Header("Interface")]
    public TextMeshProUGUI moedas;

    private void Start()
    {
        int moedasDisponiveis = PlayerPrefs.GetInt("MoedasTotais", 0);

        if (moedas != null)
        {
            moedas.text = moedasDisponiveis.ToString();
        }
    }
}
