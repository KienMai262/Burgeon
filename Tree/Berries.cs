using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berries : MonoBehaviour
{
    [SerializeField] protected GameObject note;
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject tree;
    [SerializeField] protected float colldownTree = 10f;
    [SerializeField] protected int choppingTimes = 3;

    public Item item;
    public Collectable collectable;
    public bool isFruit;
    public bool isDropFruit;
    public float timeColldowTree;
    private void Start()
    {
        note.SetActive(false);
        item = GetComponent<Item>();
        collectable = GetComponent<Collectable>();
    }
    private void Update()
    {
        if (isFruit)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 0.8f)
            {
                note.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    isDropFruit = true;
                    choppingTimes--;
                    isFruit = false;
                    collectable.AddToInventory();
                    player.GetComponent<Player>().exp += (int)(gameObject.GetComponent<Item>().data.price / 10);
                    collectable.AddToInventory();
                    player.GetComponent<Player>().exp += (int)(gameObject.GetComponent<Item>().data.price / 10);
                    collectable.AddToInventory();
                    player.GetComponent<Player>().exp += (int)(gameObject.GetComponent<Item>().data.price / 10);
                    note.SetActive(false);
                    tree.SetActive(false);
                    timeColldowTree = 0;
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
                if(tree.activeSelf)
                {
                    tree.SetActive(false);
                }
            }
            else
            {
                ColldownTree();
            }
        }
    }
    protected void ColldownTree()
    {
        tree.SetActive(true);
        choppingTimes = 3;
        isFruit = true;
    }
}
