using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActiveManage : MonoBehaviour
{
    [SerializeField] protected GameObject objectActive;
    [SerializeField] protected GameObject activeUI;
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject activeButton;
    [SerializeField] public Condition conditions;

    public bool objectActived = false;

    bool check = false;

    private void Start()
    {
        objectActive.SetActive(false);
        activeButton.SetActive(false);
        activeUI.SetActive(false);
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < 1f)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F) && !check)
            {
                OpenActiveUI();
                if (CheckItem())
                {
                    if (activeButton.activeSelf == false)
                    {
                        activeButton.SetActive(true);
                        activeButton.GetComponent<Button>().onClick.RemoveAllListeners();
                        activeButton.GetComponent<Button>().onClick.AddListener(() => Active(conditions));
                    }
                }
                else
                {
                    if (activeButton.activeSelf == true)
                    {
                        activeButton.SetActive(false);
                    }
                }
                check = true;
            }
        }
        else
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }


        if (activeUI.activeSelf == false)
        {
            check = false;
        }

    }
    public void HandleUpdateActive()
    {
        if (objectActived)
        {
            Debug.Log(gameObject.transform.GetSiblingIndex() + gameObject.name);
            gameObject.transform.parent.GetComponent<ListMap>().mapList[gameObject.transform.GetSiblingIndex()] = true;
            objectActive.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public void Active(Condition conditions)
    {
        for (int i = 0; i < player.GetComponent<Player>().inventory.slots.Count; i++)
        {
            if (conditions.item.itemCode == player.GetComponent<Player>().inventory.slots[i].data.itemCode)
            {
                Debug.Log(gameObject.name);
                player.GetComponent<Player>().inventory.gold -= conditions.gold;
                player.GetComponent<Player>().inventory.slots[i].count -= conditions.amount - 1;
                player.GetComponent<Player>().inventory.slots[i].RemoveItem();
                break;
            }
        }

        objectActived = true;
        HandleUpdateActive();
        activeUI.transform.parent.GetComponent<ActiveUI>().ToggleActive();
    }

    public void OpenActiveUI()
    {
        activeUI.transform.parent.GetComponent<ActiveUI>().ToggleActive();
        activeUI.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = conditions.level.ToString();
        activeUI.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = conditions.gold.ToString();
        activeUI.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = conditions.item.sprite;
        activeUI.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "x " + conditions.amount.ToString();
    }

    public bool CheckItem()
    {
        int count = 0;
        if (conditions.level <= player.GetComponent<Player>().level) count++;
        if (conditions.gold <= player.GetComponent<Player>().inventory.gold) count++;
        foreach (var playerItem in player.GetComponent<Player>().inventory.slots)
        {
            if (conditions.item.itemCode == playerItem.data.itemCode && conditions.amount <= playerItem.count)
            {
                count++;
            }
        }
        if (count == 3)
        {
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class Condition
{
    public int level;
    public int gold;
    public ItemData item;
    public int amount;
}
