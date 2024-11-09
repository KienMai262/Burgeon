using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataTrader : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] protected GameObject itemPrefab;
    [SerializeField] public RectTransform content;


    private DatabaseReference reference;
    private FirebaseUser auth;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void LoadTrader(string id)
    {
        reference.Child("Users").Child(id).Child("playerName").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("LoadTrader");
                DataSnapshot snapshot = task.Result;
                transform.Find("Trander").Find("Name").GetComponent<TMP_Text>().text = snapshot.Value.ToString();
            }
        });
    }

    public void LoadData(string tradeId, string userId)
    {
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
        }
        reference.Child("Trade").Child(tradeId).Child(userId).Child("Items").Child("Item").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<DataSnapshot> childrenList = new List<DataSnapshot>(snapshot.Children);
                for (int i = 0; i < childrenList.Count; i++)
                {
                    if (i <= content.childCount - 1)
                    {
                        content.GetChild(i).gameObject.SetActive(true);
                        content.GetChild(i).GetChild(0).GetComponent<Image>().sprite = ReadWriteData.instance.itemDataList[Convert.ToInt32(childrenList[i].Child("0").Value)].sprite;
                        content.GetChild(i).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = childrenList[i].Child("1").Value.ToString();
                    }
                    else
                    {
                        GameObject item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                        item.transform.SetParent(content, false);
                        item.transform.GetChild(0).GetComponent<Image>().sprite = ReadWriteData.instance.itemDataList[Convert.ToInt32(childrenList[i].Child("0").Value)].sprite;
                        item.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = childrenList[i].Child("1").Value.ToString();
                    }
                }
            }
        });
        reference.Child("Trade").Child(tradeId).Child(userId).Child("Items").Child("Gold").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    transform.Find("ListItem").Find("Gold").Find("Gold").GetComponent<TMP_Text>().text = snapshot.Value.ToString();
                }
                else
                {
                    transform.Find("ListItem").Find("Gold").Find("Gold").GetComponent<TMP_Text>().text = "0";
                }
            }
        });
    }
}
