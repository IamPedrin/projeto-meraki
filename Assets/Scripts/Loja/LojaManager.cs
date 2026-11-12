using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Linq;
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
    public GameObject painelNomeHUD;
    public float tempoEntreItens = 0.3f;
    public float duracaoVoo = 0.8f;

    [Header("Posições do Pop-up de Topo")]
    public float posicaoYForaDaTela = 300f;
    public float posicaoYNaTela = -100f;

    private AlimentoSO _alimentoNoPopupAtual;
    private int _valorTotalCarrinho = 0;
    private List<AlimentoSO> _itensNoCarrinho = new List<AlimentoSO>();
    public static Sprite SpritePlacaPreco;

    private void Start()
    {
        // Garante que o popup comece invisível
        if (painelPopup != null) painelPopup.SetActive(false);

        ConfigurarVisualLojaETopo();
        GerarPrateleirasDinamicamente();
        AtualizarUICarrinho();
    }

    public void ConfigurarVisualLojaETopo()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();

        RectTransform safeAreaHud = canvas != null
            ? canvas.transform.Find("PanelPrincipal/SafeAreaHUD") as RectTransform
            : null;

        if (safeAreaHud != null && painelNomeHUD == null)
        {
            Transform pn = safeAreaHud.Find("PanelNome");
            if (pn != null) painelNomeHUD = pn.gameObject;
        }

        // 1. Alinhamento perfeito do drop "Alimentos enviados para a despensa"
        if (painelPopupTopo != null)
        {
            RectTransform rtTopo = painelPopupTopo.GetComponent<RectTransform>();
            if (safeAreaHud != null && rtTopo.parent != safeAreaHud)
            {
                rtTopo.SetParent(safeAreaHud, false);
            }
            rtTopo.SetAsLastSibling();
            rtTopo.anchorMin = new Vector2(0.5f, 1f);
            rtTopo.anchorMax = new Vector2(0.5f, 1f);
            rtTopo.pivot = new Vector2(0.5f, 1f);
            rtTopo.sizeDelta = new Vector2(760f, 96f);
            rtTopo.localScale = Vector3.one;
            if (!painelPopupTopo.activeSelf)
            {
                rtTopo.anchoredPosition = new Vector2(0f, posicaoYForaDaTela);
            }

            Image imgTopo = rtTopo.GetComponent<Image>();
            if (imgTopo != null)
            {
                imgTopo.type = Image.Type.Sliced;
                imgTopo.pixelsPerUnitMultiplier = 0.25f;
                imgTopo.color = Color.white;
            }

            TextMeshProUGUI tmpTopo = rtTopo.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpTopo != null)
            {
                RectTransform trt = tmpTopo.rectTransform;
                trt.anchorMin = Vector2.zero;
                trt.anchorMax = Vector2.one;
                trt.pivot = new Vector2(0.5f, 0.5f);
                trt.anchoredPosition = Vector2.zero;
                trt.sizeDelta = new Vector2(-40f, -16f);
                tmpTopo.text = "Alimentos enviados para a despensa!";
                tmpTopo.fontSize = 36f;
                tmpTopo.enableAutoSizing = true;
                tmpTopo.fontSizeMin = 20f;
                tmpTopo.fontSizeMax = 36f;
                tmpTopo.fontStyle = FontStyles.Bold;
                tmpTopo.color = new Color(1f, 0.96f, 0.88f, 1f);
                tmpTopo.alignment = TextAlignmentOptions.Center;
            }

            posicaoYForaDaTela = 300f;
            posicaoYNaTela = -32f;
        }

        // 2. Ajuste da primeira parte da Loja (PanelLoja)
        if (painelLoja != null)
        {
            RectTransform rtLoja = painelLoja.GetComponent<RectTransform>();
            rtLoja.anchorMin = new Vector2(0.5f, 0.5f);
            rtLoja.anchorMax = new Vector2(0.5f, 0.5f);
            rtLoja.pivot = new Vector2(0.5f, 0.5f);
            rtLoja.anchoredPosition = new Vector2(0f, -10f);
            rtLoja.sizeDelta = new Vector2(1540f, 900f);
            rtLoja.localScale = Vector3.one;

            // Obtém os sprites 9-slice já utilizados nos outros painéis do Hub
            Sprite spCard = null, spHeader = null, spWood = null, spCream = null;
            if (canvas != null)
            {
                var coz = canvas.transform.Find("PanelCozinha");
                if (coz != null)
                {
                    var ic = coz.GetComponent<Image>();
                    if (ic != null) spCard = ic.sprite;
                    var ft = coz.Find("FaixaTituloCozinha");
                    if (ft != null && ft.GetComponent<Image>() != null) spHeader = ft.GetComponent<Image>().sprite;
                    var fd = coz.Find("FaixaDespensa");
                    if (fd != null && fd.GetComponent<Image>() != null) spWood = fd.GetComponent<Image>().sprite;
                    var mp = coz.Find("MolduraMesaPrato");
                    if (mp != null && mp.GetComponent<Image>() != null) spCream = mp.GetComponent<Image>().sprite;
                }
            }

            var rawImg = rtLoja.Find("RawImage");
            if (rawImg != null)
            {
                if (spWood == null && rawImg.GetComponent<Image>() != null)
                    spWood = rawImg.GetComponent<Image>().sprite;
                rawImg.gameObject.SetActive(false);
            }
            if (spWood != null) SpritePlacaPreco = spWood;

            Image imgLoja = rtLoja.GetComponent<Image>();
            if (imgLoja != null && spCard != null)
            {
                imgLoja.sprite = spCard;
                imgLoja.type = Image.Type.Sliced;
                imgLoja.pixelsPerUnitMultiplier = 0.25f;
                imgLoja.color = Color.white;
            }

            TMP_FontAsset fontPadrao = textoTotalMoedas != null ? textoTotalMoedas.font : null;

            RectTransform CriarOuAjustarFaixa(Transform pai, string nome, Vector2 anchor, Vector2 pos, Vector2 size, Sprite sprite, Color corFundo, string texto, float tamFonte, Color corTexto)
            {
                Transform t = pai.Find(nome);
                GameObject go = t != null ? t.gameObject : new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                if (t == null) go.transform.SetParent(pai, false);

                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchorMin = anchor;
                rt.anchorMax = anchor;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = pos;
                rt.sizeDelta = size;
                rt.localScale = Vector3.one;

                Image img = go.GetComponent<Image>();
                if (sprite != null) img.sprite = sprite;
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 0.25f;
                img.color = corFundo;
                img.raycastTarget = false;

                if (!string.IsNullOrEmpty(texto))
                {
                    Transform lt = go.transform.Find("Label");
                    GameObject lgo = lt != null ? lt.gameObject : new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                    if (lt == null) lgo.transform.SetParent(go.transform, false);

                    RectTransform lrt = lgo.GetComponent<RectTransform>();
                    lrt.anchorMin = Vector2.zero;
                    lrt.anchorMax = Vector2.one;
                    lrt.pivot = new Vector2(0.5f, 0.5f);
                    lrt.anchoredPosition = Vector2.zero;
                    lrt.sizeDelta = new Vector2(-24f, -8f);
                    lrt.localScale = Vector3.one;

                    TextMeshProUGUI tmp = lgo.GetComponent<TextMeshProUGUI>();
                    if (fontPadrao != null) tmp.font = fontPadrao;
                    tmp.text = texto;
                    tmp.fontSize = tamFonte;
                    tmp.enableAutoSizing = true;
                    tmp.fontSizeMin = 18f;
                    tmp.fontSizeMax = tamFonte;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.color = corTexto;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.raycastTarget = false;
                }
                return rt;
            }

            if (spHeader != null)
            {
                CriarOuAjustarFaixa(rtLoja, "FaixaTituloLoja", new Vector2(0.5f, 1f), new Vector2(0f, -62f), new Vector2(820f, 88f),
                    spHeader, Color.white, "MERCADO DE ALIMENTOS", 44f, new Color(1f, 0.96f, 0.88f, 1f));
            }

            var btnFechar = rtLoja.Find("ButtonFechar") as RectTransform;
            if (btnFechar != null)
            {
                btnFechar.anchorMin = new Vector2(1f, 1f);
                btnFechar.anchorMax = new Vector2(1f, 1f);
                btnFechar.pivot = new Vector2(0.5f, 0.5f);
                btnFechar.anchoredPosition = new Vector2(-68f, -62f);
                btnFechar.sizeDelta = new Vector2(86f, 86f);
            }

            var svAlimentos = rtLoja.Find("Scroll View Alimentos") as RectTransform;
            if (svAlimentos != null)
            {
                svAlimentos.anchorMin = new Vector2(0.5f, 0.5f);
                svAlimentos.anchorMax = new Vector2(0.5f, 0.5f);
                svAlimentos.pivot = new Vector2(0.5f, 0.5f);
                svAlimentos.anchoredPosition = new Vector2(0f, 85f);
                svAlimentos.sizeDelta = new Vector2(1380f, 470f);
                if (spCream != null && svAlimentos.GetComponent<Image>() != null)
                {
                    var img = svAlimentos.GetComponent<Image>();
                    img.sprite = spCream;
                    img.type = Image.Type.Sliced;
                    img.pixelsPerUnitMultiplier = 0.25f;
                    img.color = Color.white;
                }

                var vp = svAlimentos.Find("Viewport") as RectTransform;
                if (vp != null)
                {
                    vp.anchorMin = Vector2.zero;
                    vp.anchorMax = Vector2.one;
                    vp.pivot = new Vector2(0.5f, 0.5f);
                    vp.anchoredPosition = Vector2.zero;
                    vp.sizeDelta = new Vector2(-36f, -36f);
                }

                var cont = svAlimentos.Find("Viewport/ContainerAlimentos") as RectTransform;
                if (cont != null)
                {
                    var glg = cont.GetComponent<GridLayoutGroup>();
                    if (glg != null)
                    {
                        glg.cellSize = new Vector2(165f, 165f);
                        glg.spacing = new Vector2(45f, 65f);
                        glg.padding = new RectOffset(35, 35, 25, 65);
                        glg.childAlignment = TextAnchor.UpperCenter;
                    }
                }
            }

            if (spWood != null)
            {
                CriarOuAjustarFaixa(rtLoja, "FaixaTituloCarrinho", new Vector2(0.5f, 0.5f), new Vector2(-240f, -185f), new Vector2(420f, 52f),
                    spWood, Color.white, "SEU CARRINHO", 26f, new Color(1f, 0.96f, 0.88f, 1f));
            }

            var svCarrinho = rtLoja.Find("Scroll View Carrinho") as RectTransform;
            if (svCarrinho != null)
            {
                svCarrinho.anchorMin = new Vector2(0.5f, 0.5f);
                svCarrinho.anchorMax = new Vector2(0.5f, 0.5f);
                svCarrinho.pivot = new Vector2(0.5f, 0.5f);
                svCarrinho.anchoredPosition = new Vector2(-240f, -295f);
                svCarrinho.sizeDelta = new Vector2(900f, 165f);
                if (spCream != null && svCarrinho.GetComponent<Image>() != null)
                {
                    var img = svCarrinho.GetComponent<Image>();
                    img.sprite = spCream;
                    img.type = Image.Type.Sliced;
                    img.pixelsPerUnitMultiplier = 0.25f;
                    img.color = Color.white;
                }

                var vp = svCarrinho.Find("Viewport") as RectTransform;
                if (vp != null)
                {
                    vp.anchorMin = Vector2.zero;
                    vp.anchorMax = Vector2.one;
                    vp.pivot = new Vector2(0.5f, 0.5f);
                    vp.anchoredPosition = Vector2.zero;
                    vp.sizeDelta = new Vector2(-28f, -24f);
                }

                var contCar = svCarrinho.Find("Viewport/ContainerCarrinho") as RectTransform;
                if (contCar != null)
                {
                    var hlg = contCar.GetComponent<HorizontalLayoutGroup>();
                    if (hlg != null)
                    {
                        hlg.spacing = 18f;
                        hlg.padding = new RectOffset(18, 18, 12, 12);
                        hlg.childAlignment = TextAnchor.MiddleLeft;
                        hlg.childForceExpandWidth = false;
                        hlg.childForceExpandHeight = false;
                        hlg.childControlWidth = true;
                        hlg.childControlHeight = true;
                    }
                }
            }

            if (spWood != null)
            {
                var molduraTotal = CriarOuAjustarFaixa(rtLoja, "MolduraTotalCompra", new Vector2(0.5f, 0.5f), new Vector2(465f, -210f), new Vector2(430f, 86f),
                    spWood, Color.white, "", 28f, Color.white);

                CriarOuAjustarFaixa(molduraTotal, "LabelTotal", new Vector2(0f, 0.5f), new Vector2(95f, 0f), new Vector2(150f, 50f),
                    spWood, new Color(1f, 1f, 1f, 0f), "TOTAL:", 28f, new Color(1f, 0.95f, 0.82f, 1f));
            }

            var imgCoinTotal = rtLoja.Find("Image") as RectTransform;
            if (imgCoinTotal != null)
            {
                imgCoinTotal.SetAsLastSibling();
                imgCoinTotal.anchorMin = new Vector2(0.5f, 0.5f);
                imgCoinTotal.anchorMax = new Vector2(0.5f, 0.5f);
                imgCoinTotal.pivot = new Vector2(0.5f, 0.5f);
                imgCoinTotal.anchoredPosition = new Vector2(430f, -210f);
                imgCoinTotal.sizeDelta = new Vector2(50f, 50f);
                var icImg = imgCoinTotal.GetComponent<Image>();
                if (icImg != null) icImg.preserveAspect = true;
            }

            var txtMoedasTotais = rtLoja.Find("TextMoedasTotais") as RectTransform;
            if (txtMoedasTotais != null)
            {
                txtMoedasTotais.SetAsLastSibling();
                txtMoedasTotais.anchorMin = new Vector2(0.5f, 0.5f);
                txtMoedasTotais.anchorMax = new Vector2(0.5f, 0.5f);
                txtMoedasTotais.pivot = new Vector2(0.5f, 0.5f);
                txtMoedasTotais.anchoredPosition = new Vector2(555f, -210f);
                txtMoedasTotais.sizeDelta = new Vector2(170f, 60f);
                var tmpTot = txtMoedasTotais.GetComponent<TextMeshProUGUI>();
                if (tmpTot != null)
                {
                    tmpTot.alignment = TextAlignmentOptions.Left;
                    tmpTot.fontStyle = FontStyles.Bold;
                    tmpTot.fontSize = 42f;
                    tmpTot.fontSizeMax = 42f;
                    tmpTot.color = new Color(1f, 0.92f, 0.45f, 1f);
                }
            }

            var btnFinalizar = rtLoja.Find("ButtonFinalizarCompra") as RectTransform;
            if (btnFinalizar != null)
            {
                btnFinalizar.SetAsLastSibling();
                btnFinalizar.anchorMin = new Vector2(0.5f, 0.5f);
                btnFinalizar.anchorMax = new Vector2(0.5f, 0.5f);
                btnFinalizar.pivot = new Vector2(0.5f, 0.5f);
                btnFinalizar.anchoredPosition = new Vector2(465f, -325f);
                btnFinalizar.sizeDelta = new Vector2(430f, 104f);
            }

            if (painelPopup != null)
            {
                painelPopup.transform.SetAsLastSibling();
            }
        }
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
            if (painelNomeHUD != null) painelNomeHUD.SetActive(false);

            painelPopupTopo.SetActive(true);
            RectTransform rectPopup = painelPopupTopo.GetComponent<RectTransform>();
            rectPopup.DOKill();
            rectPopup.anchoredPosition = new Vector2(0f, posicaoYForaDaTela);

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
                if (painelNomeHUD != null) painelNomeHUD.SetActive(true);
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
