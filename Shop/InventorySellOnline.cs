using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySellOnline : MonoBehaviour
{
    [SerializeField] protected GameObject slotPrefab;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected GameObject infomation;

    [SerializeField] protected TMP_InputField textQuantity;
    [SerializeField] protected TMP_InputField textGold;
    [SerializeField] protected GameObject buttonSell;


    [SerializeField] public GameObject player;

    private DatabaseReference reference;
    private FirebaseUser auth;

    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
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
                    content.GetChild(i).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slots[i].count.ToString();
                    content.GetChild(i).GetComponent<SlotSellOnline>().code = slots[i].data;
                    content.GetChild(i).GetComponent<SlotSellOnline>().quantity = slots[i].count;
                }
            }
            else
            {
                if (slots[i].data.itemCode != itemCode.nullItem)
                {
                    GameObject item = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
                    item.transform.SetParent(content, false);
                    item.transform.GetChild(0).GetComponent<Image>().sprite = slots[i].data.sprite;
                    item.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = slots[i].count.ToString();
                    item.GetComponent<SlotSellOnline>().code = slots[i].data;
                    item.GetComponent<SlotSellOnline>().quantity = slots[i].count;
                }
            }
        }

        buttonSell.GetComponent<Button>().onClick.RemoveAllListeners();
        buttonSell.GetComponent<Button>().onClick.AddListener(Sell);

    }

    public void Sell()
    {
        Debug.Log("Sell");
        if (textQuantity.text != "" && textGold.text != "")
        {
            int quantity = int.Parse(textQuantity.text);
            int price = int.Parse(textGold.text);

            var ifmt = infomation.GetComponent<SlotSellOnline>();
            int code = -1;
            for (int i = 0; i < ReadWriteData.instance.itemDataList.Count; i++)
            {
                if (ReadWriteData.instance.itemDataList[i].itemCode == ifmt.code.itemCode)
                {
                    code = i;
                    break;
                }
            }
            if (ifmt.quantity >= quantity && code != -1)
            {
                // Tạo một tham chiếu tạm thời để lưu trữ timestamp
                DatabaseReference tempRef = reference.Child("tempTimestamp").Push();
                tempRef.SetValueAsync(ServerValue.Timestamp).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        // Lấy giá trị timestamp từ máy chủ
                        tempRef.GetValueAsync().ContinueWithOnMainThread(getTask =>
                        {
                            if (getTask.IsCompleted)
                            {
                                long timestamp = (long)getTask.Result.Value;
                                string itemID = timestamp.ToString();
                                Debug.Log("Timestamp: " + itemID);
                                string idSeller = auth.UserId;

                                ItemDataJson itemDataJson = new ItemDataJson();
                                itemDataJson.IdSeller = idSeller;
                                itemDataJson.code = code;
                                itemDataJson.quantity = quantity;
                                itemDataJson.price = price;

                                ItemUsedSellJson itemUsedSellJson = new ItemUsedSellJson();
                                itemUsedSellJson.code = code;
                                itemUsedSellJson.quantity = quantity;
                                itemUsedSellJson.price = price;

                                string json = JsonUtility.ToJson(itemDataJson);
                                string jsonUsed = JsonUtility.ToJson(itemUsedSellJson);

                                reference.Child("Shop").Child("Item").Child(itemID).SetRawJsonValueAsync(json).ContinueWithOnMainThread(saveTask =>
                                {
                                    if (saveTask.IsCompleted)
                                    {
                                        Debug.Log("Item saved successfully: " + json);
                                    }
                                    else
                                    {
                                        Debug.LogError("Failed to save item: " + saveTask.Exception);
                                    }
                                });

                                reference.Child("Shop").Child("Seller").Child(idSeller).Child(itemID).SetRawJsonValueAsync(jsonUsed).ContinueWithOnMainThread(saveTask =>
                                {
                                    if (saveTask.IsCompleted)
                                    {
                                        Debug.Log("Seller item saved successfully: " + jsonUsed);
                                    }
                                    else
                                    {
                                        Debug.LogError("Failed to save seller item: " + saveTask.Exception);
                                    }
                                });

                                // Xóa tham chiếu tạm thời
                                tempRef.RemoveValueAsync();
                            }
                            else
                            {
                                Debug.LogError("Failed to get timestamp: " + getTask.Exception);
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError("Failed to set timestamp: " + task.Exception);
                    }
                });
                var playerIn = player.GetComponent<Player>().inventory;
                int index = playerIn.slots.FindIndex(x => x.data.itemCode == infomation.GetComponent<SlotSellOnline>().code.itemCode);
                for (int i = 0; i < quantity; i++)
                    playerIn.Remove(index);
            }
            else
            {
                Debug.Log("Quantity is not enough");
            }
            textQuantity.text = "";
            textGold.text = "";

        }
    }
}
public class ItemDataJson
{
    public string IdSeller;
    public int code;
    public int quantity;
    public int price;
}
public class ItemUsedSellJson
{
    public int code;
    public int quantity;
    public int price;
}
