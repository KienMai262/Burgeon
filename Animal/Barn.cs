using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Barn : MonoBehaviour
{
    public enum animal
    {
        cow,
        chicken
    }
    public animal typeAnimal;
    [SerializeField] protected GameObject noteEat;
    [SerializeField] protected GameObject noteAnimalFood;
    [SerializeField] protected GameObject water;
    [SerializeField] protected GameObject hay;
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject foodUI;
    [SerializeField] protected Sprite slotNull;
    [SerializeField] protected int quantity = 1;
    [SerializeField] protected float timeEat = 5;
    [SerializeField] protected AudioSource audioSource;

    public List<ItemData> foods = new List<ItemData>();

    public bool haveFood;
    public bool isEating;
    public bool isAnimalFood;
    public float timeRun;

    protected void SelectFood(animal type)
    {
        typeAnimal = type;
        foodUI.GetComponent<SelectUI>().ToggleSeed();
        foodUI.GetComponent<SelectUI>().slots.Clear();
        foodUI.transform.GetChild(0).GetChild(3).GetComponent<Seed>().code = itemCode.nullItem;
        foreach (ItemData food in foods)
        {
            foreach (Inventory.Slot inventory in player.GetComponent<Player>().inventory.slots)
            {
                if (food.itemCode == inventory.data.itemCode)
                {
                    foodUI.GetComponent<SelectUI>().slots.Add(food);
                }
            }
        }
        foodUI.GetComponent<SelectUI>().SelectFood();
    }
    protected void CheckHaveFood()
    {
        foreach (Inventory.Slot inventory in player.GetComponent<Player>().inventory.slots)
        {
            var slots = player.GetComponent<Player>().inventory.slots;
            int index = slots.FindIndex(slot => slot.data.itemCode == inventory.data.itemCode);
            var select = foodUI.transform.GetChild(0).GetChild(3);
            if (inventory.data.itemCode == select.GetComponent<Seed>().code)
            {
                if (slots[index].count == 0)
                {
                    haveFood = false;
                    select.GetComponent<Seed>().code = itemCode.nullItem;
                    select.GetChild(0).GetComponent<Image>().sprite = slotNull;
                }
            }
        }
    }
    protected void Eating()
    {
        if (timeRun >= timeEat)
        {
            noteAnimalFood.SetActive(true);
            isAnimalFood = true;
            if (hay != null)
                hay.SetActive(false);
            if (water != null)
                water.GetComponent<Animator>().SetBool("Water", false);

        }
    }
}
