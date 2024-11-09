using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selling : MonoBehaviour
{
    [SerializeField] private GameObject shopOnlinePrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] protected GameObject textGold;
    [SerializeField] public GameObject buttonAddGold;
    [SerializeField] public GameObject player;
    private DatabaseReference reference;
    private FirebaseUser auth;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public int gold = 0;
    public float percent = 0.9f;

    public void LoadSelling()
    {
        // Ẩn tất cả các đối tượng con hiện có
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
        }

        reference.Child("Shop").Child("Seller").Child(auth.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
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
                foreach (var child in childrenList)
                {
                    Debug.Log(child.Key);
                    if (child.Key != "gold")
                    {
                        if (index < content.childCount)
                        {
                            var slotShopOnline = content.GetChild(index).GetComponent<SlotShopOnline>();
                            content.GetChild(index).gameObject.SetActive(true);

                            slotShopOnline.code = ReadWriteData.instance.itemDataList[Convert.ToInt32(child.Child("code").Value)];
                            slotShopOnline.quantity = Convert.ToInt32(child.Child("quantity").Value);
                            slotShopOnline.price = Convert.ToInt32(child.Child("price").Value);
                            slotShopOnline.id = auth.UserId;
                            slotShopOnline.itemID = child.Key;

                            content.GetChild(index).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                            content.GetChild(index).GetChild(2).GetComponent<Button>().onClick.AddListener(slotShopOnline.Cancel);
                        }
                        else
                        {
                            GameObject shopOnline = Instantiate(shopOnlinePrefab, Vector3.zero, Quaternion.identity);
                            shopOnline.transform.SetParent(content, false);

                            var slotShopOnline = shopOnline.GetComponent<SlotShopOnline>();
                            slotShopOnline.code = ReadWriteData.instance.itemDataList[Convert.ToInt32(child.Child("code").Value)];
                            slotShopOnline.quantity = Convert.ToInt32(child.Child("quantity").Value);
                            slotShopOnline.price = Convert.ToInt32(child.Child("price").Value);
                            slotShopOnline.id = auth.UserId;
                            slotShopOnline.itemID = child.Key;

                            shopOnline.transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                            shopOnline.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(slotShopOnline.Cancel);
                        }
                        index++;
                    }
                    else
                    {
                        if (child.Exists)
                        {
                            gold = Convert.ToInt32(child.GetValue(true));
                        }
                    }
                }
            }
        });

        textGold.GetComponent<TMP_Text>().text = gold.ToString();
        buttonAddGold.GetComponent<Button>().onClick.RemoveAllListeners();
    }

}
