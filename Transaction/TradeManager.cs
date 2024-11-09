using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google.MiniJSON;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    [SerializeField] public GameObject dataMe;
    [SerializeField] public GameObject dataTrade;
    [SerializeField] public GameObject tradeUI;
    [SerializeField] public GameObject inviteTradeUI;
    [SerializeField] protected GameObject noteUI;

    string tradeId = "";
    public string tradeUserId = "";
    bool meAccept = false;
    bool tradeAccept = false;

    bool isMakeEvent = false;
    private DatabaseReference reference;
    private FirebaseUser auth;

    public static TradeManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    private void Start()
    {
        dataMe.GetComponent<LoadDataMe>().enterGoldUI.SetActive(false);
        tradeUI.SetActive(false);
        inviteTradeUI.SetActive(false);
        reference.Child("Users").Child(auth.UserId).Child("Trade").ValueChanged += HandleTrade;
        reference.Child("Users").Child(auth.UserId).Child("Trading").ValueChanged += HandleAcceptTrade;
    }

    private void OnDisable()
    {
        reference.Child("Users").Child(auth.UserId).Child("Trade").ValueChanged -= HandleTrade;
        reference.Child("Users").Child(auth.UserId).Child("Trading").ValueChanged -= HandleAcceptTrade;
    }

    protected async void HandleTrade(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            Debug.Log(tradeUserId);
            tradeUserId = args.Snapshot.Value.ToString();
            Debug.Log(tradeUserId);
            if (!string.IsNullOrEmpty(tradeUserId))
            {
                inviteTradeUI.SetActive(true);

                inviteTradeUI.transform.Find("Accept").GetComponent<UnityEngine.UI.Button>()
                    .onClick.AddListener(() => AcceptTrade(tradeUserId));
                inviteTradeUI.transform.Find("Decline").GetComponent<UnityEngine.UI.Button>()
                    .onClick.AddListener(() => DontAcceptTrade());

                // Sử dụng bất đồng bộ để lấy tên người dùng.
                var userNameSnapshot = await reference.Child("Users").Child(tradeUserId).Child("name").GetValueAsync();
                if (userNameSnapshot.Exists)
                {
                    inviteTradeUI.transform.Find("Text").GetComponent<TMP_Text>().text =
                        $"You have an invitation to trade from {userNameSnapshot.Value}";
                }
            }
        }
    }

    public async void AcceptTrade(string id)
    {
        inviteTradeUI.SetActive(false);
        // Xóa lời mời sau khi người chơi chấp nhận.
        await reference.Child("Users").Child(auth.UserId).Child("Trade").RemoveValueAsync();

        // Tạo timestamp trên máy chủ và lấy tradeId.
        DatabaseReference tempRef = reference.Child("tempTimestamp").Push();
        await tempRef.SetValueAsync(ServerValue.Timestamp);

        // Lấy giá trị timestamp từ máy chủ
        var getTask = await tempRef.GetValueAsync();
        if (getTask != null && getTask.Value != null)
        {
            tradeId = getTask.Value.ToString();
            await tempRef.RemoveValueAsync();
        }

        // Đăng ký sự kiện sau khi đã có tradeId.
        reference.Child("Trade").Child(tradeId).Child(id).Child("Items").ValueChanged += HandleAcceptItems;
        reference.Child("Trade").Child(tradeId).Child(id).Child("Accept").ValueChanged += HandleTradeOK;
        isMakeEvent = true;
        await reference.Child("Trade").Child(tradeId).Child(id).SetValueAsync("a");
        await reference.Child("Trade").Child(tradeId).Child(auth.UserId).SetValueAsync("a");
        // Cập nhật trạng thái trading.
        await reference.Child("Users").Child(id).Child("Trading").SetValueAsync(tradeId);
        await reference.Child("Users").Child(auth.UserId).Child("Trading").SetValueAsync(tradeId);
    }

    public void AcctiveEvent()
    {
        Debug.Log("Active: " + tradeUserId);
        reference.Child("Trade").Child(tradeId).Child(tradeUserId).Child("Items").ValueChanged += HandleAcceptItems;
        reference.Child("Trade").Child(tradeId).Child(tradeUserId).Child("Accept").ValueChanged += HandleTradeOK;
    }

    public void DontAcceptTrade()
    {
        reference.Child("Users").Child(auth.UserId).Child("Trade").RemoveValueAsync();
        inviteTradeUI.SetActive(false);
    }

    protected void HandleAcceptTrade(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {

            ToggleTrade();
            dataMe.GetComponent<LoadDataMe>().LoadInventory();
            reference.Child("Trade").Child(args.Snapshot.Value.ToString()).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("HandleAcceptTrade: " + tradeUserId);
                    foreach (DataSnapshot child in task.Result.Children)
                    {
                        Debug.Log(child.Key);
                        if (child.Key.ToString() != auth.UserId.ToString())
                        {
                            tradeUserId = child.Key.ToString();
                            Debug.Log("HandleAcceptTrade: " + tradeUserId);
                            tradeId = args.Snapshot.Value.ToString();
                            dataTrade.GetComponent<LoadDataTrader>().LoadTrader(tradeUserId);
                            dataMe.GetComponent<LoadDataMe>().buttonOK.GetComponent<Button>().onClick.RemoveAllListeners();
                            dataMe.GetComponent<LoadDataMe>().buttonOK.GetComponent<Button>().onClick.AddListener(dataMe.GetComponent<LoadDataMe>().EnterGold);
                            if (!isMakeEvent)
                            {
                                AcctiveEvent();
                            }
                        }
                    }
                }
            });
        }
    }

    public void ToggleTrade()
    {
        if (tradeUI.activeSelf)
        {
            tradeUI.SetActive(false);
        }
        else
        {
            tradeUI.SetActive(true);
        }
    }

    public void InviteTrade(string id)
    {

        bool online = false;
        reference.Child("status").Child(id).Child("online").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                online = (bool)task.Result.Value;
                if (online)
                {
                    reference.Child("Users").Child(id).Child("Trade").SetValueAsync(auth.UserId);
                }
            }
        });
    }

    public void AcceptTrade()
    {
        int gold = dataMe.GetComponent<LoadDataMe>().gold;

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(dataMe.GetComponent<LoadDataMe>().slots, Formatting.Indented, settings);

        reference.Child("Trade").Child(tradeId).Child(auth.UserId).Child("Items").Child("Item").SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                reference.Child("Trade").Child(tradeId).Child(auth.UserId).Child("Items").Child("Gold").SetValueAsync(gold);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error setting item data: " + task.Exception);
            }
        });
    }

    public void HandleAcceptItems(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            Debug.Log("HandleAcceptItems");
            dataTrade.GetComponent<LoadDataTrader>().LoadData(tradeId, tradeUserId);
        }
    }

    public void EnterTrade()
    {
        reference.Child("Trade").Child(tradeId).Child(auth.UserId).Child("Accept").SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                meAccept = true;
                foreach (Transform child in dataTrade.GetComponent<LoadDataTrader>().content)
                {
                    child.gameObject.SetActive(false);
                }
                dataTrade.transform.Find("ListItem").Find("Gold").Find("Gold").GetComponent<TMP_Text>().text = "0";
                dataMe.gameObject.transform.Find("ListItemSlots").Find("Gold").Find("Gold").GetComponent<TMP_Text>().text = "0";
                tradeUI.SetActive(false);
                if (tradeAccept && meAccept)
                {
                    AddAndRemoveItem();
                    noteUI.SetActive(true);
                    noteUI.GetComponent<Note>().text.GetComponent<TMP_Text>().text = "Transaction successful";
                    reference.Child("Users").Child(auth.UserId).Child("Trading").RemoveValueAsync();
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error setting accept value: " + task.Exception);
            }
        });
    }

    public void HandleTradeOK(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (args.Snapshot.Exists)
        {
            Debug.Log("HandleTradeOK");
            if ((bool)args.Snapshot.Value)
            {
                tradeAccept = true;
                if (tradeAccept && meAccept)
                {
                    AddAndRemoveItem();
                    noteUI.SetActive(true);
                    noteUI.GetComponent<Note>().text.GetComponent<TMP_Text>().text = "Transaction successful";
                    reference.Child("Users").Child(auth.UserId).Child("Trading").RemoveValueAsync();
                }
            }
            else
            {
                dataMe.GetComponent<LoadDataMe>().slots = new List<List<int>>();
                reference.Child("Trade").Child(tradeId).RemoveValueAsync();
                reference.Child("Users").Child(auth.UserId).Child("Trading").RemoveValueAsync();
                tradeUI.SetActive(false);
                // Hiển thị thông báo
                noteUI.SetActive(true);
                noteUI.GetComponent<Note>().text.GetComponent<TMP_Text>().text = "The other party has canceled the transaction";
            }
        }
    }

    public void CancelTrade()
    {
        reference.Child("Trade").Child(tradeId).Child(auth.UserId).Child("Accept").SetValueAsync(false).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                dataMe.GetComponent<LoadDataMe>().slots = new List<List<int>>();
                tradeUI.SetActive(false);
                reference.Child("Users").Child(auth.UserId).Child("Trading").RemoveValueAsync();
                // Hiển thị thông báo
                noteUI.SetActive(true);
                noteUI.GetComponent<Note>().text.GetComponent<TMP_Text>().text = "You have canceled the transaction";
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error canceling trade: " + task.Exception);
            }
        });
    }

    public void AddAndRemoveItem()
    {
        // Add item in inventory
        int gold;
        reference.Child("Trade").Child(tradeId).Child(tradeUserId).Child("Items").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                gold = Convert.ToInt32(snapshot.Child("Gold").Value.ToString());
                dataMe.GetComponent<LoadDataMe>().player.GetComponent<Player>().inventory.gold += gold;
                foreach (DataSnapshot child in snapshot.Child("Item").Children)
                {
                    int itemCode = Convert.ToInt32(child.Child("0").Value.ToString());
                    ItemData itemData = ReadWriteData.instance.itemDataList[itemCode];
                    int quantity = Convert.ToInt32(child.Child("1").Value.ToString());
                    dataMe.GetComponent<LoadDataMe>().player.GetComponent<Player>().inventory.Add(itemData, quantity);
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error adding items to inventory: " + task.Exception);
            }
        });

        // Remove item in inventory
        dataMe.GetComponent<LoadDataMe>().player.GetComponent<Player>().inventory.gold -= dataMe.GetComponent<LoadDataMe>().gold;
        foreach (var slot in dataMe.GetComponent<LoadDataMe>().slots)
        {
            var playerIn = dataMe.GetComponent<LoadDataMe>().player.GetComponent<Player>().inventory;
            int index = playerIn.slots.FindIndex(x => x.data.itemCode == ReadWriteData.instance.itemDataList[slot[0]].itemCode);
            for (int i = 0; i < slot[1]; i++)
            {
                dataMe.GetComponent<LoadDataMe>().player.GetComponent<Player>().inventory.Remove(index);
            }
        }

        dataMe.GetComponent<LoadDataMe>().slots = new List<List<int>>();
        dataMe.GetComponent<LoadDataMe>().gold = 0;
        tradeAccept = false;
        meAccept = false;

        reference.Child("Users").Child(auth.UserId).Child("Trading").RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error removing trading status: " + task.Exception);
            }
        });


        reference.Child("Trade").Child(tradeId).Child(tradeUserId).Child("Items").ValueChanged -= HandleAcceptItems;
        reference.Child("Trade").Child(tradeId).Child(tradeUserId).Child("Accept").ValueChanged -= HandleTradeOK;

        reference.Child("Trade").Child(tradeId).RemoveValueAsync();
        isMakeEvent = false;

    }
}