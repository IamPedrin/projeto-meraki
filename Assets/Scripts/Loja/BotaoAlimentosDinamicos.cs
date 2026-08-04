using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotaoAlimentosDinamicos : MonoBehaviour
{
    [Header("Conexões do Prefab")]
    public TextMeshProUGUI textoPreco;

    private AlimentoSO _meuAlimento;
    private LojaManager _lojaManager;

    public void ConfigurarBotao(AlimentoSO alimento, LojaManager manager)
    {
        _meuAlimento = alimento;
        _lojaManager = manager;

        GetComponent<Image>().sprite = alimento.iconeVisual;
        textoPreco.text = alimento.precoMoedas.ToString();

        Button meuBotao = GetComponent<Button>();
        meuBotao.onClick.AddListener(ClicarNoBotao);
    }

    public void ClicarNoBotao()
    {
        _lojaManager.PrepararPopup(_meuAlimento);
    }
}
