using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading;
using System;
using System.Data;
using Firebase.Auth;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ReadWriteData : MonoBehaviour
{
    public DatabaseReference reference;
    public FirebaseUser auth;
    //create a singleton
    public static ReadWriteData instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    [Header("Game Object")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject rations;
    [SerializeField] protected GameObject poultry;
    [SerializeField] protected GameObject cattle;
    [SerializeField] protected GameObject trees;
    [SerializeField] protected GameObject map;

    [SerializeField] public List<ItemData> itemDataList;

    public float timeInactive;

    public class DataSave
    {
        public string timeOut;
        public string playerName;
        public SerializableVector3 positionPlayer;
        public int level;
        public int exp;
        public int expNeed;
        public List<List<int>> inventory;
        public int gold;
        public List<string> friendsList = new List<string>();
        public List<string> inviteList = new List<string>();
        public List<List<rationData>> rations;
        public List<poultryData> poultry;
        public List<poultryData> cattle;
        public List<treeData> fruitTrees;
        public List<treeData> berryTrees;
        public List<bool> map;
        public orderData orderData;

        public DataSave()
        {
            inventory = new List<List<int>>();
            rations = new List<List<rationData>>();
            poultry = new List<poultryData>();
            cattle = new List<poultryData>();
            fruitTrees = new List<treeData>();
            berryTrees = new List<treeData>();
            map = new List<bool>();
            orderData = new orderData();
        }


    }
    public class rationData
    {
        public int indexRation;
        public float timeRation;
        public bool isWater;
        public rationData()
        {
            indexRation = -1;
            timeRation = 0;
            isWater = false;
        }
    }
    public class poultryData
    {
        public bool isEating;
        public float timeRun;
        public poultryData()
        {
            isEating = false;
            timeRun = 0;
        }
    }

    public class treeData
    {
        public bool isFruit;
        public float timeColldowTree;
        public treeData()
        {
            isFruit = false;
            timeColldowTree = 0;
        }
    }

    public class orderData
    {
        public List<int> orderUnlock;
        public List<List<itemInOrder>> orderItems;
        public List<int> orderPrize;

        public int quantityOrder;
        public class itemInOrder
        {
            public int itemCode;
            public int amount;
        }
        public orderData()
        {
            orderUnlock = new List<int>();
            orderItems = new List<List<itemInOrder>>();
            orderPrize = new List<int>();
        }
    }

    DataSave dataSave = new DataSave();


    private void Start()
    {
        // Debug.Log("LoataFromFirebase");
        LoadDataFromFirebase(auth.UserId);

        DatabaseReference userStatusRef = FirebaseDatabase.DefaultInstance.GetReference("/status/" + auth.UserId);
        DatabaseReference connectedRef = FirebaseDatabase.DefaultInstance.GetReference(".info/connected");

        connectedRef.ValueChanged += (object sender, ValueChangedEventArgs e) =>
        {
            if ((bool)e.Snapshot.Value)
            {
                userStatusRef.Child("online").SetValueAsync(true);
            }
            else
            {
                // Khi người dùng mất kết nối
                userStatusRef.Child("online").SetValueAsync(false);
                userStatusRef.Child("lastOnline").SetValueAsync(ServerValue.Timestamp);
            }
        };
    }

    private void OnApplicationQuit()
    {
        DatabaseReference userStatusRef = FirebaseDatabase.DefaultInstance.GetReference("/status/" + auth.UserId);
        userStatusRef.Child("online").SetValueAsync(false);
        userStatusRef.Child("lastOnline").SetValueAsync(ServerValue.Timestamp);
        SaveDataToFirebase(auth.UserId);
    }

    //image load game
    [Header("Image Load Game")]
    int sumCountLoad = 6;
    int countLoad = 0;
    [SerializeField] private GameObject slideLoadGame;
    private void Update()
    {
        if (countLoad <= sumCountLoad)
        {
            slideLoadGame.transform.GetChild(0).GetComponent<Slider>().value = (float)countLoad / sumCountLoad;
        }
        if (countLoad == sumCountLoad)
        {
            Debug.Log("Load data complete!");
            Invoke("Active", 1f);
            countLoad++;
        }
    }
    protected void Active()
    {
        slideLoadGame.SetActive(false);
    }


    public void SaveDataToFirebase(string userId)
    {
        dataSave.timeOut = System.DateTime.Now.ToString();
        Debug.Log(dataSave.timeOut);
        dataSave.playerName = player.GetComponent<Player>().playerName;
        dataSave.positionPlayer = new SerializableVector3(player.transform.position);
        dataSave.level = player.GetComponent<Player>().level;
        dataSave.exp = player.GetComponent<Player>().exp;
        dataSave.expNeed = player.GetComponent<Player>().expNeed;

        dataSave.inventory = new List<List<int>>();
        SaveStateInventory();

        dataSave.friendsList = new List<string>();
        dataSave.inviteList = new List<string>();
        SaveFriend();

        dataSave.rations = new List<List<rationData>>();
        SaveStateRations();

        dataSave.poultry = new List<poultryData>();
        dataSave.cattle = new List<poultryData>();
        SaveStateAnimal();

        dataSave.fruitTrees = new List<treeData>();
        dataSave.berryTrees = new List<treeData>();
        SaveStateTree();

        dataSave.map = new List<bool>();
        Savemap();

        dataSave.orderData = new orderData();
        SaveOrder();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(dataSave, Formatting.Indented, settings);

        // Cập nhật hoặc thêm mới dữ liệu
        reference.Child("Users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Data saved to Firebase successfully.");
            }
            else
            {
                Debug.LogError("Failed to save data to Firebase: " + task.Exception);
            }
        });

    }
    public void LoadDataFromFirebase(string userId)
    {
        reference.Child("Users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        // reference.Child("Users").Child("xNCtkKCUC2UWdFJRnzkHIxqF4zJ2").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string data = snapshot.GetRawJsonValue();
                    dataSave = JsonConvert.DeserializeObject<DataSave>(data);
                    Debug.Log("Data loaded from Firebase!");


                    Loadmap(dataSave);
                    if (dataSave.playerName.Length < 4 || dataSave.playerName.Length > 20)
                    {
                        player.GetComponent<Player>().NameToPlayer();
                    }
                    else
                    {
                        player.GetComponent<Player>().playerName = dataSave.playerName;
                    }
                    player.GetComponent<Player>().playerID = userId;
                    player.transform.position = dataSave.positionPlayer.ToVector3();
                    player.GetComponent<Player>().level = dataSave.level;
                    player.GetComponent<Player>().exp = dataSave.exp;
                    player.GetComponent<Player>().expNeed = dataSave.expNeed;
                    LoadStateInventory();
                    LoadStateRations(dataSave);
                    LoadStateAnimal(dataSave);
                    LoadStateTree(dataSave);
                    LoadOrder(dataSave);
                    // LoadFriend(dataSave);

                    // Calculate time inactive
                    DateTime timeIn = System.DateTime.Now;
                    DateTime timeOut = DateTime.Parse(dataSave.timeOut);
                    timeInactive = (float)(timeIn - timeOut).TotalSeconds;
                    if (timeIn.Date != timeOut.Date || timeIn.Month != timeOut.Month || timeIn.Year != timeOut.Year)
                    {
                        OrderManager.Instance.orderItems.Clear();
                        OrderManager.Instance.prizeL.Clear();
                        OrderManager.Instance.quantityOrder = 0;
                        OrderManager.Instance.LoadQuantityOrder();
                    }
                }
                else
                {
                    Debug.Log("UserId does not exist, creating new UserId.");
                    player.GetComponent<Player>().playerID = userId;
                    countLoad = sumCountLoad;
                    player.GetComponent<Player>().NameToPlayer();
                }
            }
            else
            {
                Debug.LogError("Failed to load data from Firebase: " + task.Exception);
            }
        });
    }

    protected void SaveStateInventory()
    {
        dataSave.gold = player.GetComponent<Player>().inventory.gold;
        foreach (var item in player.GetComponent<Player>().inventory.slots)
        {
            if (item.data == null)
            {
                dataSave.inventory.Add(new List<int> { -1, 0 });
            }
            else
            {
                foreach (var itemData in itemDataList)
                {
                    if (item.data.itemCode == itemData.itemCode)
                    {
                        dataSave.inventory.Add(new List<int> { itemDataList.IndexOf(itemData), item.count });
                        break;
                    }
                }
            }
        }
    }
    protected void LoadStateInventory()
    {
        Debug.Log("LoadStateInventory");
        player.GetComponent<Player>().inventory.gold = dataSave.gold;
        for (int i = 0; i < dataSave.inventory.Count; i++)
        {
            if (dataSave.inventory[i][0] == -1)
            {
                player.GetComponent<Player>().inventory.slots[i] = new Inventory.Slot();
            }
            else
            {
                player.GetComponent<Player>().inventory.slots[i] = new Inventory.Slot();
                player.GetComponent<Player>().inventory.slots[i].data = itemDataList[dataSave.inventory[i][0]];
                player.GetComponent<Player>().inventory.slots[i].count = dataSave.inventory[i][1];
            }
        }
        countLoad++;
        // Debug.Log(countLoad);
    }

    // Save/Load Rations
    protected void SaveStateRations()
    {
        foreach (Transform child in rations.transform)
        {
            // if (child.gameObject.activeSelf == false)
            // {
            //     continue;
            // }
            List<rationData> rationsList = new List<rationData>();
            foreach (Transform child2 in child.transform.GetChild(1))
            {
                rationData rationData = new rationData();
                int indexRation = child2.GetComponent<Rations>().saveIndexRationData;
                rationData.indexRation = indexRation;
                if (indexRation != -1)
                {
                    var ration = child2.GetComponent<Rations>().rationList[indexRation].GetComponent<Ration>();
                    rationData.timeRation = ration.timeGrowRun;
                    rationData.isWater = ration.isWater;
                    rationsList.Add(rationData);
                }
                else
                {
                    rationsList.Add(rationData);
                }
            }
            dataSave.rations.Add(rationsList);
        }
    }

    public void LoadStateRations(DataSave dataSave)
    {

        Debug.Log("LoadStateRations");
        foreach (Transform child in rations.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            foreach (Transform child2 in child.transform.GetChild(1))
            {
                var rationData = dataSave.rations[child.GetSiblingIndex()][child2.GetSiblingIndex()];
                int indexRation = rationData.indexRation;
                if (indexRation != -1)
                {
                    child2.GetComponent<Rations>().LoadRation(indexRation);
                    var ration = child2.GetComponent<Rations>().rationList[indexRation].GetComponent<Ration>();
                    ration.timeGrowRun = rationData.timeRation;
                    ration.isWater = rationData.isWater;
                }
            }
        }
        countLoad++;
        // Debug.Log(countLoad);
    }

    // Save/Load Animals
    protected void SaveStateAnimal()
    {
        foreach (Transform child in poultry.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            poultryData poultryData = new poultryData();
            poultryData.isEating = child.GetComponent<BarnChicken>().isEating;
            poultryData.timeRun = child.GetComponent<BarnChicken>().timeRun;
            dataSave.poultry.Add(poultryData);
        }
        foreach (Transform child in cattle.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            poultryData poultryData = new poultryData();
            poultryData.isEating = child.GetComponent<BarnCow>().isEating;
            poultryData.timeRun = child.GetComponent<BarnCow>().timeRun;
            dataSave.cattle.Add(poultryData);
        }
    }
    public void LoadStateAnimal(DataSave dataSave)
    {

        Debug.Log("LoadStateAnimal");
        foreach (Transform child in poultry.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            var poultryData = dataSave.poultry[child.GetSiblingIndex()];
            child.GetComponent<BarnChicken>().isEating = poultryData.isEating;
            child.GetComponent<BarnChicken>().timeRun = poultryData.timeRun;
        }
        foreach (Transform child in cattle.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            var poultryData = dataSave.cattle[child.GetSiblingIndex()];
            child.GetComponent<BarnCow>().isEating = poultryData.isEating;
            child.GetComponent<BarnCow>().timeRun = poultryData.timeRun;
        }
        countLoad++;
        // Debug.Log(countLoad);
    }

    // Save/Load Trees
    protected void SaveStateTree()
    {
        //Save Fruit Trees
        foreach (Transform child in trees.transform.GetChild(0))
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            treeData treeData = new treeData();
            treeData.isFruit = child.GetComponent<FruitTree>().isFruit;
            treeData.timeColldowTree = child.GetComponent<FruitTree>().timeColldowTree;
            dataSave.fruitTrees.Add(treeData);
        }
        //Save Berry Trees
        foreach (Transform child in trees.transform.GetChild(1))
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            treeData treeData = new treeData();
            treeData.isFruit = child.GetComponent<Berries>().isFruit;
            treeData.timeColldowTree = child.GetComponent<Berries>().timeColldowTree;
            dataSave.berryTrees.Add(treeData);
        }
    }
    public void LoadStateTree(DataSave dataSave)
    {
        Debug.Log("LoadStateTree");
        //Load Fruit Trees
        foreach (Transform child in trees.transform.GetChild(0))
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            var treeData = dataSave.fruitTrees[child.GetSiblingIndex()];
            child.GetComponent<FruitTree>().isFruit = treeData.isFruit;
            child.GetComponent<FruitTree>().timeColldowTree = treeData.timeColldowTree;
            if (!treeData.isFruit)
            {
                child.GetComponent<Animator>().SetBool("Fruit", false);
            }
        }
        //Load Berry Trees
        foreach (Transform child in trees.transform.GetChild(1))
        {
            if (child.gameObject.activeSelf == false)
            {
                continue;
            }
            var treeData = dataSave.berryTrees[child.GetSiblingIndex()];
            child.GetComponent<Berries>().isFruit = treeData.isFruit;
            child.GetComponent<Berries>().timeColldowTree = treeData.timeColldowTree;
        }
        countLoad++;
        // Debug.Log(countLoad);
    }

    //Save/Load map
    public void Savemap()
    {
        dataSave.map = map.GetComponent<ListMap>().mapList;
    }
    public void Loadmap(DataSave dataSave)
    {
        Debug.Log("Loadmap");
        foreach (Transform child in map.transform)
        {
            child.gameObject.GetComponent<ActiveManage>().objectActived = dataSave.map[child.GetSiblingIndex()];
            child.gameObject.GetComponent<ActiveManage>().HandleUpdateActive();
        }
        countLoad++;
        // Debug.Log(countLoad);
    }

    //Save/Load Friend
    public void SaveFriend()
    {
        foreach (var friend in player.GetComponent<Player>().friendsList)
        {
            dataSave.friendsList.Add(friend);
        }
        foreach (var invite in player.GetComponent<Player>().inviteList)
        {
            dataSave.inviteList.Add(invite);
        }
    }
    public void LoadFriend(DataSave dataSave)
    {
        Debug.Log("LoadFriend");
        foreach (var friend in dataSave.friendsList)
        {
            player.GetComponent<Player>().friendsList.Add(friend);
        }
        foreach (var invite in dataSave.inviteList)
        {
            player.GetComponent<Player>().inviteList.Add(invite);
        }
        countLoad++;
    }

    //Save/Load Order
    public void SaveOrder()
    {
        orderData orderData = new orderData();
        foreach (var item in RandomOrder.Instance.orderItems)
        {
            int index = itemDataList.FindIndex(x => x.itemCode == item.itemCode);
            orderData.orderUnlock.Add(index);
        }

        foreach (var order in OrderManager.Instance.orderItems)
        {
            List<orderData.itemInOrder> itemInOrders = new List<orderData.itemInOrder>();
            foreach (var item in order)
            {
                orderData.itemInOrder itemInOrder = new orderData.itemInOrder();
                itemInOrder.itemCode = itemDataList.FindIndex(x => x.itemCode == item.item.itemCode);
                itemInOrder.amount = item.amount;
                itemInOrders.Add(itemInOrder);
            }
            orderData.orderItems.Add(itemInOrders);
        }
        foreach (var prize in OrderManager.Instance.prizeL)
        {
            orderData.orderPrize.Add(prize);
        }
        orderData.quantityOrder = OrderManager.Instance.quantityOrder;
        dataSave.orderData = orderData;
    }
    public void LoadOrder(DataSave dataSave)
    {
        Debug.Log("LoadOrder");
        foreach (var item in dataSave.orderData.orderUnlock)
        {
            RandomOrder.Instance.orderItems.Add(itemDataList[item]);
        }
        if (dataSave.orderData.orderItems == null)
        {
            Debug.LogError("Order data is null!");
            countLoad++;
            return;
        }
        foreach (var order in dataSave.orderData.orderItems)
        {
            List<RandomOrder.Order> orders = new List<RandomOrder.Order>();
            foreach (var item in order)
            {
                RandomOrder.Order orderItem = new RandomOrder.Order(itemDataList[item.itemCode], item.amount);
                orders.Add(orderItem);
            }
            OrderManager.Instance.orderItems.Add(orders);
        }
        OrderManager.Instance.prizeL.Clear();
        foreach (var prize in dataSave.orderData.orderPrize)
        {
            OrderManager.Instance.prizeL.Add(prize);
        }
        OrderManager.Instance.quantityOrder = dataSave.orderData.quantityOrder;
        OrderManager.Instance.LoadOrder();
        countLoad++;
    }
}


public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
