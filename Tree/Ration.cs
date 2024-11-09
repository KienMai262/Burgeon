using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ration : MonoBehaviour
{
    [SerializeField] private float timeGrow = 10;
    [SerializeField] protected GameObject timeSlider;

    public Item item;
    public Collectable collectable;
    Animator anim;
    float keyGrow = 1;
    public bool isGrow;
    public bool isTime = true;
    public bool isHarvest = false;
    public bool isWater = false;
    public float time = 0;
    public float timeGrowRun = 0f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        item = GetComponent<Item>();
        collectable = GetComponent<Collectable>();
    }
    private void Update()
    {
        // if (!isGrow)
        //     StartCoroutine(Grow());
        // if (timeSlider.activeSelf && isTime)
        // {
        //     time += Time.deltaTime;
        //     TimeSlider();
        // }
        if (isGrow && isTime)
        {
            timeGrowRun += Time.deltaTime;

            if (timeGrowRun >= timeGrow)
            {
                anim.SetFloat("Grow", 4);
                timeSlider.SetActive(false);
                isHarvest = true;
                isGrow = false;
            }
            else if (timeGrowRun >= (2f / 3f) * timeGrow)
            {
                anim.SetFloat("Grow", 3);
            }
            else if (timeGrowRun >= (1f / 3f) * timeGrow)
            {
                anim.SetFloat("Grow", 2);
                if(!isWater)
                {
                    isTime = false;
                    isWater = true;
                }
            }
            else
            {
                anim.SetFloat("Grow", 1);
            }
        }
    }
    protected void TimeSlider()
    {
        if (!timeSlider.activeSelf)
        {
            timeSlider.SetActive(true);
        }
        timeSlider.GetComponent<Slider>().value = time / timeGrow;
    }

    IEnumerator Grow()
    {
        isGrow = true;
        yield return new WaitForSecondsRealtime(timeGrow);
        anim.SetFloat("Grow", keyGrow);
        if (keyGrow < 4 && isTime) keyGrow += 1;
        else if (keyGrow == 4)
        {
            timeSlider.SetActive(false);
            isHarvest = true;
        }
        if (keyGrow == 2)
        {
            isTime = false;
        }
        isGrow = false;
    }
    public void Harvest()
    {
        time = 0;
        timeGrowRun = 0;
        keyGrow = 1;
        isHarvest = false;
        isGrow = false;
        isWater = false;
    }
}
