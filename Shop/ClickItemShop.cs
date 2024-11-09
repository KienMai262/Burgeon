using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickItemShop : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ItemData itemData = transform.parent.parent.GetComponent<SlotShop>().code;
        var tmp = transform.parent.parent.parent.parent.parent.parent.Find("Infomation");
        tmp.GetChild(0).GetComponent<TMP_Text>().text = itemData.name;
        tmp.GetChild(1).GetComponent<TMP_Text>().text = "Price: " + itemData.price.ToString();
        tmp.GetComponent<SlotShop>().code = itemData;
    }
}
