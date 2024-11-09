using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUI : MonoBehaviour
{
    [SerializeField] protected GameObject activeUI;

    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static ActiveUI Instance { get; private set; }
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


    public void ToggleActive()
    {
        if (activeUI.activeSelf)
        {
            activeUI.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            activeUI.SetActive(true);
            OnShowInventory.Invoke();
        }
    }
}
