using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomOrder
{
    private static RandomOrder instance;
    public static RandomOrder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RandomOrder();
            }
            return instance;
        }
    }

    public class Order
    {
        public ItemData item;
        public int amount;
        public Order(ItemData item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    public List<ItemData> orderItems = new List<ItemData>();
    public List<Order> orderAmounts = new List<Order>();
    public int prize;

    public void RandomOrderItem()
    {
        int nbr = 5;
        if (orderItems.Count < 5)
        {
            nbr = orderItems.Count + 1;
        }
        int amount = Random.Range(1, nbr);
        for (int i = 0; i < amount; i++)
        {
            int itemIndex = -1;
            itemCode selectedItem = itemCode.nullItem;
            int nBP = Random.Range(3, 20);

            while (orderAmounts.Exists(order => order.item.itemCode == selectedItem) || selectedItem == itemCode.nullItem)
            {
                itemIndex = Random.Range(0, orderItems.Count);
                Debug.Log("itemIndex: " + itemIndex);
                selectedItem = orderItems[itemIndex].itemCode;
            }

            orderAmounts.Add(new Order(orderItems[itemIndex], nBP));
            prize += nBP * orderItems[itemIndex].price*(Random.Range(100, 180)/100);
        }
    }
}
