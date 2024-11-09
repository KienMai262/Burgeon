using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedUI : MonoBehaviour
{
    [SerializeField] protected GameObject seedPanel;
    [SerializeField] protected SlotUI itemPrefab;
    [SerializeField] protected RectTransform content;
    [SerializeField] protected GameObject selecSeed;

    public List<SlotUI> slots = new List<SlotUI>();

    public itemCode code;
    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static SeedUI Instance { get; private set; }
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
        code = selecSeed.GetComponent<Seed>().code;
        seedPanel.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleSeed();
        }

    }
    public void ToggleSeed()
    {
        if (seedPanel.activeSelf)
        {
            seedPanel.SetActive(false);
            OnCloseInventory.Invoke();
            code = selecSeed.GetComponent<Seed>().code;
        }
        else
        {
            seedPanel.SetActive(true);
            OnShowInventory.Invoke();
            code = selecSeed.GetComponent<Seed>().code;
        }
    }
}

