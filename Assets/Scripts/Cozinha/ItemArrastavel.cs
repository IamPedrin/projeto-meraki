using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Necessário para o TextMeshPro

public class ItemArrastavel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AlimentoSO alimentoData;
    [HideInInspector] public Transform despensaTransform;

    [Header("Interface do Stack")]
    public TextMeshProUGUI textoQuantidade;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Canvas _canvasPrincipal;

    public static List<string> itensEmTransito = new List<string>();

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _canvasPrincipal = GetComponentInParent<Canvas>();
    }

    public void AtualizarQuantidade(int qtd)
    {
        if (textoQuantidade != null)
        {
            if (qtd > 1)
            {
                textoQuantidade.text = "x" + qtd.ToString();
                textoQuantidade.gameObject.SetActive(true);
            }
            else
            {
                textoQuantidade.gameObject.SetActive(false);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Transform paiAntigo = transform.parent;
        SlotPrato slot = paiAntigo.GetComponent<SlotPrato>();

        if (slot != null)
        {
            slot.alimentoNesteSlot = null;
        }

        transform.SetParent(_canvasPrincipal.transform);
        transform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;

        if (textoQuantidade != null) textoQuantidade.gameObject.SetActive(false);

        if (alimentoData != null) itensEmTransito.Add(alimentoData.idUnico);
        if (CozinhaManager.Instancia != null) CozinhaManager.Instancia.AtualizarDespensa();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvasPrincipal.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (alimentoData != null) itensEmTransito.Remove(alimentoData.idUnico);

        if (transform.parent == _canvasPrincipal.transform)
        {
            Destroy(gameObject);
        }

        if (CozinhaManager.Instancia != null) CozinhaManager.Instancia.AtualizarDespensa();
    }
}