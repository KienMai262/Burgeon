using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public ItemData data;
        public int count;
        public int maxAllowed;


        public Slot()
        {
            data = new ItemData();
            count = 0;
            maxAllowed = 99;
        }
        public bool CanAddItem()
        {
            return count < maxAllowed;
        }
        public void AddItem(ItemData item, int quantity)
        {
            this.data = item;
            count += quantity;
        }
        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;
                if (count == 0)
                {
                    data = new ItemData();
                }
            }
        }
    }
    public List<Slot> slots = new List<Slot>();
    public int gold;

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            slots.Add(new Slot());
        }
    }

    //Items
    public void Add(ItemData item, int quantity)
    {
        foreach (Slot slot in slots)
        {
            if (slot.data.itemCode == item.itemCode && slot.CanAddItem())
            {
                slot.AddItem(item, quantity);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.data.itemCode == itemCode.nullItem)
            {
                slot.AddItem(item, quantity);
                int kt = 0;
                foreach (var item1 in RandomOrder.Instance.orderItems)
                {
                    if (item1.itemCode == item.itemCode)
                    {
                        kt = 1;
                        break;
                    }
                }
                if (kt == 0)
                {
                    RandomOrder.Instance.orderItems.Add(item);
                }
                return;
            }
        }
    }
    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }

    //Gold
    public void AddGold(int amount)
    {
        this.gold += amount;
    }
    public void RemoveGold(int amount)
    {
        this.gold -= amount;
    }

}
