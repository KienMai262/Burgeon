using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] protected GameObject menuPanel;
    [SerializeField] protected GameObject settingPanel;


    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    public static MenuController Instance { get; private set; }
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
        menuPanel.SetActive(false);
        settingPanel.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
            OnCloseInventory.Invoke();
        }
        else
        {
            menuPanel.SetActive(true);
            OnShowInventory.Invoke();
        }
    }

    public void ToggleSetting()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
        else
        {
            settingPanel.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
