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
    public Transform conteinerDaPrateleira; // Onde os botões vão nascer
    public GameObject prefabBotaoAlimento;  // O botão vazio que serve de molde

    [Header("Carrinho Visual")]
    public Transform conteinerDoCarrinho;   // Onde os sprites das frutas vão nascer
    public GameObject prefabIconeCarrinho;  // Apenas um molde de Image vazia
    public TextMeshProUGUI textoTotalMoedas;

    private AlimentoSO _alimentoNoPopupAtual;
    private int _valorTotalCarrinho = 0;

    // Lista para sabermos exatamente o que a criança comprou no final
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
        // 1. Limpa a prateleira por segurança (caso tenha botões de teste)
        foreach (Transform filho in conteinerDaPrateleira)
        {
            Destroy(filho.gameObject);
        }

        // 2. Cria um botão para cada alimento que você cadastrou no catálogo!
        foreach (AlimentoSO alimento in catalogoDeAlimentos)
        {
            GameObject novoBotao = Instantiate(prefabBotaoAlimento, conteinerDaPrateleira);
            BotaoAlimentosDinamicos scriptDoBotao = novoBotao.GetComponent<BotaoAlimentosDinamicos>();

            scriptDoBotao.ConfigurarBotao(alimento, this);
        }
    }

    // Deixei essa função preparada para quando formos fazer o Popup!
    public void PrepararPopup(AlimentoSO alimentoClicado)
    {
        _alimentoNoPopupAtual = alimentoClicado;
        Debug.Log("Abrindo popup para: " + alimentoClicado.nomeAlimento);

        // TODO: Ativar o painel do Popup e preencher os textos (Próxima etapa!)
    }

    // O Botão de "Adicionar" dentro do Popup vai chamar esta função
    public void AdicionarAoCarrinho()
    {
        if (_alimentoNoPopupAtual == null) return;

        // 1. Salva na lista de compras
        _itensNoCarrinho.Add(_alimentoNoPopupAtual);
        _valorTotalCarrinho += _alimentoNoPopupAtual.precoMoedas;

        // 2. Cria o Sprite do alimento fisicamente lá na barra de baixo!
        GameObject iconeVisual = Instantiate(prefabIconeCarrinho, conteinerDoCarrinho);
        iconeVisual.GetComponent<Image>().sprite = _alimentoNoPopupAtual.iconeVisual;

        AtualizarUICarrinho();
    }

    private void AtualizarUICarrinho()
    {
        textoTotalMoedas.text = _valorTotalCarrinho.ToString();
    }

    // O Botão de "Joinha" (Thumbs Up) chama esta função
    public void BotaoComprar()
    {
        if (_itensNoCarrinho.Count == 0) return;

        if (BancoMoedas.GastarMoedas(_valorTotalCarrinho))
        {
            Debug.Log("Compra Aprovada! Itens indo para a cozinha...");

            // Aqui futuramente enviaremos para o PlayerPrefs da cozinha

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

        // Destrói as imagens da barra de baixo
        foreach (Transform filho in conteinerDoCarrinho)
        {
            Destroy(filho.gameObject);
        }

        AtualizarUICarrinho();
    }
}
