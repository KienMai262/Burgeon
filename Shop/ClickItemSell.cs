using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickItemSell : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SlotSellOnline slot = transform.parent.parent.GetComponent<SlotSellOnline>();
        var tmp = transform.parent.parent.parent.parent.parent.parent.Find("Infomation");
        tmp.GetChild(0).GetComponent<TMP_Text>().text = slot.code.name;
        tmp.GetComponent<SlotSellOnline>().code = slot.code;
        tmp.GetComponent<SlotSellOnline>().quantity = slot.quantity;
    }
}
