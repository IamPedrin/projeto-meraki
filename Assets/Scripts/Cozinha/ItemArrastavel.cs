using UnityEngine;
using UnityEngine.EventSystems;

public class ItemArrastavel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AlimentoSO alimentoData;

    [HideInInspector] public Transform despensaTransform;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Canvas _canvasPrincipal;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _canvasPrincipal = GetComponentInParent<Canvas>();
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvasPrincipal.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (transform.parent == _canvasPrincipal.transform)
        {
            transform.SetParent(despensaTransform);
        }
    }
}
