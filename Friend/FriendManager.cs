using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FriendManager : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject friendUI;
    [SerializeField] protected GameObject friendList;
    [SerializeField] protected GameObject friendListPrefab;
    [SerializeField] protected GameObject inviteList;
    [SerializeField] protected GameObject inviteListPrefab;
    [SerializeField] protected GameObject searchFriend;


    private DatabaseReference reference;
    private FirebaseUser auth;
    private void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }


    private void Start()
    {
        friendUI.SetActive(false);
        reference.Child("Users").Child(auth.UserId).Child("friendsList").ValueChanged += HandleAddFriend;
        reference.Child("Users").Child(auth.UserId).Child("inviteList").ValueChanged += HandleInviteFriend;
    }

    private void OnDestroy()
    {
        reference.Child("Users").Child(auth.UserId).Child("friendsList").ValueChanged -= HandleAddFriend;
        reference.Child("Users").Child(auth.UserId).Child("inviteList").ValueChanged -= HandleInviteFriend;
    }

    protected void HandleAddFriend(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot snapshot = args.Snapshot;
        if (snapshot.Exists)
        {
            if (!player.IsDestroyed())
            {
                var playerComponent = player.GetComponent<Player>();
                playerComponent.friendsList.Clear();
                foreach (DataSnapshot child in snapshot.Children)
                {
                    playerComponent.friendsList.Add(child.Value.ToString());
                }
            }
        }
    }

    protected void HandleInviteFriend(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot snapshot = args.Snapshot;
        if (snapshot.Exists)
        {
            if (!player.IsDestroyed())
            {
                var playerComponent = player.GetComponent<Player>();
                playerComponent.inviteList.Clear();
                foreach (DataSnapshot child in snapshot.Children)
                {
                    playerComponent.inviteList.Add(child.Value.ToString());
                }
            }
        }
    }
    public void ToggleFriend()
    {
        if (friendUI.activeSelf)
        {
            friendUI.SetActive(false);
        }
        else
        {
            friendUI.SetActive(true);
            ShowListFriend();
        }

    }

    //Friend
    public void ShowListFriend()
    {
        inviteList.SetActive(false);
        searchFriend.SetActive(false);
        friendList.SetActive(true);
        foreach (Transform child in friendList.transform.GetChild(0).GetChild(0))
        {
            Destroy(child.gameObject);
        }
        gameObject.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Friend List";
        var listF = player.GetComponent<Player>().friendsList;
        foreach (string list in listF)
        {
            Debug.Log(list);
            reference.Child("Users").Child(list).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string name = snapshot.Child("playerName").Value.ToString();
                    GameObject friend = Instantiate(friendListPrefab, friendList.transform.GetChild(0).GetChild(0).transform);
                    friend.transform.SetParent(friendList.transform.GetChild(0).GetChild(0), false);
                    friend.transform.GetChild(1).GetComponent<TMP_Text>().text = name;

                    //Transaction

                    friend.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => TradeManager.instance.InviteTrade(list));
                    TradeManager.instance.tradeUserId = list;
                    //Chat
                    friend.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ChatAndNoteManager.instance.ChatFriend(list));
                }
            });
        }
    }

    //Search
    public void ShowSearchFriend()
    {
        friendList.SetActive(false);
        inviteList.SetActive(false);
        searchFriend.SetActive(true);
        gameObject.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Friend Search";
        searchFriend.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text = "";
        searchFriend.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        searchFriend.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
    }
    string id;
    public void Search()
    {
        searchFriend.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        searchFriend.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        int kt = 0;
        id = searchFriend.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text;
        searchFriend.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text = "";
        foreach (string list in player.GetComponent<Player>().friendsList)
        {
            if (list == id)
            {
                kt = 1;
                break;
            }
        }
        if (id == auth.UserId)
        {
            kt = 1;
        }

        if (kt == 0)
        {
            reference.Child("Users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error");
                    searchFriend.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    searchFriend.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Success");
                    DataSnapshot snapshot = task.Result;
                    Debug.Log(snapshot.Exists);
                    if (snapshot.Exists)
                    {
                        Debug.Log(snapshot.Child("playerName").Value);
                        string name = snapshot.Child("playerName").Value.ToString();
                        searchFriend.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                        searchFriend.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = name;
                        searchFriend.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("Not Found");
                        searchFriend.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                        searchFriend.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                    }
                }
            });
        }
    }
    public void AddFriend()
    {
        reference.Child("Users").Child(id).Child("inviteList").Child(CountInviteList(id).ToString()).SetValueAsync(auth.UserId.ToString());
        searchFriend.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        searchFriend.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text = "";
    }
    public int CountInviteList(string id)
    {
        int size = 0;
        reference.Child("Users").Child(id).Child("inviteList").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot child in snapshot.Children)
                {
                    size++;
                }
            }
            else
            {
                Debug.LogError("Lỗi khi lấy danh sách inviteList: " + task.Exception);
            }
        });
        return size;
    }

    //invite
    public void ShowListInvite()
    {
        friendList.SetActive(false);
        searchFriend.SetActive(false);
        inviteList.SetActive(true);
        foreach (Transform child in inviteList.transform.GetChild(0).GetChild(0))
        {
            Destroy(child.gameObject);
        }
        gameObject.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Invite List";
        foreach (string child in player.GetComponent<Player>().inviteList)
        {
            GameObject invite = Instantiate(inviteListPrefab, inviteList.transform.GetChild(0).GetChild(0).transform);
            reference.Child("Users").Child(child).Child("playerName").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot1 = task.Result;
                    invite.transform.GetChild(1).GetComponent<TMP_Text>().text = snapshot1.Value.ToString();
                }
            });
            invite.transform.SetParent(inviteList.transform.GetChild(0).GetChild(0), false);
            int id = player.GetComponent<Player>().inviteList.IndexOf(child);
            invite.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => AcceptInvite(id));
            invite.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DeclineInvite(id));
        }
    }
        public async void AcceptInvite(int id)
    {
        string uid = player.GetComponent<Player>().inviteList[id];
        await reference.Child("Users").Child(auth.UserId).Child("inviteList").Child(id.ToString()).RemoveValueAsync();
        GameObject button = EventSystem.current.currentSelectedGameObject;
        if (button != null)
        {
            Destroy(button.transform.parent.gameObject);
        }
        player.GetComponent<Player>().inviteList.RemoveAt(id);
    
        int friendCount = await CountFriendsListAsync(auth.UserId);
        await reference.Child("Users").Child(auth.UserId).Child("friendsList").Child(friendCount.ToString()).SetValueAsync(uid);
    
        friendCount = await CountFriendsListAsync(uid);
        await reference.Child("Users").Child(uid).Child("friendsList").Child(friendCount.ToString()).SetValueAsync(auth.UserId);
    }
    public async Task<int> CountFriendsListAsync(string id)
    {
        int size = 0;
        var task = await reference.Child("Users").Child(id).Child("friendsList").GetValueAsync();
        if (task != null)
        {
            DataSnapshot snapshot = task;
            foreach (DataSnapshot child in snapshot.Children)
            {
                size++;
            }
        }
        else
        {
            Debug.LogError("Lỗi khi lấy danh sách friendsList.");
        }
        Debug.Log("friend: " + id + " : " + size);
        return size;
    }
    public void DeclineInvite(int id)
    {
        reference.Child("Users").Child(auth.UserId).Child("inviteList").Child(id.ToString()).RemoveValueAsync();
        GameObject button = EventSystem.current.currentSelectedGameObject;
        Destroy(button.transform.parent.gameObject);
        player.GetComponent<Player>().inviteList.RemoveAt(id);
    }
}
