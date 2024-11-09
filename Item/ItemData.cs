using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data", order = 51)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public itemCode itemCode;
    public Sprite sprite;
    public Sprite spriteSeed;
    public int price;
}
public enum itemCode
{
    nullItem = 0,
    rice = 1,
    corn = 2,
    carrot = 3,
    cauliflower = 4,
    tomato = 5,
    eggplant = 6,
    roseBlue = 7,
    cabbage = 8,
    pumpkin = 9,
    radishWhite = 10,
    radishPurple = 11,
    cabbagePink = 12,
    start = 13,
    cucumber = 14,
    Apple = 15,
    Orange = 16,
    Peach = 17,
    Pear = 18,
    berriesRed = 19,
    berriesBlue = 20,
    berriesPurple = 21,
    milkBlue = 22,
    milkPink = 23,
    milkGreen = 24,
    milkBrown = 25,
    milkPurple = 26,
    eggYellow = 27,
    eggBrown = 28,
    eggPink = 29,
    eggGreen = 30,
    eggBlue = 31,
}