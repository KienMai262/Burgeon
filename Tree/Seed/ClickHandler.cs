using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableUI : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        itemCode itemcode = transform.parent.parent.GetComponent<Seed>().code;
        transform.parent.parent.parent.parent.Find("SelecIcon").GetComponent<Seed>().code = itemcode;
        transform.parent.parent.parent.parent.Find("SelecIcon").Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = transform.parent.GetComponent<UnityEngine.UI.Image>().sprite;
        
    }
}