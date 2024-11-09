using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTree : MonoBehaviour
{
    [SerializeField] protected GameObject note;
    [SerializeField] protected GameObject player;
    [SerializeField] protected float colldownTree = 10f;

    public Item item;
    public Collectable collectable;
    public bool isFruit;
    // public bool isDropFruit;
    // public bool isChopping;
    // public bool isBearsFruit;
    public bool check = true;
    protected Animator anim;
    protected int choppingTimes = 3;
    public float timeColldowTree;
    private void Start()
    {
        anim = GetComponent<Animator>();
        note.SetActive(false);
        item = GetComponent<Item>();
        collectable = GetComponent<Collectable>();
    }
    private void Update()
    {
        if (isFruit)
        {
            anim.SetBool("Fruit", true);
            if (Vector3.Distance(transform.position, player.transform.position) < 0.9f)
            {
                note.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F) && check)
                {
                    if (choppingTimes == 1)
                    {
                        // isDropFruit = true;
                        StartCoroutine(SetAnimation(true, 0, "DropFruit"));
                        choppingTimes--;
                        StartCoroutine(SetAnimation(false, 1.4f, "DropFruit"));
                        isFruit = false;
                        collectable.AddToInventory();
                        player.GetComponent<Player>().exp += (int)(gameObject.GetComponent<Item>().data.price / 10);
                        collectable.AddToInventory();
                        player.GetComponent<Player>().exp += (int)(gameObject.GetComponent<Item>().data.price / 10);
                        collectable.AddToInventory();
                        player.GetComponent<Player>().exp += (int)(gameObject.GetComponent<Item>().data.price / 10);
                        note.SetActive(false);
                        timeColldowTree = 0;
                    }
                    else
                    {
                        StartCoroutine(SetAnimation(true, 0, "Chop"));
                        choppingTimes--;
                        StartCoroutine(SetAnimation(false, 1.4f, "Chop"));
                    }
                }
            }
            else
            {
                note.SetActive(false);
            }
        }
        else
        {
            if (timeColldowTree < colldownTree)
            {
                timeColldowTree += Time.deltaTime;
            }
            else
            {
                ColldownTree();
            }
        }
    }
    IEnumerator SetAnimation(bool value, float f, string name)
    {
        yield return new WaitForSecondsRealtime(f);
        check = !value;
        if (name != "BearsFruit")
        {
            SoundPlayer.Instance.PlaySoundChop();
            player.GetComponent<MoveController>().isMoving = !value;
            player.GetComponent<Animator>().SetBool("Chop", value);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        anim.SetBool(name, value);
    }

    protected void ColldownTree()
    {
        StartCoroutine(SetAnimation(true, 0, "BearsFruit"));
        StartCoroutine(SetAnimation(false, 0.5f, "BearsFruit"));
        choppingTimes = 3;
        isFruit = true;
    }
}
