using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataMe : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] protected GameObject slotPrefab;
    [SerializeField] public GameObject itemTradePrefab;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected RectTransform contentTrade;
    [SerializeField] public GameObject quantityAndInfomation;
    [SerializeField] public GameObject enterGoldUI;
    [SerializeField] public GameObject buttonOK;

    [SerializeField] public List<List<int>> slots = new List<List<int>>();
    [SerializeField] public int gold;

    public void LoadInventory()
    {
        gameObject.transform.Find("Inventory").gameObject.SetActive(true);
        gameObject.transform.Find("ListItemInventory").gameObject.SetActive(true);
        gameObject.transform.Find("ListItemSlots").gameObject.SetActive(false);
        quantityAndInfomation.SetActive(false);
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
        }
        var slots = player.GetComponent<Player>().inventory.slots;
        for (int i = 0; i < slots.Count; i++)
        {
            if (i <= content.childCount - 1)
            {
                if (slots[i].data.itemCode != itemCode.nullItem)
                {
                    content.GetChild(i).gameObject.SetActive(true);
                    content.GetChild(i).GetChild(0).GetComponent<Image>().sprite = slots[i].data.sprite;
                    content.GetChild(i).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slots[i].count.ToString();
                    content.GetChild(i).GetComponent<SlotSellOnline>().code = slots[i].data;
                    content.GetChild(i).GetComponent<SlotSellOnline>().quantity = slots[i].count;
                }
            }
            else
            {
                if (slots[i].data.itemCode != itemCode.nullItem)
                {
                    GameObject item = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
                    item.transform.SetParent(content, false);
                    item.transform.GetChild(0).GetComponent<Image>().sprite = slots[i].data.sprite;
                    item.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slots[i].count.ToString();
                    item.GetComponent<SlotSellOnline>().code = slots[i].data;
                    item.GetComponent<SlotSellOnline>().quantity = slots[i].count;
                }
            }
        }
    }

    public void EnterGold()
    {
        enterGoldUI.SetActive(true);
        enterGoldUI.transform.GetChild(0).Find("OK").GetComponent<Button>().onClick.RemoveAllListeners();
        enterGoldUI.transform.GetChild(0).Find("OK").GetComponent<Button>().onClick.AddListener(ConfirmGold);
    }

    public void ConfirmGold()
    {
        var tmp = enterGoldUI.transform.GetChild(0).transform.Find("Gold");
        string inputGold = tmp.GetComponent<TMP_InputField>().text;
        tmp.GetComponent<TMP_InputField>().text = "";
        int value;
        if (int.TryParse(inputGold, out value))
        {
            if (gold <= player.GetComponent<Player>().inventory.gold)
            {
                gold = value;
                enterGoldUI.SetActive(false);
                EnterMyGold();
                LoadItemTrade();
                gameObject.transform.Find("Inventory").gameObject.SetActive(false);
                TradeManager.instance.AcceptTrade();
                buttonOK.GetComponent<Button>().onClick.RemoveAllListeners();
                buttonOK.GetComponent<Button>().onClick.AddListener(TradeManager.instance.EnterTrade);
            }
        }
        else
        {
            Debug.Log("Invalid input");
        }
    }

    public void EnterMyGold()
    {
        gameObject.transform.Find("ListItemSlots").Find("Gold").Find("Gold").GetComponent<TMP_Text>().text = gold.ToString();
    }
    public void LoadItemTrade()
    {
        gameObject.transform.Find("ListItemInventory").gameObject.SetActive(false);
        gameObject.transform.Find("ListItemSlots").gameObject.SetActive(true);
        foreach (Transform child in contentTrade)
        {
            child.gameObject.SetActive(false);
        }
        Debug.Log(slots.Count);
        for (int i = 0; i < slots.Count; i++)
        {
            if (i <= contentTrade.childCount - 1)
            {
                contentTrade.GetChild(i).gameObject.SetActive(true);
                contentTrade.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ReadWriteData.instance.itemDataList[slots[i][0]].sprite;
                contentTrade.GetChild(i).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slots[i][1].ToString();
            }
            else
            {
                GameObject item = Instantiate(itemTradePrefab, Vector3.zero, Quaternion.identity);
                item.transform.SetParent(contentTrade, false);
                item.transform.GetChild(0).GetComponent<Image>().sprite = ReadWriteData.instance.itemDataList[slots[i][0]].sprite;
                item.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slots[i][1].ToString();
            }
        }
    }

    public void ConfirmItemTrade()
    {
        var tmp = quantityAndInfomation.transform.GetChild(0).transform.Find("Quantity");
        string inputQuantity = tmp.GetComponent<TMP_InputField>().text;
        tmp.GetComponent<TMP_InputField>().text = "";
        SlotSellOnline slot = new SlotSellOnline();
        int value;
        int qis = 0;
        foreach (var innerSlot in slots)
        {
            if (innerSlot[0] == ReadWriteData.instance.itemDataList.IndexOf(quantityAndInfomation.transform.GetChild(0).GetComponent<SlotSellOnline>().code))
            {
                qis = innerSlot[1];
                break;
            }
        }
        if (int.TryParse(inputQuantity, out value))
        {
            if ((value + qis) <= quantityAndInfomation.transform.GetChild(0).GetComponent<SlotSellOnline>().quantity)
            {
                slot.code = quantityAndInfomation.transform.GetChild(0).GetComponent<SlotSellOnline>().code;
                slot.quantity = value;
                foreach (var itemData in ReadWriteData.instance.itemDataList)
                {
                    int kt = 0;
                    int index = ReadWriteData.instance.itemDataList.IndexOf(itemData);
                    if (itemData.itemCode == slot.code.itemCode)
                    {
                        Debug.Log("index: " + index);
                        foreach (var innerSlot in slots)
                        {
                            if (innerSlot[0] == index)
                            {
                                kt = 1;
                                break;
                            }
                        }
                        if (kt == 0)
                        {
                            slots.Add(new List<int> { index, slot.quantity });
                        }
                        else
                        {
                            for (int i = 0; i < slots.Count; i++)
                            {
                                if (slots[i][0] == index)
                                {
                                    slots[i][1] += slot.quantity;
                                    break;
                                }
                            }
                        }
                    }
                }
                quantityAndInfomation.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Invalid input");
        }
    }
    public void CancelItemTrade()
    {
        quantityAndInfomation.transform.GetChild(0).transform.Find("Quantity").GetComponent<TMP_InputField>().text = "";
        quantityAndInfomation.SetActive(false);
    }


}
