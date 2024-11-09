using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ShopOnline : MonoBehaviour
{
    [SerializeField] private GameObject shopOnlinePrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] public GameObject player;
    private DatabaseReference reference;
    private FirebaseUser auth;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void LoadShop()
    {
        // Ẩn tất cả các đối tượng con hiện có
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
        }

        reference.Child("Shop").Child("Item").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<DataSnapshot> childrenList = new List<DataSnapshot>(snapshot.Children);

                int index = 0;
                for (int i = childrenList.Count - 1; i >= 0; i--)
                {
                    var item = childrenList[i];
                    string id = item.Child("IdSeller").Value.ToString();
                    if (id == auth.UserId) continue;
                    string itemID = item.Key;
                    ItemData codeS = ReadWriteData.instance.itemDataList[Convert.ToInt32(item.Child("code").Value)];
                    int quantity = Convert.ToInt32(item.Child("quantity").Value);
                    int price = Convert.ToInt32(item.Child("price").Value);

                    if (index < content.childCount)
                    {
                        var slotShopOnline = content.GetChild(index).GetComponent<SlotShopOnline>();
                        content.GetChild(index).gameObject.SetActive(true);

                        slotShopOnline.code = codeS;
                        slotShopOnline.quantity = quantity;
                        slotShopOnline.price = price;
                        slotShopOnline.id = id;
                        slotShopOnline.itemID = itemID;

                        content.GetChild(index).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                        content.GetChild(index).GetChild(2).GetComponent<Button>().onClick.AddListener(slotShopOnline.BuyItem);
                    }
                    else
                    {
                        GameObject itemS = Instantiate(shopOnlinePrefab, Vector3.zero, Quaternion.identity);
                        itemS.transform.SetParent(content, false);

                        var slotShopOnline = itemS.GetComponent<SlotShopOnline>();
                        slotShopOnline.code = codeS;
                        slotShopOnline.quantity = quantity;
                        slotShopOnline.price = price;
                        slotShopOnline.id = id;
                        slotShopOnline.itemID = itemID;

                        itemS.transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                        itemS.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(slotShopOnline.BuyItem);
                    }

                    index++;
                }
            }
        });
    }
}
