using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class ChatAndNoteManager : MonoBehaviour, IChatClientListener
{
    public static ChatAndNoteManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private ChatClient chatClient;
    public string userId;
    public string friendId;
    public TMPro.TMP_Text chatLog;
    public TMPro.TMP_InputField inputField;
    public GameObject chatUI;

    // Danh sách để lưu trữ tin nhắn theo cặp (ID người nhận, danh sách nội dung tin nhắn)
    public List<Tuple<string, List<string>>> messageList = new List<Tuple<string, List<string>>>();

    private void Start()
    {
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        chatUI.SetActive(false);
        ConnectToPhotonChat();
    }
    public void ChatFriend(string id)
    {
        friendId = id;
        ToggleChat();
        UpdateChatLog();  // Cập nhật giao diện khi thay đổi người dùng
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();  // Cập nhật client để nhận tin nhắn
        }
    }

    // Kết nối đến Photon Chat
    public void ConnectToPhotonChat()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                           "1.0", new AuthenticationValues(userId));
    }

    public void OnSendButtonPressed()
    {
        string message = inputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            chatClient.SendPrivateMessage(friendId, message);

            var recipientTuple = messageList.Find(tuple => tuple.Item1 == friendId);

            if (recipientTuple == null)
            {
                recipientTuple = new Tuple<string, List<string>>(friendId, new List<string>());
                messageList.Add(recipientTuple);
            }

            recipientTuple.Item2.Add($"Me: {message}");

            inputField.text = "";
            UpdateChatLog();
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
    }

    private void UpdateChatLog()
    {
        var recipientTuple = messageList.Find(tuple => tuple.Item1 == friendId);
        if (recipientTuple != null)
        {
            chatLog.text = string.Join("\n", recipientTuple.Item2);
        }
        else
        {
            chatLog.text = "";
        }
    }

    public void ToggleChat()
    {
        chatUI.SetActive(!chatUI.activeSelf);
    }

    // Các phương thức của IChatClientListener
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"DebugReturn: {level} - {message}");
    }

    public void OnConnected()
    {
        Debug.Log("Đã kết nối đến máy chủ chat.");
    }

    public void OnDisconnected()
    {
        Debug.Log("Đã ngắt kết nối khỏi máy chủ chat.");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Trạng thái chat thay đổi thành: {state}");
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"Nhận được tin nhắn riêng từ {sender}");

        var recipientTuple = messageList.Find(tuple => tuple.Item1 == sender);
        if (recipientTuple == null)
        {
            recipientTuple = new Tuple<string, List<string>>(sender, new List<string>());
            messageList.Add(recipientTuple);
        }
        FirebaseDatabase.DefaultInstance.RootReference.Child("Users").Child(sender).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string name = snapshot.Child("playerName").Value.ToString();
                recipientTuple.Item2.Add($"{name}: {message}");
                if (sender == friendId)
                {
                    UpdateChatLog();
                }
            }
        });


    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Đã đăng ký vào các kênh.");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Đã hủy đăng ký khỏi các kênh.");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"Cập nhật trạng thái từ {user}: {status}");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"Người dùng {user} đã đăng ký vào kênh {channel}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"Người dùng {user} đã hủy đăng ký khỏi kênh {channel}");
    }
}