using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] protected GameObject inventoryPanel;
    [SerializeField] protected SlotUI itemPrefab;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected TMP_Text textGold;
    [SerializeField] protected Player player;

    public List<SlotUI> slots = new List<SlotUI>();
    //   
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static InventoryUI Instance { get; private set; }
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
        InitializeInventoryUI(player.inventorySize);
        inventoryPanel.SetActive(false);
    }
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            inventoryPanel.SetActive(true);
            OnShowInventory.Invoke();
            Refresh();
        }
    }
    protected void Refresh()
    {
        if (slots.Count == player.inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (player.inventory.slots[i].data.itemCode != itemCode.nullItem)
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else
                {
                    slots[i].SetEmpty();
                }
            }
        }
        int gold = player.GetComponent<Player>().inventory.gold;
        textGold.text = gold.ToString();
    }
    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            SlotUI item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(content, false);
            slots.Add(item);
        }
    }

    public void Remove(int slotID)
    {
        player.inventory.Remove(slotID);
        Refresh();
    }
}
