using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    [SerializeField] protected GameObject seedPanel;
    [SerializeField] protected Seed itemPrefab;
    [SerializeField] protected RectTransform content;

    public List<ItemData> slots = new List<ItemData>();

    public itemCode code;
    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static SelectUI Instance { get; private set; }
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
        seedPanel.SetActive(false);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     ToggleSeed();
        // }
    }
    public void ToggleSeed()
    {
        if (seedPanel.activeSelf)
        {
            seedPanel.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            seedPanel.SetActive(true);
            OnShowInventory.Invoke();
        }
    }

    public void SelectFood()
    {
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
        }
        Debug.Log(slots.Count);
        for (int i = 0; i < slots.Count; i++)
        {
            if (i <= content.childCount - 1)
            {
                content.GetChild(i).gameObject.SetActive(true);
                content.GetChild(i).GetChild(0).GetComponent<Image>().sprite = slots[i].sprite;
                content.GetChild(i).GetComponent<Seed>().code = slots[i].itemCode;
            }
            else
            {
                Seed item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                item.transform.SetParent(content, false);
                item.transform.GetChild(0).GetComponent<Image>().sprite = slots[i].sprite;
                item.GetComponent<Seed>().code = slots[i].itemCode;
            }
        }
    }
}
