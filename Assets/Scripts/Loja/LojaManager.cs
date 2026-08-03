using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LojaManager : MonoBehaviour
{
    [Header("Banco de Dados")]
    [Tooltip("Arraste todos os arquivos de alimentos (AlimentoSO) criados aqui")]
    public List<AlimentoSO> catalogoDeAlimentos;

    public GameObject PanelLoja;

    [Header("Prateleira (Geração Dinâmica)")]
    public Transform conteinerDaPrateleira; 
    public GameObject prefabBotaoAlimento; 

    [Header("Carrinho Visual")]
    public Transform conteinerDoCarrinho;  
    public GameObject prefabIconeCarrinho; 
    public TextMeshProUGUI textoTotalMoedas;

    private AlimentoSO _alimentoNoPopupAtual;
    private int _valorTotalCarrinho = 0;

    private List<AlimentoSO> _itensNoCarrinho = new List<AlimentoSO>();

    private void Start()
    {
        GerarPrateleirasDinamicamente();
        AtualizarUICarrinho();
    }

    public void AbrirLoja()
    {
        PanelLoja.SetActive(true);
    }

    public void FecharLoja()
    {
        PanelLoja.SetActive(false);
    }

    private void GerarPrateleirasDinamicamente()
    {
        foreach (Transform filho in conteinerDaPrateleira)
        {
            Destroy(filho.gameObject);
        }

        foreach (AlimentoSO alimento in catalogoDeAlimentos)
        {
            GameObject novoBotao = Instantiate(prefabBotaoAlimento, conteinerDaPrateleira);
            BotaoAlimentosDinamicos scriptDoBotao = novoBotao.GetComponent<BotaoAlimentosDinamicos>();

            scriptDoBotao.ConfigurarBotao(alimento, this);
        }
    }

    public void PrepararPopup(AlimentoSO alimentoClicado)
    {
        _alimentoNoPopupAtual = alimentoClicado;
        Debug.Log("Abrindo popup para: " + alimentoClicado.nomeAlimento);

    }

    public void AdicionarAoCarrinho()
    {
        if (_alimentoNoPopupAtual == null) return;

        _itensNoCarrinho.Add(_alimentoNoPopupAtual);
        _valorTotalCarrinho += _alimentoNoPopupAtual.precoMoedas;

        GameObject iconeVisual = Instantiate(prefabIconeCarrinho, conteinerDoCarrinho);
        iconeVisual.GetComponent<Image>().sprite = _alimentoNoPopupAtual.iconeVisual;

        AtualizarUICarrinho();
    }

    private void AtualizarUICarrinho()
    {
        textoTotalMoedas.text = _valorTotalCarrinho.ToString();
    }

    public void BotaoComprar()
    {
        if (_itensNoCarrinho.Count == 0) return;

        if (BancoMoedas.GastarMoedas(_valorTotalCarrinho))
        {
            Debug.Log("Compra Aprovada! Itens indo para a cozinha...");


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

        foreach (Transform filho in conteinerDoCarrinho)
        {
            Destroy(filho.gameObject);
        }

        AtualizarUICarrinho();
    }
}
