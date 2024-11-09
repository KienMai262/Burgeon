using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Firebase.Extensions;
using Newtonsoft.Json;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    //create a singleton
    public static CreateAndJoinRoom instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private DatabaseReference reference;
    private FirebaseUser auth;

    private void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(auth.UserId, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        reference.Child("Users").Child(auth.UserId).Child("roomCoop").SetValueAsync(true);
        roomIDkt = auth.UserId;

        //RoomUI
        
    }

    string roomIDkt;
    public void JoinRoom(string roomID)
    {
        reference.Child("Users").Child(roomID).Child("roomCoop").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Room not found");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists && snapshot.Value is bool roomCoop)
                {
                    if (roomCoop)
                    {
                        PhotonNetwork.JoinRoom(roomID);
                        roomIDkt = roomID;
                    }
                    else
                    {
                        Debug.Log("Room is not cooperative");
                    }
                }
                else
                {
                    Debug.Log("RoomCoop value not found or invalid");
                }
            }
        });
    }
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            ReadWriteData.instance.SaveDataToFirebase(auth.UserId);
            PhotonNetwork.LoadLevel("SampleScene");
            JoinRoomId(roomIDkt);
        }
    }
    public void JoinRoomId(string roomID)
    {
        ReadWriteData.instance.SaveDataToFirebase(roomID);
        reference.Child("Users").Child(roomID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Room not found");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                ReadWriteData.DataSave dataSave = new ReadWriteData.DataSave();
                if (snapshot.Exists)
                {
                    string data = snapshot.GetRawJsonValue();
                    dataSave = JsonConvert.DeserializeObject<ReadWriteData.DataSave>(data);

                    ReadWriteData.instance.Loadmap(dataSave);
                    ReadWriteData.instance.LoadStateRations(dataSave);
                    ReadWriteData.instance.LoadStateAnimal(dataSave);
                    ReadWriteData.instance.LoadStateTree(dataSave);
                }
            }
        });
    }
}
