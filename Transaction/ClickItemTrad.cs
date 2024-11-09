using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickItemTrad : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SlotSellOnline slot = transform.parent.parent.GetComponent<SlotSellOnline>();
        var tmp = transform.parent.parent.parent.parent.parent.parent.parent.Find("Panel").GetChild(0);
        tmp.GetChild(0).GetComponent<TMP_Text>().text = slot.code.name;
        tmp.GetComponent<SlotSellOnline>().code = slot.code;
        tmp.GetComponent<SlotSellOnline>().quantity = slot.quantity;

        var tmp2 = transform.parent.parent.parent.parent.parent.parent.parent.GetComponent<LoadDataMe>();
        tmp2.quantityAndInfomation.SetActive(true);
        tmp.GetChild(2).GetComponent<TMP_InputField>().text = "";
        tmp2.quantityAndInfomation.transform.GetChild(0).Find("OK").GetComponent<Button>().onClick.RemoveAllListeners();
        tmp2.quantityAndInfomation.transform.GetChild(0).Find("Cancel").GetComponent<Button>().onClick.RemoveAllListeners();
        tmp2.quantityAndInfomation.transform.GetChild(0).Find("OK").GetComponent<Button>().onClick.AddListener(tmp2.ConfirmItemTrade);
        tmp2.quantityAndInfomation.transform.GetChild(0).Find("Cancel").GetComponent<Button>().onClick.AddListener(tmp2.CancelItemTrade);
    }
}
