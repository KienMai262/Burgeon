using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOnlineManager : MonoBehaviour
{
    [SerializeField] protected GameObject shopOnlineUI;
    [SerializeField] protected GameObject shopUI;
    [SerializeField] protected GameObject inventoryUI;
    [SerializeField] protected GameObject sellingUI;
    [SerializeField] protected TMP_Text textName;
    [SerializeField] protected TMP_Text textGold;

    private void Start()
    {
        shopUI.SetActive(false);
        inventoryUI.SetActive(false);
        sellingUI.SetActive(false);
        shopOnlineUI.SetActive(false);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha8)){
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        if (shopOnlineUI.activeSelf)
        {
            shopUI.SetActive(false);
            inventoryUI.SetActive(false);
            sellingUI.SetActive(false);
            shopOnlineUI.SetActive(false);
        }
        else
        {
            shopOnlineUI.SetActive(true);
            shopUI.SetActive(true);
            LoadShop();
        }
    }

    public void LoadShop()
    {
        shopUI.SetActive(true);
        inventoryUI.SetActive(false);
        sellingUI.SetActive(false);
        shopUI.GetComponent<ShopOnline>().LoadShop();
        textGold.text = shopUI.GetComponent<ShopOnline>().player.GetComponent<Player>().inventory.gold.ToString();
        textName.text = "Shop";
    }
    public void LoadInventory()
    {
        shopUI.SetActive(false);
        inventoryUI.SetActive(true);
        sellingUI.SetActive(false);
        inventoryUI.GetComponent<InventorySellOnline>().LoadInventory();
        textGold.text = inventoryUI.GetComponent<InventorySellOnline>().player.GetComponent<Player>().inventory.gold.ToString();
        textName.text = "Inventory";
    }

    public void LoadSelling(){
        shopUI.SetActive(false);
        inventoryUI.SetActive(false);
        sellingUI.SetActive(true);
        sellingUI.GetComponent<Selling>().LoadSelling();
        textGold.text = sellingUI.GetComponent<Selling>().player.GetComponent<Player>().inventory.gold.ToString();
        sellingUI.GetComponent<Selling>().buttonAddGold.GetComponent<Button>().onClick.RemoveAllListeners();
        sellingUI.GetComponent<Selling>().buttonAddGold.GetComponent<Button>().onClick.AddListener(AddGold);
        textName.text = "Selling";
    }

    public void AddGold()
    {
        var selltmp = sellingUI.GetComponent<Selling>();
        selltmp.player.GetComponent<Player>().inventory.AddGold((int)(selltmp.gold * selltmp.percent));
        FirebaseDatabase.DefaultInstance.RootReference.Child("Shop").Child("Seller").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("gold").SetValueAsync("0");
        LoadSelling();
    }
}
