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

        Image imgAlimento = GetComponent<Image>();
        imgAlimento.sprite = alimento.iconeVisual;
        imgAlimento.preserveAspect = true;
        textoPreco.text = alimento.precoMoedas.ToString();

        // Cria ou ajusta a plaquinha de preço abaixo do alimento
        Transform badgeT = transform.Find("FaixaPrecoItem");
        if (badgeT == null && LojaManager.SpritePlacaPreco != null)
        {
            GameObject badgeGo = new GameObject("FaixaPrecoItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeGo.transform.SetParent(transform, false);
            badgeGo.transform.SetAsFirstSibling();
            RectTransform brt = badgeGo.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.5f, 0.5f);
            brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.pivot = new Vector2(0.5f, 0.5f);
            brt.anchoredPosition = new Vector2(0f, -104f);
            brt.sizeDelta = new Vector2(142f, 44f);
            Image bImg = badgeGo.GetComponent<Image>();
            bImg.sprite = LojaManager.SpritePlacaPreco;
            bImg.type = Image.Type.Sliced;
            bImg.pixelsPerUnitMultiplier = 0.25f;
            bImg.color = Color.white;
            bImg.raycastTarget = false;

            if (textoPreco != null)
            {
                textoPreco.color = new Color(1f, 0.95f, 0.8f, 1f);
            }
        }

        Button meuBotao = GetComponent<Button>();
        meuBotao.onClick.AddListener(ClicarNoBotao);
    }

    public void ClicarNoBotao()
    {
        _lojaManager.PrepararPopup(_meuAlimento);
    }
}
