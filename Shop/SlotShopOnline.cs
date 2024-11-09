using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotShopOnline : MonoBehaviour
{
    public ItemData code;
    public int quantity;
    public int price;
    public string id;
    public string itemID;

    private DatabaseReference reference;
    private FirebaseUser auth;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }
    private void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = code.sprite;
        gameObject.transform.GetChild(1).GetComponent<TMP_Text>().text = code.itemName + "\n quantity: " + quantity + "\n price: " + price;
    }

    public void BuyItem()
    {
        var inventory = gameObject.transform.parent.parent.parent.GetComponent<ShopOnline>().player.GetComponent<Player>().inventory;
        if (inventory.gold >= price)
        {
            inventory.RemoveGold(price);
            inventory.Add(code, quantity);
        }

        var dataShop = reference.Child("Shop");
        dataShop.Child("Seller").Child(id).Child(itemID).RemoveValueAsync();
        int gold = 0;
        dataShop.Child("Seller").Child(id).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Child("gold").Exists)
                {
                    gold = Convert.ToInt32(snapshot.Child("gold").GetValue(true));
                }
                else {
                    gold = 0;
                }
            }
        });
        dataShop.Child("Seller").Child(id).Child("gold").SetValueAsync((gold + price).ToString());
        dataShop.Child("Item").Child(itemID).RemoveValueAsync();
    }

    public void Cancel(){
        Destroy(gameObject);
        reference.Child("Shop").Child("Seller").Child(auth.UserId).Child(itemID).RemoveValueAsync();
        reference.Child("Shop").Child("Item").Child(itemID).RemoveValueAsync();
        
        gameObject.transform.parent.parent.parent.GetComponent<Selling>().player.GetComponent<Player>().inventory.Add(code, quantity);

    }
}
