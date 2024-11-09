using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] protected GameObject shopPanel;
    [SerializeField] protected SlotShop itemPrefab;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected GameObject infomationItem;
    [SerializeField] protected TMP_Text pageText;
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text priceText;
    [SerializeField] protected GameObject player;
    [SerializeField] protected TMP_Text textGold;
    [SerializeField] protected GameObject buttonBuy;
    [SerializeField] protected GameObject buttonSell;

    public List<ItemData> slots = new List<ItemData>();

    public itemCode code;
    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static ShopUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    //

    private void Start()
    {
        shopPanel.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ToggleSlot();
            textGold.text = player.GetComponent<Player>().inventory.gold.ToString();
        }
    }
    public void ToggleSlot()
    {
        if (shopPanel.activeSelf)
        {
            shopPanel.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            shopPanel.SetActive(true);
            OnShowInventory.Invoke();
            nameText.text = "";
            priceText.text = "";
            pageText.text = "Shop";
            SwapPageShop();
        }
    }

    public void LoadShop()
    {
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
        }
        for (int i = 0; i < slots.Count; i++)
        {
            if (i <= content.childCount - 1)
            {
                content.GetChild(i).gameObject.SetActive(true);
                content.GetChild(i).GetChild(0).GetComponent<Image>().sprite = slots[i].sprite;
                content.GetChild(i).GetComponent<SlotShop>().code = slots[i];
            }
            else
            {
                SlotShop item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                item.transform.SetParent(content, false);
                item.transform.GetChild(0).GetComponent<Image>().sprite = slots[i].sprite;
                item.GetComponent<SlotShop>().code = slots[i];
            }
        }
        textGold.text = player.GetComponent<Player>().inventory.gold.ToString();
    }
    public void LoadInventory()
    {
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
                    content.GetChild(i).GetComponent<SlotShop>().code = slots[i].data;
                }
            }
            else
            {
                if (slots[i].data.itemCode != itemCode.nullItem)
                {
                    SlotShop item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                    item.transform.SetParent(content, false);
                    item.transform.GetChild(0).GetComponent<Image>().sprite = slots[i].data.sprite;
                    item.GetComponent<SlotShop>().code = slots[i].data;
                }
            }
        }
        textGold.text = player.GetComponent<Player>().inventory.gold.ToString();
    }
    public void SwapPageInventory()
    {
        LoadInventory();
        nameText.text = "";
        priceText.text = "";
        infomationItem.GetComponent<SlotShop>().code = new ItemData();
        pageText.text = "Inventory";
        buttonSell.SetActive(true);
        buttonBuy.SetActive(false);
    }
    public void SwapPageShop()
    {
        LoadShop();
        nameText.text = "";
        priceText.text = "";
        infomationItem.GetComponent<SlotShop>().code = new ItemData();
        pageText.text = "Shop";
        buttonSell.SetActive(false);
        buttonBuy.SetActive(true);
    }

    public void BuyItem()
    {
        if (infomationItem.GetComponent<SlotShop>().code != null)
            if (player.GetComponent<Player>().inventory.gold >= infomationItem.GetComponent<SlotShop>().code.price)
            {
                player.GetComponent<Player>().inventory.RemoveGold(infomationItem.GetComponent<SlotShop>().code.price);
                player.GetComponent<Player>().inventory.Add(infomationItem.GetComponent<SlotShop>().code, 1);
                textGold.text = player.GetComponent<Player>().inventory.gold.ToString();
            }
            else
            {
                Debug.Log("Not enough gold");
            }
    }

    public void SellItem()
    {
        if (infomationItem.GetComponent<SlotShop>().code != null)
        {
            var playerIn = player.GetComponent<Player>().inventory;
            int index = playerIn.slots.FindIndex(x => x.data.itemCode == infomationItem.GetComponent<SlotShop>().code.itemCode);
            playerIn.AddGold(infomationItem.GetComponent<SlotShop>().code.price * 9 / 10);
            // playerIn.Remove(playerIn.slots.FindIndex(x => x.data.itemCode == infomationItem.GetComponent<SlotShop>().code.itemCode));
            if (playerIn.slots[index].count > 0)
            {
                playerIn.slots[index].count--;
                if (playerIn.slots[index].count == 0)
                {
                    playerIn.slots[index].data = new ItemData();
                    nameText.text = "";
                    priceText.text = "";
                    infomationItem.GetComponent<SlotShop>().code = new ItemData();
                }
            }
            textGold.text = playerIn.gold.ToString();
            LoadInventory();
        }
    }
}
