using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarnChicken : Barn
{
    private void Start()
    {
        noteEat.SetActive(false);
        noteAnimalFood.SetActive(false);
    }
    private void Update()
    {
        if (foodUI.transform.GetChild(0).GetChild(3).GetComponent<Seed>().code != itemCode.nullItem && foodUI.activeSelf && typeAnimal == animal.chicken)
        {
            haveFood = true;
        }
        else
        {
            haveFood = false;
        }
        if (isEating)
        {
            if (Vector2.Distance(player.transform.position, noteEat.transform.position) < 1f)
            {
                noteEat.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CheckHaveFood();

                    if (haveFood)
                    {
                        audioSource.Play();
                        isEating = false;
                        noteEat.SetActive(false);
                        foreach (Inventory.Slot inventory in player.GetComponent<Player>().inventory.slots)
                        {
                            var slots = player.GetComponent<Player>().inventory.slots;
                            int index = slots.FindIndex(slot => slot.data.itemCode == inventory.data.itemCode);
                            var select = foodUI.transform.GetChild(0).GetChild(3);
                            if (inventory.data.itemCode == select.GetComponent<Seed>().code)
                            {
                                slots[index].RemoveItem();
                                if (slots[index].count == 0)
                                {
                                    haveFood = false;
                                    select.GetComponent<Seed>().code = itemCode.nullItem;
                                    select.GetChild(0).GetComponent<Image>().sprite = slotNull;
                                }
                            }
                        }
                    }
                    else
                    {
                        SelectFood(animal.chicken);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Tab))
                {
                    SelectFood(animal.chicken);
                }
            }
            else
            {
                noteEat.SetActive(false);
            }
        }

        //Ăn
        if (!isEating)
        {
            if (timeRun < timeEat)
            {
                timeRun += Time.deltaTime;
            }
            if (noteAnimalFood.activeSelf)
            {
                isAnimalFood = false;
            }
        }
        else
        {
            timeRun = 0;
        }
        Eating();

        if (isAnimalFood)
        {
            if (Vector2.Distance(player.transform.position, noteEat.transform.position) < 1f)
            {
                noteEat.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    noteAnimalFood.SetActive(false);
                    isAnimalFood = false;
                    player.GetComponent<Player>().inventory.Add(gameObject.GetComponent<Item>().data, quantity);
                    isEating = true;
                    timeRun = 0;
                }
            }
            else
            {
                noteEat.SetActive(false);
            }
        }
    }
}