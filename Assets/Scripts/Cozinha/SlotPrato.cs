using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPrato : MonoBehaviour, IDropHandler
{
    [Header("Status do Slot")]
    public AlimentoSO alimentoNesteSlot;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ItemArrastavel itemQueChegou = eventData.pointerDrag.GetComponent<ItemArrastavel>();

            if (itemQueChegou != null)
            {
                if (transform.childCount == 0)
                {
                    itemQueChegou.transform.SetParent(transform, false);
                    RectTransform rtSlot = transform as RectTransform;
                    RectTransform rtItem = itemQueChegou.GetComponent<RectTransform>();
                    if (rtItem != null)
                    {
                        rtItem.anchorMin = new Vector2(0.5f, 0.5f);
                        rtItem.anchorMax = new Vector2(0.5f, 0.5f);
                        rtItem.pivot = new Vector2(0.5f, 0.5f);
                        rtItem.anchoredPosition = Vector2.zero;
                        rtItem.localScale = Vector3.one;
                        if (rtSlot != null)
                        {
                            rtItem.sizeDelta = rtSlot.sizeDelta * 0.9f;
                        }
                    }
                    else
                    {
                        itemQueChegou.transform.localPosition = Vector3.zero;
                    }

                    alimentoNesteSlot = itemQueChegou.alimentoData;
                }
            }
        }
    }

    public void EsvaziarSlot()
    {
        alimentoNesteSlot = null;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
