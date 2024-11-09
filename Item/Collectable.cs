using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Collectable : MonoBehaviour
{
    public void AddToInventory()
    {
        Player player = FindObjectOfType<Player>();
        Item item = GetComponent<Item>();
        if (item != null)
        {
            if (player != null)
            {
                player.inventory.Add(item.data, 1);
            }
        }
    }
}
