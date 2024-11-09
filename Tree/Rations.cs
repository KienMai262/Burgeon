using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rations : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject note;
    [SerializeField] protected GameObject noteHarvest;
    [SerializeField] protected GameObject noteWater;
    [SerializeField] protected GameObject location;
    [SerializeField] protected GameObject seedUI;
    [SerializeField] public List<GameObject> rationList;

    bool isForest = false;
    bool isHarvest = false;
    int index = 0;
    itemCode itemCode = itemCode.nullItem;
    public int indexRationData = -1;
    public int saveIndexRationData = -1;

    private void Start()
    {
        note.SetActive(false);
        noteHarvest.SetActive(false);
        noteWater.SetActive(false);
        foreach (GameObject ration in rationList)
        {
            ration.SetActive(false);
        }
    }

    private void Update()
    {
        //Chọn hạt giống
        if (seedUI.transform.parent.GetComponent<SeedUI>().code != itemCode.nullItem)
        {
            itemCode = seedUI.transform.parent.GetComponent<SeedUI>().code;
        }
        //Kiểm tra xem người chơi có ở gần cây không
        isHarvest = rationList[index].GetComponent<Ration>().isHarvest;
        if (Vector2.Distance(player.transform.position, transform.position) < .5f && !isForest)
        {
            note.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                int indexSet = -1;
                if (seedUI.activeSelf)
                {
                    seedUI.transform.parent.GetComponent<SeedUI>().ToggleSeed();
                }
                else
                {
                    if (itemCode == itemCode.nullItem)
                    {
                        seedUI.transform.parent.GetComponent<SeedUI>().ToggleSeed();
                        itemCode = seedUI.transform.parent.GetComponent<SeedUI>().code;
                    }
                    else
                    foreach (GameObject ration in rationList)
                    {
                        // Debug.Log(ration.GetComponent<Ration>().item.data.itemCode + " " + itemCode);
                        if (ration.GetComponent<Item>().data.itemCode == itemCode)
                        {
                            indexSet = rationList.IndexOf(ration);
                            break;
                        }
                    }
                }
                if (indexSet != -1)
                {
                    indexRationData = indexSet;
                    saveIndexRationData = indexSet;
                }
            }
        }
        else
        {
            note.SetActive(false);
        }
        //Trồng cây
        if (indexRationData != -1)
        {
            ActivateRation(indexRationData);
            indexRationData = -1;
        }
        //kiểm tra thu hoạch
        if (isHarvest && Vector2.Distance(player.transform.position, transform.position) < .5f)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Harvest();
            }
        }

        if (Vector2.Distance(player.transform.position, transform.position) < .5f)
        {
            noteHarvest.SetActive(true);
        }
        else
        {
            noteHarvest.SetActive(false);
        }

        //Kiểm tra tưới nước
        if (Vector2.Distance(player.transform.position, transform.position) < .5f && !rationList[index].GetComponent<Ration>().isTime)
        {
            noteWater.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(WaterAnim(true, 0f));
                StartCoroutine(WaterAnim(false, 1.5f));
            }
        }
        else
        {
            noteWater.SetActive(false);
        }
    }
    public void ActivateRation(int index)
    {
        StartCoroutine(AnimatorPlayerHoe(0, true));
        rationList[index].GetComponent<Ration>().isGrow = true;
        isForest = true;
        location.SetActive(false);
        rationList[index].SetActive(true);
        // rationList[index].GetComponent<Ration>().Harvest();
        this.index = index;
        StartCoroutine(AnimatorPlayerHoe(1.5f, false));
    }
    public void LoadRation(int index)
    {
        rationList[index].GetComponent<Ration>().isGrow = true;
        isForest = true;
        location.SetActive(false);
        rationList[index].SetActive(true);
        // rationList[index].GetComponent<Ration>().Harvest();
        this.index = index;
    }
    IEnumerator AnimatorPlayerHoe(float time, bool isHoe)
    {
        yield return new WaitForSeconds(time);
        SoundPlayer.Instance.PlaySoundHoe();
        player.GetComponent<Animator>().SetBool("Hoe", isHoe);
        player.GetComponent<MoveController>().isMoving = !isHoe;
    }

    IEnumerator WaterAnim(bool isWater, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        noteWater.SetActive(false);
        SoundPlayer.Instance.PlaySoundWatering();
        player.GetComponent<Animator>().SetBool("Spray", isWater);
        player.GetComponent<MoveController>().isMoving = !isWater;
        if (time != 0)
        {
            rationList[index].GetComponent<Ration>().isTime = true;
        }
    }
    public void Harvest()
    {

        foreach (GameObject ration in rationList)
        {
            if (ration.activeSelf)
            {
                ration.GetComponent<Ration>().collectable.AddToInventory();
                ration.GetComponent<Ration>().Harvest();
                ration.SetActive(false);
                player.GetComponent<Player>().exp += (int)(ration.GetComponent<Ration>().item.data.price/10);
                break;
            }
        }
        isForest = false;
        location.SetActive(true);
        isHarvest = false;
        saveIndexRationData = -1;
    }
}
