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
                    itemQueChegou.transform.SetParent(transform);
                    itemQueChegou.transform.localPosition = Vector3.zero;

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
