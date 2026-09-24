using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
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
    public GameObject painelPopup;
    public Image imagemPopup;
    public TextMeshProUGUI textoNomePopup;
    public TextMeshProUGUI textoDescricaoPopup;
    public TextMeshProUGUI textoCategoriaPopup;
    public TextMeshProUGUI textoPrecoPopup;
    public TextMeshProUGUI textoCalorias;

    [Header("Feedbacks Visuais")]
    public TextMeshProUGUI textoAvisoCompra;

    [Header("Animação DoTween (Pós-Compra)")]
    public RectTransform iconeCozinhaHUD;
    public Transform canvasAnimacao;
    public GameObject prefabItemVoador;
    public GameObject painelPopupTopo;
    public float tempoEntreItens = 0.3f;
    public float duracaoVoo = 0.8f;

    [Header("Posições do Pop-up de Topo")]
    public float posicaoYForaDaTela = 300f;
    public float posicaoYNaTela = -100f;

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
        if (textoCalorias != null)
        {
            textoCalorias.text = alimentoClicado.calorias.ToString("F0") + " kcal";
        }

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
            List<AlimentoSO> itensComprados = new List<AlimentoSO>(_itensNoCarrinho);

            foreach (AlimentoSO item in _itensNoCarrinho)
            {
                InventarioManager.AdicionarAlimento(item.idUnico, 1);
            }

            EsvaziarCarrinho();

            MostrarMoedas scriptMoedas = FindAnyObjectByType<MostrarMoedas>();
            if (scriptMoedas != null) scriptMoedas.AtualizarTela();

            painelLoja.SetActive(false);
            if (painelPopup != null) painelPopup.SetActive(false);
            EsconderAviso();

            StartCoroutine(AnimarItensParaCozinha(itensComprados));
        }
        else
        {
            MostrarAviso("Moedas Insuficientes!", Color.red);
        }
    }

    private IEnumerator AnimarItensParaCozinha(List<AlimentoSO> itens)
    {
        if (painelPopupTopo != null)
        {
            painelPopupTopo.SetActive(true);
            RectTransform rectPopup = painelPopupTopo.GetComponent<RectTransform>();

            rectPopup.anchoredPosition = new Vector2(rectPopup.anchoredPosition.x, posicaoYForaDaTela);

            rectPopup.DOAnchorPosY(posicaoYNaTela, 0.6f).SetEase(Ease.OutBack);
        }

        yield return new WaitForSeconds(0.4f);

        foreach (AlimentoSO item in itens)
        {
            GameObject voador = Instantiate(prefabItemVoador, canvasAnimacao);
            RectTransform rect = voador.GetComponent<RectTransform>();
            Image img = voador.GetComponent<Image>();

            img.sprite = item.iconeVisual;

            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.zero;

            rect.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.4f);

            rect.DOMove(iconeCozinhaHUD.position, duracaoVoo).SetEase(Ease.InOutQuad);
            rect.DOScale(Vector3.zero, duracaoVoo).SetEase(Ease.InBack).OnComplete(() =>
            {
                iconeCozinhaHUD.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
                Destroy(voador);
            });

            yield return new WaitForSeconds(tempoEntreItens);
        }

        yield return new WaitForSeconds(duracaoVoo + 0.5f);

        if (painelPopupTopo != null)
        {
            RectTransform rectPopup = painelPopupTopo.GetComponent<RectTransform>();

            rectPopup.DOAnchorPosY(posicaoYForaDaTela, 0.6f).SetEase(Ease.InBack).OnComplete(() =>
            {
                painelPopupTopo.SetActive(false);
            });
        }
    }

    private void MostrarAviso(string mensagem, Color cor)
    {
        if (textoAvisoCompra != null)
        {
            textoAvisoCompra.gameObject.SetActive(true);
            textoAvisoCompra.text = mensagem;
            textoAvisoCompra.color = cor;

            CancelInvoke("EsconderAviso");
            Invoke("EsconderAviso", 3f);
        }
    }

    private void EsconderAviso()
    {
        if (textoAvisoCompra != null) textoAvisoCompra.text = "";
        textoAvisoCompra.gameObject.SetActive(false);
    }

    private void EsvaziarCarrinho()
    {
        _itensNoCarrinho.Clear();
        _valorTotalCarrinho = 0;
        foreach (Transform filho in conteinerDoCarrinho) Destroy(filho.gameObject);
        AtualizarUICarrinho();
    }

    private void FecharLojaTotalmente()
    {
        if (painelLoja != null)
        {
            painelLoja.SetActive(false);
        }
        EsconderAviso();
    }

}
