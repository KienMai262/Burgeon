using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Dynamic;
using System.Linq;

[System.Serializable]
public class OrderManager : MonoBehaviour
{
    [SerializeField] protected GameObject orderPrefab;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected GameObject orderPanel;
    [SerializeField] protected GameObject note;
    [SerializeField] protected GameObject player;
    [SerializeField] protected float timeF = 5f;
    // public RandomOrder randomOrder;
    public int quantityOrderInTheDay = 20;
    public int maxQuantityOrder = 8;

    [SerializeField] public List<List<RandomOrder.Order>> orderItems = new List<List<RandomOrder.Order>>();
    [SerializeField] public List<int> prizeL = new List<int>();

    public int quantityOrder = 0;
    public float time;
    public bool timerIsRunning = false;

    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static OrderManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    //

    private void Start()
    {
        note.SetActive(false);
        time = timeF;
        // RandomOrder.Instance.orderItems = new List<ItemData>(ReadWriteData.instance.itemDataList);
    }
    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < 1f)
        {
            note.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleOrder();
            }
        }
        else
        {
            note.SetActive(false);
        }

        if (timerIsRunning && quantityOrder < quantityOrderInTheDay && orderItems.Count < maxQuantityOrder)
        {
            if (time > 0)
            {
                // Giảm thời gian còn lại
                time -= Time.deltaTime;
                // Cập nhật giao diện
                UpdateTimerDisplay(time);
            }
            else
            {
                CreateOrder();
            }
        }
        else
        {
            orderPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", 0, 0);
        }
    }



    void UpdateTimerDisplay(float timeToDisplay)
    {
        // Định dạng lại thời gian hiển thị (phút:giây)
        timeToDisplay += 1; // Để thời gian hiện thị không về số âm

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); // Lấy số phút
        float seconds = Mathf.FloorToInt(timeToDisplay % 60); // Lấy số giây còn lại

        // Cập nhật text UI với định dạng mm:ss
        orderPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void ToggleOrder()
    {
        if (orderPanel.activeSelf)
        {
            orderPanel.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            orderPanel.SetActive(true);
            OnShowInventory.Invoke();
            RandomOrder.Instance.orderAmounts.Clear();
            RandomOrder.Instance.prize = 0;
            LoadQuantityOrder();
        }
    }

    public void AddOrder()
    {
        if (orderItems.Count >= maxQuantityOrder)
        {
            Debug.Log("Max order");
            return;
        }
        if (quantityOrder >= quantityOrderInTheDay)
        {
            Debug.Log("Max order in the day");
            return;
        }
        RandomOrder.Instance.RandomOrderItem();
        orderItems.Add(new List<RandomOrder.Order>(RandomOrder.Instance.orderAmounts));
        prizeL.Add(RandomOrder.Instance.prize);
        quantityOrder++;
        LoadQuantityOrder();
        ShowOrder(RandomOrder.Instance.orderAmounts, RandomOrder.Instance.prize);
        RandomOrder.Instance.orderAmounts.Clear();
        RandomOrder.Instance.prize = 0;
    }

    public void LoadOrder()
    {
        foreach (var order in orderItems)
        {
            ShowOrder(order, prizeL[orderItems.IndexOf(order)]);
        }
        LoadQuantityOrder();
    }
    public void LoadQuantityOrder()
    {
        orderPanel.transform.GetChild(3).GetComponent<TMP_Text>().text = quantityOrder.ToString() + "/" + quantityOrderInTheDay.ToString();
    }

    protected void ShowOrder(List<RandomOrder.Order> order, int prize)
    {
        var orderShow = Instantiate(orderPrefab, Vector3.zero, Quaternion.identity);
        orderShow.transform.SetParent(content, false);
        orderShow.transform.GetChild(0).GetChild(3).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Send);
        foreach (Transform slot in orderShow.transform.GetChild(0).GetChild(0))
        {
            slot.gameObject.SetActive(false);
        }
        foreach (var item in order)
        {
            var obj = orderShow.transform.GetChild(0).GetChild(0).GetChild(order.IndexOf(item)).gameObject;
            obj.SetActive(true);
            obj.GetComponent<UnityEngine.UI.Image>().sprite = item.item.sprite;
            obj.GetComponentInChildren<TMP_Text>().text = "X" + item.amount.ToString();
        }
        orderShow.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = prize.ToString();
    }

    public void Send()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (button != null)
        {
            int index = button.transform.parent.parent.GetSiblingIndex();
            int kt = 0;
            foreach (var items in orderItems[index])
            {
                foreach (var item in player.GetComponent<Player>().inventory.slots)
                {
                    if (item.data.itemCode == items.item.itemCode)
                    {
                        if (item.count >= items.amount)
                        {
                            kt++;
                        }
                        else
                        {
                            Debug.Log("Not enough item");
                            return;
                        }
                    }
                }
            }
            if (kt == orderItems[index].Count)
            {
                foreach (var items in orderItems[index])
                {
                    foreach (var item in player.GetComponent<Player>().inventory.slots)
                    {
                        if (item.data.itemCode == items.item.itemCode)
                        {
                            item.count -= items.amount;
                            if (item.count == 0)
                            {
                                item.data.itemName = "";
                                item.data.itemCode = itemCode.nullItem;
                                item.data.sprite = null;
                            }
                        }
                    }
                }
                player.GetComponent<Player>().inventory.gold += prizeL[index];
                player.GetComponent<Player>().exp += (int)(prizeL[index] / 10);
                orderItems.RemoveAt(index);
                Destroy(content.GetChild(index).gameObject);
                prizeL.RemoveAt(index);
            }
        }
    }

    public void CreateOrder()
    {
        AddOrder();
        time = timeF;
    }
}