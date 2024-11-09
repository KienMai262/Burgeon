using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickIcon : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.transform.parent.parent.parent.parent.parent.parent.GetComponent<SwitchIcon>().icon.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
    }
}
