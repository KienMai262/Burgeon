using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] public int inventorySize = 24;
    [SerializeField] protected GameObject nameUI;
    public string playerName;
    public string playerID;
    public Inventory inventory;
    public int level;
    public int exp;
    public int expNeed = 10;
    public List<string> friendsList = new List<string>();
    public List<string> inviteList = new List<string>();

    //
    public event Action OnShowInventory;
    public event Action OnCloseInventory;
    private void Awake()
    {
        inventory = new Inventory(inventorySize);
        if (nameUI == null)
        {
            nameUI = GameObject.Find("NameToPlayer");
            if (nameUI == null)
            {
                Debug.LogError("NameToPlayer not found");
            }
        }
    }
    //

    private void Start()
    {
        nameUI.SetActive(false);
    }
    private void Update()
    {
        if (exp >= expNeed)
        {
            level++;
            exp = exp - expNeed;
            expNeed += expNeed * 5 / 10;
        }
    }
    public void NameToPlayer()
    {
        nameUI.SetActive(true);
        OnShowInventory.Invoke();
    }
    public void AccName()
    {
        string testName = nameUI.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text;
        if (testName.Length < 4 || testName.Length > 20 || Regex.IsMatch(testName, @"[^a-zA-Z0-9]"))
        {
            nameUI.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = "Please enter a name with a number of characters from 4 to 20 and does not contain special characters!";
            nameUI.transform.GetChild(0).GetChild(1).GetComponent<TMP_InputField>().text = "";
        }
        else
        {
            playerName = testName;
            // Time.timeScale = 1;
            nameUI.SetActive(false);
            OnCloseInventory.Invoke();
            ReadWriteData.instance.SaveDataToFirebase(ReadWriteData.instance.auth.UserId);
        }
    }
}
