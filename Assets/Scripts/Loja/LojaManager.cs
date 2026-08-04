using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LojaManager : MonoBehaviour
{
    [Header("Banco de Dados")]
    public List<AlimentoSO> catalogoDeAlimentos;

    [Header("Configurações de UI")]
    public GameObject painelLoja;

    [Header("Prateleira (Geração Dinâmica)")]
    public Transform conteinerDaPrateleira;
    public GameObject prefabBotaoAlimento;

    [Header("Carrinho Visual")]
    public Transform conteinerDoCarrinho;
    public GameObject prefabIconeCarrinho;
    public TextMeshProUGUI textoTotalMoedas;

    [Header("Pop-up de Detalhes")]
    public GameObject painelPopup; // A tela inteira do popup (Fundo escuro + janela)
    public Image imagemPopup;
    public TextMeshProUGUI textoNomePopup;
    public TextMeshProUGUI textoDescricaoPopup;
    public TextMeshProUGUI textoCategoriaPopup;
    public TextMeshProUGUI textoPrecoPopup;

    private AlimentoSO _alimentoNoPopupAtual;
    private int _valorTotalCarrinho = 0;
    private List<AlimentoSO> _itensNoCarrinho = new List<AlimentoSO>();

    private void Start()
    {
        // Garante que o popup comece invisível
        if (painelPopup != null) painelPopup.SetActive(false);

        GerarPrateleirasDinamicamente();
        AtualizarUICarrinho();
    }

    public void AbrirLoja()
    {
        painelLoja.SetActive(true);
    }

    public void FecharLoja()
    {
        painelLoja.SetActive(false);
    }

    private void GerarPrateleirasDinamicamente()
    {
        foreach (Transform filho in conteinerDaPrateleira) Destroy(filho.gameObject);

        foreach (AlimentoSO alimento in catalogoDeAlimentos)
        {
            GameObject novoBotao = Instantiate(prefabBotaoAlimento, conteinerDaPrateleira);
            novoBotao.GetComponent<BotaoAlimentosDinamicos>().ConfigurarBotao(alimento, this);
        }
    }

    public void PrepararPopup(AlimentoSO alimentoClicado)
    {
        _alimentoNoPopupAtual = alimentoClicado;

        imagemPopup.sprite = alimentoClicado.iconeVisual;
        textoNomePopup.text = alimentoClicado.nomeAlimento;
        textoDescricaoPopup.text = alimentoClicado.descricao;
        textoPrecoPopup.text = alimentoClicado.precoMoedas.ToString();

        textoCategoriaPopup.text = FormatarCategoria(alimentoClicado.categoria);

        painelPopup.SetActive(true);
    }

    public void FecharPopup()
    {
        painelPopup.SetActive(false);
        _alimentoNoPopupAtual = null;
    }

    private string FormatarCategoria(CategoriaAlimento cat)
    {
        switch (cat)
        {
            case CategoriaAlimento.InNatura: return "In Natura";
            case CategoriaAlimento.MinimamenteProcessado: return "Minimamente Processado";
            case CategoriaAlimento.Processado: return "Processado";
            case CategoriaAlimento.Ultraprocessado: return "Ultraprocessado";
            default: return "";
        }
    }

    public void AdicionarAoCarrinho()
    {
        if (_alimentoNoPopupAtual == null) return;

        _itensNoCarrinho.Add(_alimentoNoPopupAtual);
        _valorTotalCarrinho += _alimentoNoPopupAtual.precoMoedas;

        GameObject iconeVisual = Instantiate(prefabIconeCarrinho, conteinerDoCarrinho);
        iconeVisual.GetComponent<Image>().sprite = _alimentoNoPopupAtual.iconeVisual;

        AtualizarUICarrinho();

        FecharPopup();
    }

    private void AtualizarUICarrinho()
    {
        if (textoTotalMoedas != null) textoTotalMoedas.text = _valorTotalCarrinho.ToString();
    }

    public void BotaoComprar()
    {
        if (_itensNoCarrinho.Count == 0) return;

        if (BancoMoedas.GastarMoedas(_valorTotalCarrinho))
        {
            Debug.Log("Compra Aprovada! Itens indo para a despensa...");
            EsvaziarCarrinho();
        }
        else
        {
            Debug.Log("Moedas Insuficientes!");
        }
    }

    private void EsvaziarCarrinho()
    {
        _itensNoCarrinho.Clear();
        _valorTotalCarrinho = 0;
        foreach (Transform filho in conteinerDoCarrinho) Destroy(filho.gameObject);
        AtualizarUICarrinho();
    }
}
